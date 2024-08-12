using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AdventureWorks.BAL.Service
{
    public static class ResponseToDynamic
    {
        public static string getFields(string query = "")
        {
            var queryParts = query.Split('&');
            var fields = (queryParts.Where(x => x.StartsWith("fields=")).FirstOrDefault() ?? "").Replace("fields=", "");
            fields = fields?.Trim() ?? string.Empty;
            return fields;
        }
        public static Dictionary<string, string> getInclude(string query = "")
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();

            var queryParts = query.Split('&');
            var include = (queryParts.Where(x => x.StartsWith("include=")).FirstOrDefault() ?? "").Replace("include=", "");
            include = include?.Trim() ?? string.Empty;
            var listOfInclude = include.Split(';');
            foreach (var item in listOfInclude)
            {
                var b = item.TrimEnd(')');

                var a = b.Split('(');
                if (a.Length == 2)
                    keyValuePairs.Add(a[0], a[1]);
                else
                    keyValuePairs.Add(item, "");
            }
            return keyValuePairs;
        }
        public static async Task<dynamic> contextResponse(IQueryable result, string query = "")
        {
            var queryParts = query.Split('&');
            var filters = (queryParts.Where(x => x.StartsWith("filters=")).FirstOrDefault() ?? "").Replace("filters=", "");
            var fields = (queryParts.Where(x => x.StartsWith("fields=")).FirstOrDefault() ?? "").Replace("fields=", "");
            var sort = (queryParts.Where(x => x.StartsWith("sort=")).FirstOrDefault() ?? "").Replace("sort=", "");

            int.TryParse((queryParts.Where(x => x.StartsWith("pageNo=")).FirstOrDefault() ?? "").Replace("pageNo=", ""), out int pageNo);
            int.TryParse((queryParts.Where(x => x.StartsWith("pageSize=")).FirstOrDefault() ?? "").Replace("pageSize=", ""), out int pageSize);

            filters = ConvertFiqlToLinq.FiqlToLinq(filters);

            fields = fields?.Trim() ?? string.Empty;
            filters = filters?.Trim() ?? string.Empty;
            sort = sort?.Trim() ?? string.Empty;

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