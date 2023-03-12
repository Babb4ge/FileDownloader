using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDownloader;

    internal struct DownloadChunk
    {
        public long End;
        public long Start;
    public byte[] Data;
        public long Size => End - Start;
    }

