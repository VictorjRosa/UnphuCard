﻿using Azure.Storage.Blobs;
using System.IO;
using System.Threading.Tasks;

public class BlobStorageService
{
    private readonly string connectionString = "";
    private readonly string containerName = ""; // El nombre de tu contenedor

    public async Task UploadFileAsync(string filePath, string fileName)
    {
        // Crear un cliente para la cuenta de almacenamiento
        var blobServiceClient = new BlobServiceClient(connectionString);

        // Obtener el contenedor
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

        // Crear un cliente para el archivo que quieres subir
        var blobClient = blobContainerClient.GetBlobClient(fileName);

        // Subir el archivo al Blob Storage
        using (var fileStream = File.OpenRead(filePath))
        {
            await blobClient.UploadAsync(fileStream, overwrite: true);
        }
    }

    public string GetBlobUrl(string fileName)
    {
        var blobServiceClient = new BlobServiceClient(connectionString);
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = blobContainerClient.GetBlobClient(fileName);
        return blobClient.Uri.ToString();
    }
}
