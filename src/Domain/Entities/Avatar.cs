using System;

namespace InfraStellar.Domain.Entities;

public class Avatar
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public byte[] Dados { get; set; } = Array.Empty<byte>();
    public string ContentType { get; set; } = "image/png";
}
