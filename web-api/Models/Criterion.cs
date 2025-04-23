using PointOfInterest;
using Utilities;

namespace Criteria;

public class CriteriaModel
{
    public List<Criterion> Criteria { get; set; }
}

public class Criterion
{
    public int Id { get; set; }
    public Category Category { get; set; }
    public string CategoryName => Category.GetDescription();
    public decimal Tolerance { get; set; }
    public Unit Unit { get; set; }
    public Direction Direction { get; set; }
    public override string ToString() => $"Id: {Id}, Category: {Category}, Tol: {Tolerance}";
}

public enum Unit
{
    Miles,
    Kilometers,
}

public enum Direction
{
    CloseTo,
    FarFrom,
}