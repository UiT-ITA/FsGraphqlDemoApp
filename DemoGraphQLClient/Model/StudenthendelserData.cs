using FsGraphqlDemo.GraphQL.Client.Model.StudenData;
using System.Text.Json.Serialization;

namespace FsGraphqlDemo.GraphQL.Client.Model.StudenthendelserData;

public class StudenthendelserData
{
    [JsonPropertyName("studenthendelser")]
    public required Studenthendelser Studenthendelser { get; set; }
}

public class Studenthendelser
{
    public required Edge[] Edges { get; set; }
    public required Pageinfo PageInfo { get; set; }
    public required int TotalCount { get; set; }
}

public class Pageinfo
{
    public required bool HasPreviousPage { get; set; }
    public required string StartCursor { get; set; }
    public required string EndCursor { get; set; }
    public required bool HasNextPage { get; set; }
}

public class Edge
{
    public required string Cursor { get; set; }
    public required EmneNode Node { get; set; }
}

public class EmneNode
{
    public required DateTime Tidspunkt { get; set; }
    public required string Hendelsestype { get; set; }
    public required string Id { get; set; }
    public required StudentData Student { get; set; }
}
