using System;
using System.Globalization;
using Microsoft.AspNetCore.Http;

namespace Securex.BL.Extension;
public static class FileExtension
{
	public static bool IsValidType(this IFormFile file, string type)
		=> file.ContentType.StartsWith(type);

    public static bool IsValidSize(this IFormFile file, int mb)
        => file.Length <= mb * 1024 * 1024;

    public static async Task<string> UploadAsync(this IFormFile file, params string[] paths)
    {
        string path = Path.Combine(paths);

        if (System.IO.File.Exists(path))
            Directory.CreateDirectory(path);

        string fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
        using (Stream sr = File.Create(Path.Combine(path, fileName)))
            await file.CopyToAsync(sr);

        return fileName; 
    }
}

