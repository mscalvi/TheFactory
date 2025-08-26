using System;
using System.Collections.Generic;
using System.Linq;
using ContaJunsta.Models;

namespace ContaJunsta.Services;

public class CalculationService
{
    public record PersonLite(string Id, string Name);
    public record PersonSummary(string Id, string Name, int Paid, int Should, int Balance);
    public record Transfer(string FromId, string ToId, int Cents);
    public record CalcResult(List<PersonSummary> Summaries, List<Transfer> Transfers);

    /// <summary>
    /// Regras:
    /// +cents = despesa (pagador recebe +cents; selecionados pagam quota floor)
    /// -cents = ganho   (responsável deve repassar abs(cents); selecionados recebem quota floor)
    /// Arredondamento por "baldes": somamos remainders e, no fechamento,
    /// distribuímos 1¢ por pessoa (ordem alfabética) entre quem participou de pelo menos 1 lançamento.
    /// </summary>
    public CalcResult Compute(IReadOnlyList<PersonLite> persons, IReadOnlyList<BillModel> bills)
    {
        var people = persons.ToList();
        var byId = people.ToDictionary(p => p.Id);

        // dicionários em centavos
        var paidExpenses = people.ToDictionary(p => p.Id, _ => 0); // só o que a pessoa PAGOU em despesas (>0)
        var shouldExpenses = people.ToDictionary(p => p.Id, _ => 0); // o que deveria pagar em despesas
        var shareGains = people.ToDictionary(p => p.Id, _ => 0); // o que deveria RECEBER em ganhos (reduz o "should")
        var shouldPayerGains = people.ToDictionary(p => p.Id, _ => 0); // quanto o responsável deve repassar em ganhos

        int bucketExpense = 0; // soma dos remainders de despesas
        int bucketGain = 0; // soma dos remainders de ganhos

        var active = new HashSet<string>(); // quem participou (payer ou selecionado)

        foreach (var b in bills ?? Array.Empty<BillModel>())
        {
            var sel = (b.ParticipantIds ?? new List<string>()).Where(byId.ContainsKey).ToList();
            if (sel.Count == 0) continue;

            var abs = Math.Abs(b.Cents);
            var q = abs / sel.Count;
            var r = abs % sel.Count;

            active.Add(b.ResponsiblePersonId ?? "");
            foreach (var s in sel) active.Add(s);

            if (b.Cents > 0)
            {
                // DESPESA
                if (byId.ContainsKey(b.ResponsiblePersonId ?? "")) paidExpenses[b.ResponsiblePersonId!] += abs;
                foreach (var s in sel) shouldExpenses[s] += q;
                bucketExpense += r;
            }
            else if (b.Cents < 0)
            {
                // GANHO (dinheiro que entrou e será distribuído)
                if (byId.ContainsKey(b.ResponsiblePersonId ?? "")) shouldPayerGains[b.ResponsiblePersonId!] += abs; // ele deve repassar
                foreach (var s in sel) shareGains[s] += q; // crédito para os selecionados
                bucketGain += r;
            }
        }

        // ordem para distribuição dos baldes (determinística). Usamos ALFABÉTICA (simples e previsível).
        var order = people
            .Where(p => active.Contains(p.Id))
            .OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
            .Select(p => p.Id)
            .ToList();

        // distribuições (quantos centavos cada 1 recebe dos baldes)
        var expenseAdj = people.ToDictionary(p => p.Id, _ => 0);
        var gainAdj = people.ToDictionary(p => p.Id, _ => 0);

        if (order.Count > 0)
        {
            for (int i = 0; i < bucketExpense; i++)
                expenseAdj[order[i % order.Count]] += 1;   // aumenta o que deve (debitamos 1¢)
            for (int i = 0; i < bucketGain; i++)
                gainAdj[order[i % order.Count]] += 1;      // reduz o que deve (creditamos 1¢)
        }

        // sumarização por pessoa
        var summaries = new List<PersonSummary>(people.Count);
        foreach (var p in people)
        {
            // Should = despesas (quota + ajuste) - ganhos (quota + ajuste) + "ganho" a repassar se a pessoa foi responsável
            var should =
                (shouldExpenses[p.Id] + expenseAdj[p.Id]) -
                (shareGains[p.Id] + gainAdj[p.Id]) +
                shouldPayerGains[p.Id];

            var paid = paidExpenses[p.Id];
            var balance = paid - should; // + recebe | - deve

            summaries.Add(new PersonSummary(p.Id, p.Name, paid, should, balance));
        }

        // simplificação de acertos (ganancioso: maior credor × maior devedor)
        var creditors = summaries.Where(s => s.Balance > 0).Select(s => (s.Id, s.Balance)).OrderByDescending(x => x.Balance).ToList();
        var debtors = summaries.Where(s => s.Balance < 0).Select(s => (s.Id, -s.Balance)).OrderByDescending(x => x.Item2).ToList();

        var transfers = new List<Transfer>();
        int ci = 0, di = 0;
        while (ci < creditors.Count && di < debtors.Count)
        {
            var take = Math.Min(creditors[ci].Balance, debtors[di].Item2);
            if (take > 0)
            {
                transfers.Add(new Transfer(debtors[di].Id, creditors[ci].Id, take));
                creditors[ci] = (creditors[ci].Id, creditors[ci].Balance - take);
                debtors[di] = (debtors[di].Id, debtors[di].Item2 - take);
            }
            if (creditors[ci].Balance == 0) ci++;
            if (debtors[di].Item2 == 0) di++;
        }

        return new CalcResult(summaries, transfers);
    }
}
