﻿using LionFire.Persistence;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionFire.Persistence.FilesystemFacade
{

    public interface ISimpleFilesystemFacade
    {
        Task<bool> Exists(string path);

        Task<ITransferResult> Delete(string path);

        Task<IEnumerable<string>> List(string directoryPath, string pattern = null);
        Task<byte[]> ReadAllBytes(string path);
        Task<string> ReadAllText(string path);

        Task WriteAllBytes(string path, byte[] data);
        Task WriteAllText(string path, string data);
    }
}
