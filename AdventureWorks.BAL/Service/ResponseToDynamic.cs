using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Text.Json;
using System.Text.Json.Serialization;
using AdventureWorks.BAL.ResponseModel;

namespace AdventureWorks.BAL.Service
{
    public static class ResponseToDynamic
    {
        public static string getQueryParts(string query, string part)
        {
            var queryParts = SplitConditions(query, '&');
            var result = (queryParts.Where(x => x.StartsWith($"{part}=")).FirstOrDefault() ?? "").Replace($"{part}=", "");
            result = result?.Trim() ?? string.Empty;
            return result;
        }
        public static string getSort(string query)
        {
            return getQueryParts(query, "sort");
        }
        public static string getFilters(string query)
        {
            return getQueryParts(query, "filters");
        }
        public static string getFields(string query)
        {
            return getQueryParts(query, "fields");
        }
        public static string getPageNo(string query)
        {
            return getQueryParts(query, "pageno");
        }
        public static string getPageSize(string query)
        {
            return getQueryParts(query, "pagesize");
        }
        public static IEnumerable<QueryIncludeModel> getInclude(string query)
        {
            List<QueryIncludeModel> queryIncludes = new();
            var include = getQueryParts(query, "include");
            include = include?.Trim() ?? string.Empty;
            var listOfInclude = include.Split(';');
            foreach (var item in listOfInclude)
            {
                var queryInclude = new QueryIncludeModel();

                var b = item.TrimEnd('}');

                var a = b.Split('{');
                if (a.Length == 2)
                {
                    queryInclude.objectName = a[0];
                    queryInclude.objectQuery = a[1];
                    queryInclude.objectFields = ResponseToDynamic.getFields(queryInclude.objectQuery);
                    queryInclude.objectFilters = ResponseToDynamic.getFilters(queryInclude.objectQuery);
                }
                else
                    queryInclude.objectName = item;

                queryIncludes.Add(queryInclude);
            }
            return queryIncludes;
        }
        private static IEnumerable<string> SplitConditions(string query, char separator)
        {
            int depth = 0;
            List<int> splitIndexes = new List<int>();

            for (int i = 0; i < query.Length; i++)
            {
                if (query[i] == '{') depth++;
                if (query[i] == '}') depth--;
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
        public static async Task<dynamic> contextResponse(IQueryable result, string query)
        {
            query = query.Replace("\\", "\\\\");
            var filters = getFilters(query);
            var fields = getFields(query);
            var sort = getSort(query);

            int.TryParse(getPageNo(query), out int pageNo);
            int.TryParse(getPageSize(query), out int pageSize);

            filters = ConvertFiqlToLinq.FiqlToLinq(filters);

            if (!string.IsNullOrEmpty(fields))
            {
                result = result.Select($"new ({fields})");
            }
            if (!string.IsNullOrEmpty(filters))
            {
                result = result.Where(filters);
            }
            if (!string.IsNullOrEmpty(sort))
            {
                result = result.OrderBy(sort);
            }
            if (pageNo > 0 && pageSize > 0)
            {
                result = result.Skip((pageNo - 1) * pageSize).Take(pageSize);
            }
            return await result.ToDynamicListAsync();
        }
        public static dynamic ConvertTo<T>(T retVal, string select)
        {
            var options = new JsonSerializerOptions();

            var selectColumn = select.Split(',').ToHashSet<string>();
            options.Converters.Add(new DynamicResponseConverter<T>(selectColumn));

            dynamic json = JsonDocument.Parse(JsonSerializer.Serialize(retVal, options));

            return json;
        }
        public static dynamic ConvertTo<T>(List<T> retVal, string select)
        {
            var options = new JsonSerializerOptions();

            var selectColumn = select.Split(',').ToHashSet();
            if (!selectColumn.Any(x => string.IsNullOrEmpty(x)))
            {
                options.Converters.Add(new DynamicResponseConverter<T>(selectColumn));
            }

            JsonDocument json = JsonDocument.Parse(JsonSerializer.Serialize(retVal, options));

            return json;
        }
        internal class DynamicResponseConverter<T> : JsonConverter<T>
        {
            private readonly HashSet<string> _propertiesToIgnore;

            public DynamicResponseConverter(HashSet<string> propertiesToIgnore)
            {
                _propertiesToIgnore = propertiesToIgnore;
            }

            public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }

            public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();

                foreach (var property in typeof(T).GetProperties())
                {
                    if (_propertiesToIgnore.Any(x => x.ToLower() == property.Name.ToLower()) || _propertiesToIgnore.Count() == 0)
                    {
                        var propValue = property.GetValue(value);
                        writer.WritePropertyName(property.Name);
                        JsonSerializer.Serialize(writer, propValue, options);
                    }
                }

                writer.WriteEndObject();
            }
        }
    }
}