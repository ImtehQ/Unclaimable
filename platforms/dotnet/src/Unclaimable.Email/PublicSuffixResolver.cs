using System;
using System.Collections.Generic;

namespace Unclaimable.Email;

/// <summary>
/// Deterministic suffix-aware registrant extraction. Matches longest explicit and
/// wildcard rules, with PSL-style exception rules taking precedence.
/// </summary>
internal static class PublicSuffixResolver
{
    // Bundled commonly used ICANN suffix rules. Extend from a pinned PSL snapshot
    // when the dataset is refreshed; never fetch network data at validation time.
    private const string Rules =
        "co.uk org.uk gov.uk ac.uk sch.uk net.uk ltd.uk plc.uk me.uk " +
        "com.au net.au org.au edu.au gov.au asn.au id.au " +
        "co.nz net.nz org.nz govt.nz ac.nz school.nz " +
        "co.jp ne.jp or.jp ac.jp go.jp ed.jp gr.jp lg.jp " +
        "com.br net.br org.br gov.br edu.br " +
        "com.mx org.mx gob.mx edu.mx " +
        "com.sg net.sg org.sg edu.sg gov.sg " +
        "com.tr net.tr org.tr edu.tr gov.tr " +
        "co.za org.za net.za gov.za ac.za edu.za school.za " +
        "co.in firm.in net.in org.in gen.in ind.in ac.in edu.in res.in gov.in mil.in " +
        "com.cn net.cn org.cn gov.cn edu.cn " +
        "com.hk net.hk org.hk edu.hk gov.hk " +
        "com.tw net.tw org.tw edu.tw gov.tw " +
        "com.ar org.ar net.ar gov.ar edu.ar " +
        "com.co net.co org.co gov.co edu.co " +
        "com.ng org.ng gov.ng edu.ng " +
        "com.pk net.pk org.pk edu.pk gov.pk " +
        "com.my net.my org.my gov.my edu.my " +
        "com.ph net.ph org.ph gov.ph edu.ph " +
        "com.bd org.bd gov.bd edu.bd net.bd " +
        "com.eg net.eg org.eg gov.eg edu.eg " +
        "com.ua net.ua org.ua gov.ua edu.ua " +
        "com.pl net.pl org.pl gov.pl edu.pl " +
        "com.sa net.sa org.sa gov.sa edu.sa " +
        "*.ck !www.ck *.kawasaki.jp !city.kawasaki.jp";

    private static readonly HashSet<string> KnownRules = new HashSet<string>(
        Rules.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries),
        StringComparer.Ordinal);

    internal static string GetRegistrantLabel(string domain)
    {
        var labels = domain.Split('.');
        if (labels.Length < 2) return labels[0];

        var bestSuffixLength = 1;
        var exceptionLength = 0;
        for (var index = 0; index < labels.Length; index++)
        {
            var suffix = string.Join(".", labels, index, labels.Length - index);
            var suffixLength = labels.Length - index;
            if (KnownRules.Contains("!" + suffix))
                exceptionLength = Math.Max(exceptionLength, suffixLength);
            if (KnownRules.Contains(suffix))
                bestSuffixLength = Math.Max(bestSuffixLength, suffixLength);
            if (index + 1 < labels.Length)
            {
                var wildcardSuffix = "*." + string.Join(".", labels, index + 1, labels.Length - index - 1);
                if (KnownRules.Contains(wildcardSuffix))
                    bestSuffixLength = Math.Max(bestSuffixLength, suffixLength);
            }
        }
        if (exceptionLength > 0)
            bestSuffixLength = exceptionLength - 1;
        var registrantIndex = labels.Length - bestSuffixLength - 1;
        return labels[Math.Max(0, registrantIndex)];
    }
}
