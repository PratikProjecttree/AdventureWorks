using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Linq;
namespace AdventureWorks.BAL.ODataToSqlConverter
{
    public class ODataToSqlConvert
    {
        public static string ConvertODataFilterToSql(SingleValueNode Expression)
        {
            return ConvertExpressionToSql(Expression);
        }

        private static string ConvertExpressionToSql(SingleValueNode node)
        {
            switch (node)
            {
                case ConstantNode constantNode:
                    return ConvertConstantToSql(constantNode);
                case BinaryOperatorNode binaryNode:
                    return ConvertBinaryOperatorToSql(binaryNode);
                case SingleValuePropertyAccessNode propertyAccessNode:
                    return ConvertPropertyAccessToSql(propertyAccessNode);
                case SingleValueFunctionCallNode functionCallNode:
                    return ConvertFunctionCallToSql(functionCallNode);
                default:
                    return node.Kind.ToString();
            }
        }
        private static string ConvertConstantToSql(ConstantNode constantNode)
        {
            if (constantNode.Value is string)
            {
                return $"'{constantNode?.Value?.ToString()?.Replace("'", "''")}'";
            }
            else if (constantNode.Value is bool)
            {
                return (bool)constantNode.Value ? "1" : "0";
            }
            else if (constantNode.Value is DateTime dateTime)
            {
                return $"'{dateTime:yyyy-MM-dd HH:mm:ss}'";
            }
            else if (constantNode.Value is DateTimeOffset dateTimeOffset)
            {
                return $"'{dateTimeOffset:yyyy-MM-dd HH:mm:ss}'";
            }
            else if (constantNode.Value is int || constantNode.Value is double || constantNode.Value is float)
            {
                return constantNode.Value.ToString() ?? String.Empty;
            }
            else
            {
                throw new NotSupportedException($"Unsupported constant type: {constantNode.Value.GetType().Name}");
            }
        }

        private static string ConvertBinaryOperatorToSql(BinaryOperatorNode binaryNode)
        {
            var left = ConvertExpressionToSql(binaryNode.Left);
            var right = ConvertExpressionToSql(binaryNode.Right);
            switch (binaryNode.OperatorKind)
            {
                case BinaryOperatorKind.Equal:
                    return $"{left} = {right}";
                case BinaryOperatorKind.NotEqual:
                    return $"{left} <> {right}";
                case BinaryOperatorKind.GreaterThan:
                    return $"{left} > {right}";
                case BinaryOperatorKind.GreaterThanOrEqual:
                    return $"{left} >= {right}";
                case BinaryOperatorKind.LessThan:
                    return $"{left} < {right}";
                case BinaryOperatorKind.LessThanOrEqual:
                    return $"{left} <= {right}";
                case BinaryOperatorKind.And:
                    return $"{left} AND {right}";
                case BinaryOperatorKind.Or:
                    return $"{left} OR {right}";
                default:
                    throw new NotSupportedException($"Unsupported binary operator: {binaryNode.OperatorKind}");
            }
        }

        private static string ConvertPropertyAccessToSql(SingleValuePropertyAccessNode propertyAccessNode)
        {
            // Convert property access to SQL column name
            return propertyAccessNode.Property.Name;
        }

        private static string ConvertFunctionCallToSql(SingleValueFunctionCallNode functionCallNode)
        {
            // Example for simple functions, extend for others as needed
            if (functionCallNode.Name == "startswith")
            {
                var arguments = functionCallNode.Parameters.Cast<ConstantNode>().ToArray();
                return $"LIKE '{arguments[0]}%'";
            }
            else
            {
                throw new NotSupportedException($"Unsupported function call: {functionCallNode.Name}");
            }
        }
    }
}
