
namespace LearnMVC.Models.DTOs
{
    public enum QueryOperator
    {
        Equal,
        NotEqual,
        GreaterThan,
        LessThan,
        GreaterOrEqual,
        LessOrEqual,
        Like,
        In,
        Between,
        IsNull,
        IsNotNull
    }

    public class QueryFilter
    {
        public string Column { get; set; }
        public QueryOperator Operator { get; set; }
        public object Value { get; set; }
        public object SecondValue { get; set; } // for BETWEEN
    }

    public class QuerySort
    {
        public string Column { get; set; }
        public bool Descending { get; set; }
    }


    public class QueryJoin
    {
        public string JoinType { get; set; } = "INNER"; // INNER, LEFT, RIGHT
        public string Table { get; set; }
        public string Alias { get; set; }
        public string OnCondition { get; set; }
    }

    public class QueryOptions
    {
        public string Table { get; set; }
        public string Alias { get; set; }

        public List<string> SelectColumns { get; set; } = new();
        public List<QueryFilter> Filters { get; set; } = new();
        public List<QuerySort> Sorts { get; set; } = new();
        public List<QueryJoin> Joins { get; set; } = new();

        public int? Limit { get; set; }
        public int? Offset { get; set; }
    }

}
