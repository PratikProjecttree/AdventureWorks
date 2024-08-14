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
            if (string.IsNullOrEmpty(fiql)) { return string.Empty; }

            // Split by semicolon for AND, and comma for OR
            // fiql = fiql.Replace(" AND ", ";");
            // fiql = fiql.Replace(" OR ", ",");
            // fiql = fiql.Replace(" and ", ";");
            // fiql = fiql.Replace(" or ", ",");
            fiql = fiql.Replace("\\", "\\\\");
            fiql = fiql.Replace(">=", "=ge=");
            fiql = fiql.Replace("<=", "=le=");
            fiql = fiql.Replace("!=", "=ne=");
            fiql = fiql.Replace("<", "=lt=",comparisonType: StringComparison.CurrentCulture);
            fiql = fiql.Replace(">", "=gt=");


            var andConditions = SplitConditions(fiql, ',');
            var linqConditions = new List<string>();

            foreach (var andCondition in andConditions.Where(condition => !string.IsNullOrEmpty(condition)))
            {
                var orConditions = SplitConditions(andCondition, ';');
                var linqOrConditions = new List<string>();

                foreach (var orCondition in orConditions.Where(condition => !string.IsNullOrEmpty(condition)))
                {
                    if (SplitConditions(orCondition, ',').Count() > 1)
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

                linqConditions.Add($"{startRoundBracate(linqOrConditions)}{string.Join(" AND ", linqOrConditions)}{endRoundBracate(linqOrConditions)}");
            }

            return $"{startRoundBracate(linqConditions)}{string.Join(" OR ", linqConditions)}{endRoundBracate(linqConditions)}";
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

            var andConditions = SplitConditions(fiql, ',');
            var linqConditions = new List<string>();

            foreach (var andCondition in andConditions.Where(condition => !string.IsNullOrEmpty(condition)))
            {
                var orConditions = SplitConditions(andCondition, ';');
                var linqOrConditions = new List<string>();

                foreach (var orCondition in orConditions.Where(condition => !string.IsNullOrEmpty(condition)))
                {
                    if (SplitConditions(orCondition, ',').Count() > 1)
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
                        "ilike" => "LIKE",
                        "olike" => "NOT LIKE",
                        _ => throw new ArgumentException($"Unsupported operator: {op}")
                    };

                    if (linqOp == "IN")
                    {
                        value = value.Trim('(', ')');
                        var values = value.Split(',').Select(v => (!int.TryParse(value, out _) && !decimal.TryParse(value, out _)) ? "\"" + v.Trim() + "\"" : v.Trim()).ToList();
                        value = $"new [] {{ {string.Join(", ", values)} }}";
                        linqOrConditions.Add($"{property} in {value}");
                    }
                    else if (linqOp == "NOT IN")
                    {
                        value = value.Trim('(', ')');
                        var values = value.Split(',').Select(v => (!int.TryParse(value, out _) && !decimal.TryParse(value, out _)) ? "\"" + v.Trim() + "\"" : v.Trim()).ToList();
                        value = $"new [] {{ {string.Join(", ", values)} }}";
                        linqOrConditions.Add($"!({property} in {value})");
                    }
                    else if (linqOp == "LIKE")
                    {
                        if (value.StartsWith('*') && value.EndsWith('*'))
                        {
                            value = $"\"{value.Replace("*", "")}\"";
                            linqOrConditions.Add($"{property}.Contains({value})");
                        }
                        else if (value.StartsWith('*'))
                        {
                            value = $"\"{value.Replace("*", "")}\"";
                            linqOrConditions.Add($"{property}.EndsWith({value})");
                        }
                        else if (value.EndsWith('*'))
                        {
                            value = $"\"{value.Replace("*", "")}\"";
                            linqOrConditions.Add($"{property}.StartsWith({value})");
                        }
                    }
                    else if (linqOp == "NOT LIKE")
                    {
                        if (value.StartsWith('*') && value.EndsWith('*'))
                        {
                            value = $"\"{value.Replace("*", "")}\"";
                            linqOrConditions.Add($"!({property}.Contains({value}))");
                        }
                        else if (value.StartsWith('*'))
                        {
                            value = $"\"{value.Replace("*", "")}\"";
                            linqOrConditions.Add($"!({property}.EndsWith({value}))");
                        }
                        else if (value.EndsWith('*'))
                        {
                            value = $"\"{value.Replace("*", "")}\"";
                            linqOrConditions.Add($"!({property}.StartsWith({value}))");
                        }
                    }
                    else
                    {
                        if (!int.TryParse(value, out _) && !decimal.TryParse(value, out _))
                        {
                            if (value?.ToLower() == "null")
                            {
                                linqOrConditions.Add($"{property} {linqOp} null");
                            }
                            else
                            {
                                value = $"\"{value}\"";
                                linqOrConditions.Add($"{property} {linqOp} {value}");
                            }
                        }
                        else
                        {
                            linqOrConditions.Add($"{property} {linqOp} {value}");
                        }

                    }
                }

                linqConditions.Add($"{startRoundBracate(linqOrConditions)}{string.Join(" AND ", linqOrConditions)}{endRoundBracate(linqOrConditions)}");
            }
            return $"{startRoundBracate(linqConditions)}{string.Join(" OR ", linqConditions)}{endRoundBracate(linqConditions)}";
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