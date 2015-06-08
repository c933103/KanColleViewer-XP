using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper
{
    public class ProxyRule
    {
        public bool Enabled { get; set; }
        public bool Negative { get; set; }
        public string Name { get; set; }
        public string Pattern { get; set; }
        public MatchPortion MatchIn { get; set; }
        public MatchType Type { get; set; }
        public MatchAction Action { get; set; }
        public string ActionString { get; set; }
        public IMatcher Matcher => _matcher ?? (_matcher = CreateMatcher());

        private IMatcher _matcher;
        private int _jumpTarget;

        private IMatcher CreateMatcher()
        {
            switch(Type) {
                case MatchType.Regex:
                    return new RegexMatcher(Pattern);
                case MatchType.ShExp:
                    return new ShExpMatcher(Pattern);
                case MatchType.Any:
                    return new AnyMatcher() { IsValid = true };
                default:
                    return new AnyMatcher() { IsValid = false };
            }
        }

        public enum MatchAction { Proxy, SystemProxy, Block, Goto, GotoName, SetProxyAuth }
        public enum MatchType { ShExp, Regex, Any }
        public enum MatchPortion { Host, Port, HostAndPort, Method, Path, Query, PathAndQuery, FullUrl }

        private static ProxyRule Default => new ProxyRule() { Action = MatchAction.SystemProxy, Enabled = true, Type = MatchType.Any, Negative = false };

        public struct ExecutionResult
        {
            public MatchAction Action { get; internal set; }
            public string Proxy { get; internal set; }
            public string ProxyAuth { get; internal set; }

            public static readonly ExecutionResult Default = new ExecutionResult() { Action = MatchAction.SystemProxy };
        }

        public static ProxyRule[] CompileRule(IDictionary<int, ProxyRule> rules)
        {
            if (rules == null) return new ProxyRule[] { Default };
            if (rules.Count == 0) return new ProxyRule[] { Default };

            var result = rules
                .Where(x => x.Value.Enabled && x.Value.Matcher.IsValid)
                .OrderBy(x => x.Key)
                .Select(x => new KeyValuePair<int, ProxyRule>(x.Key, (ProxyRule)x.Value.MemberwiseClone()))
                .Concat(EnumerableEx.Return(new KeyValuePair<int, ProxyRule>(int.MaxValue, Default))).ToArray();
            
            result.Where(x => x.Value.Action == MatchAction.Goto || x.Value.Action == MatchAction.GotoName).ForEach(x => CompileJumpTarget(result, x.Value));

            return result.Select(x => x.Value).ToArray();
        }

        private static void CompileJumpTarget(KeyValuePair<int, ProxyRule>[] rules, ProxyRule rule)
        {
            if (rule.Action == MatchAction.GotoName) {
                rule.Action = MatchAction.Goto;
                rule._jumpTarget = ((rules.Select((e, i) => new { Index = i, Item = e }).FirstOrDefault(x => x.Item.Value.Name == rule.ActionString)?.Index) ?? rules.Length - 1);
            } else if (rule.Action == MatchAction.Goto) {
                int number;
                if (!int.TryParse(rule.ActionString, out number)) number = int.MaxValue;
                rule._jumpTarget = rules.Select((e, i) => new { Index = i, Item = e }).First(x => x.Item.Key >= number).Index;
            }
        }

        public static ExecutionResult ExecuteRules(ProxyRule[] rules, string method, Uri uri)
        {
            var result = ExecutionResult.Default;
            for (int i = 0; i < rules.Length; i++) {
                var rule = rules[i];
                string match;
                switch(rule.MatchIn) {
                    case MatchPortion.FullUrl:
                        match = uri.AbsoluteUri;
                        break;
                    case MatchPortion.Host:
                        match = uri.Host;
                        break;
                    case MatchPortion.Port:
                        match = uri.Port.ToString();
                        break;
                    case MatchPortion.HostAndPort:
                        match = string.Format(uri.HostNameType == UriHostNameType.IPv6 ? "[{0}]:{1}" : "{0}:{1}", uri.Host, uri.Port);
                        break;
                    case MatchPortion.Method:
                        match = method;
                        break;
                    case MatchPortion.Path:
                        match = uri.AbsolutePath;
                        break;
                    case MatchPortion.Query:
                        match = uri.Query;
                        break;
                    case MatchPortion.PathAndQuery:
                        match = uri.PathAndQuery;
                        break;
                    default:
                        continue;
                }
                if (!rule.Negative ^ rule.Matcher.Match(match)) continue;
                switch(rule.Action) {
                    case MatchAction.Proxy:
                        result.Proxy = rule.ActionString;
                        goto case MatchAction.SystemProxy;
                    case MatchAction.Block:
                    case MatchAction.SystemProxy:
                        result.Action = rule.Action;
                        return result;
                    case MatchAction.SetProxyAuth:
                        result.ProxyAuth = rule.ActionString;
                        break;
                    case MatchAction.Goto:
                        i = rule._jumpTarget;
                        break;
                }
            }
            return result;
        }
    }

    public interface IMatcher {
        bool IsValid { get; }
        bool Match(string input);
    }

    class AnyMatcher : IMatcher
    {
        public bool IsValid { get; set; }
        public bool Match(string input) { return IsValid; }
    }

    class RegexMatcher : IMatcher
    {
        private readonly Regex regex;
        public RegexMatcher(string pattern) { try { regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.ECMAScript | RegexOptions.IgnoreCase); } catch { } }

        public bool IsValid => regex != null;
        public bool Match(string input) => regex?.IsMatch(input) == true;
    }

    class ShExpMatcher : IMatcher
    {
        public enum Token { Start, End, Literal, Question, Asterisk }
        struct ShellExpToken
        {
            public Token Type { get; set; }
            public string Literal { get; set; }
        }

        private readonly ShellExpToken[] pattern;
        public bool IsValid => pattern != null;

        public ShExpMatcher(string pattern)
        {
            pattern = pattern.ToLowerInvariant();
            StringBuilder sb = new StringBuilder();
            List<ShellExpToken> tokens = new List<ShellExpToken>();
            tokens.Add(new ShellExpToken { Type = Token.Start });
            for (int i = 0; i < pattern.Length; i++) {
                switch (pattern[i]) {
                    case '^':
                        if (i < pattern.Length - 1) {
                            sb.Append(pattern[++i]);
                        } else {
                            return;
                        }
                        break;
                    case '?':
                    case '*':
                        if (sb.Length != 0) {
                            tokens.Add(new ShellExpToken() { Type = Token.Literal, Literal = sb.ToString() });
                            sb.Length = 0;
                        }
                        tokens.Add(new ShellExpToken() { Type = pattern[i] == '*' ? Token.Asterisk : Token.Question });
                        break;
                    default:
                        sb.Append(pattern[i]);
                        break;
                }
            }
            if (sb.Length != 0) {
                tokens.Add(new ShellExpToken() { Type = Token.Literal, Literal = sb.ToString() });
            }
            tokens.Add(new ShellExpToken { Type = Token.End });
            this.pattern = tokens.Where((e, i) => e.Type != Token.Asterisk || tokens[i - 1].Type != Token.Asterisk).ToArray();
        }

        public bool Match(string input)
        {
            if (!IsValid) return false;
            input = input.ToLowerInvariant();
            int i = 0;
            int j = 0;
            int[] match = new int[pattern.Length];
            bool rewind = false;
            while (true) {
                if (!rewind) {
                    switch (pattern[i].Type) {
                        case Token.Start:
                            if (j != 0) return false;
                            match[i++] = 0;
                            continue;
                        case Token.End:
                            if (j == input.Length) return true;
                            rewind = true;
                            continue;
                        case Token.Question:
                            if (j >= input.Length) {
                                rewind = true;
                            } else {
                                match[i++] = ++j;
                            }
                            continue;
                        case Token.Literal:
                            if (string.CompareOrdinal(input, j, pattern[i].Literal, 0, pattern[i].Literal.Length) != 0) {
                                rewind = true;
                            } else {
                                match[i] = j = (j + pattern[i].Literal.Length);
                                i++;
                            }
                            continue;
                        case Token.Asterisk:
                            match[i++] = j = input.Length;
                            continue;
                    }
                } else {
                    switch (pattern[--i].Type) {
                        case Token.Asterisk:
                            if (match[i] == match[i - 1]) return false;
                            j = --match[i++];
                            rewind = false;
                            continue;
                        case Token.Start:
                            return false;
                        default:
                            continue;
                    }
                }
            }
        }
    }
}
