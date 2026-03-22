namespace OpsT360.Models;

public sealed class ContainerProfile
{
    public int EntityId { get; init; }
    public string Size { get; init; } = "40HC";
    public string ShippingLine { get; init; } = "MSC";
    public string OriginPort { get; init; } = "ECGYE";
    public string LoadingPort { get; init; } = "ECGYE";
    public string DischargePort { get; init; } = "USMIA";
    public string DestinationPort { get; init; } = "USMIA";
    public string Goods { get; init; } = "Carga general";
    public string Booking { get; init; } = "BK2025-009999";
    public int OriginWeight { get; init; } = 20000;
    public string TerminalEntryDate { get; init; } = "2025-01-01T00:00:00";
    public string PreNoticeDate { get; init; } = "2025-01-01T00:00:00";
    public string Carrier { get; init; } = "TRANSPORTISTA GENERICO";
    public string Plate { get; init; } = "ABC999";
    public string Driver { get; init; } = "Conductor";
    public string CustomsSeal { get; init; } = "SAP999";
    public string Observations { get; init; } = "Datos de respaldo del simulador";
}
