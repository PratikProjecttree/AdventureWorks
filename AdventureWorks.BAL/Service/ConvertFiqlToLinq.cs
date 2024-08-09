using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
namespace AdventureWorks.BAL.Service
{
    public static class ConvertFiqlToLinq
    {
        public static string FiqlToLinq(string fiql)
        {
            // Split by semicolon for AND, and comma for OR
            fiql = fiql.Replace(" AND ", ";");
            fiql = fiql.Replace(" OR ", ",");
            fiql = fiql.Replace(" and ", ";");
            fiql = fiql.Replace(" or ", ",");

            var andConditions = SplitConditions(fiql, ';');
            var linqConditions = new List<string>();

            foreach (var andCondition in andConditions)
            {
                var orConditions = SplitConditions(andCondition, ',');
                var linqOrConditions = new List<string>();

                foreach (var orCondition in orConditions)
                {
                    if (orCondition.Contains(";"))
                    {
                        var fiqlloopString = FiqlToLinqLoop(orCondition.TrimStart('(').TrimEnd(')'));
                        linqOrConditions.Add($"{fiqlloopString}");
                    }
                    else
                    {
                        var fiqlloopString = FiqlToLinqLoop(orCondition.TrimStart('(').TrimEnd(')'));
                        linqOrConditions.Add($"{fiqlloopString}");
                    }
                }

                linqConditions.Add($"{startRoundBracate(linqOrConditions)}{string.Join(" OR ", linqOrConditions)}{endRoundBracate(linqOrConditions)}");
            }

            return $"{startRoundBracate(linqConditions)}{string.Join(" AND ", linqConditions)}{endRoundBracate(linqConditions)}";
        }
        static string startRoundBracate(List<string> conditions)
        {
            if (conditions.Count > 1)
            {
                return "(";
            }
            else
            {
                return "";
            }
        }
        static string endRoundBracate(List<string> conditions)
        {
            if (conditions.Count > 1)
            {
                return ")";
            }
            else
            {
                return "";
            }
        }
        static string FiqlToLinqLoop(string fiql)
        {
            // Split by semicolon for AND, and comma for OR
            fiql = fiql.Replace(" AND ", ";");
            fiql = fiql.Replace(" OR ", ",");
            fiql = fiql.Replace(" and ", ";");
            fiql = fiql.Replace(" or ", ",");

            var andConditions = SplitConditions(fiql, ';');
            var linqConditions = new List<string>();

            foreach (var andCondition in andConditions)
            {
                var orConditions = SplitConditions(andCondition, ',');
                var linqOrConditions = new List<string>();

                foreach (var orCondition in orConditions)
                {
                    if (orCondition.Contains(";"))
                    {
                        var fiqlloopString = FiqlToLinqLoop(orCondition.TrimStart('(').TrimEnd(')'));
                        linqOrConditions.Add($"{fiqlloopString}");
                    }
                    var parts = orCondition.Split(new[] { '=' }, 3);
                    if (parts.Length < 3)
                    {
                        throw new ArgumentException("Invalid FIQL query");
                    }

                    string property = parts[0];
                    string op = parts[1];
                    string value = parts[2];

                    string linqOp = op switch
                    {
                        "gt" => ">",
                        "lt" => "<",
                        "ge" => ">=",
                        "le" => "<=",
                        "eq" => "==",
                        "ne" => "!=",
                        "==" => "==",
                        "" => "==",
                        "in" => "IN",
                        "out" => "NOT IN",
                        "like" => "LIKE",
                        _ => throw new ArgumentException($"Unsupported operator: {op}")
                    };

                    // Convert value if necessary (e.g., for strings add quotes)
                    if (linqOp == "IN")
                    {
                        value = value.Trim('(', ')'); // remove parentheses
                        var values = value.Split(',').Select(v => v.Trim()).ToList();
                        value = $"new [] {{ {string.Join(", ", values)} }}";
                        linqOrConditions.Add($"{property} in {value}");
                    }
                    else if (linqOp == "NOT IN")
                    {
                        value = value.Trim('(', ')'); // remove parentheses
                        var values = value.Split(',').Select(v => v.Trim()).ToList();
                        value = $"new [] {{ {string.Join(", ", values)} }}";
                        linqOrConditions.Add($"{property} not in {value}");
                    }
                    else if (linqOp == "LIKE")
                    {
                        value = $"\"{value.Replace('*', '%')}\""; // replace '*' with '%'
                        linqOrConditions.Add($"{property}.Contains({value})");
                    }
                    else
                    {
                        if (!int.TryParse(value, out _))
                        {
                            // String value
                            value = $"\"{value}\"";
                        }
                        linqOrConditions.Add($"{property} {linqOp} {value}");
                    }
                }

                linqConditions.Add($"{startRoundBracate(linqOrConditions)}{string.Join(" OR ", linqOrConditions)}{endRoundBracate(linqOrConditions)}");
            }
            return $"{startRoundBracate(linqConditions)}{string.Join(" AND ", linqConditions)}{endRoundBracate(linqConditions)}";
        }

        private static IEnumerable<string> SplitConditions(string query, char separator)
        {
            int depth = 0;
            List<int> splitIndexes = new List<int>();

            for (int i = 0; i < query.Length; i++)
            {
                if (query[i] == '(') depth++;
                if (query[i] == ')') depth--;
                if (depth == 0 && query[i] == separator)
                {
                    splitIndexes.Add(i);
                }
            }

            splitIndexes.Add(query.Length);

            int start = 0;
            foreach (int index in splitIndexes)
            {
                yield return query.Substring(start, index - start);
                start = index + 1;
            }
        }
    }
}