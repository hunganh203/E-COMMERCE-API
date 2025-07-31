using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class FileStorage
{
    public int Id { get; set; }

    [MaxLength(255)]
    public string? FileName { get; set; }

    [MaxLength(255)]
    public string? FileUniqueName { get; set; }

    public long? Size { get; set; }

    [MaxLength(255)]
    public string? Type { get; set; }

    [MaxLength(255)]
    public string? Path { get; set; }

    [MaxLength(255)]
    public string? Extension { get; set; }

    [MaxLength(255)]
    public string? Module { get; set; }

    [MaxLength(255)]
    public string? DocumentType { get; set; }

    public static FileStorage Create(string? fileName, string? fileUniqueName, long? size,
        string? type, string? path, string? extension, string? module, string? documentType)
    {
        return new FileStorage
        {
            FileName = fileName,
            FileUniqueName = fileUniqueName,
            Size = size,
            Type = type,
            Path = path,
            Extension = extension,
            Module = module,
            DocumentType = documentType
        };
    }

    public void Update(string? fileName, string? fileUniqueName, long? size, string? type,
        string? path, string? extension, string? module, string? documentType)
    {
        FileName = fileName;
        FileUniqueName = fileUniqueName;
        Size = size;
        Type = type;
        Path = path;
        Extension = extension;
        Module = module;
        DocumentType = documentType;
    }
}