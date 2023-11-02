using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;
using System.Xml.Linq;

namespace EEditor
{
    public class EBELVL
    {
        public string WorldName { get; set; }
        public string Description { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public string OwnerName { get; set; }
        public string OwnerID { get; set; }

        public string CrewName { get; set; }
        public string CrewID { get; set; }
        public int CrewStatus { get; set; }

        public int BackgroundColor { get; set; }
        public float Gravity { get; set; }
        public bool Minimap { get; set; }
        public bool Campaign { get; set; }
        public int Version { get; set; }

        public readonly EBEBlock[,,] blocks;

        public EBEBlock this[int l, int x, int y]
        {
            get => blocks[l, x, y];
            set => blocks[l, x, y] = value.id == 0 ? null : value;
        }

        public static int Position { get; set; }

        public EBELVL(
            int version,
            string ownerName,
            string worldName,
            int width,
            int height,
            int backgroundColor,
            string description,
            string crewID,
            string crewName,
            int crewStatus,
            string ownerID
        )
        {
            Version = version;
            OwnerName = ownerName;
            WorldName = worldName;
            Width = width;
            Height = height;
            BackgroundColor = backgroundColor;
            Description = description;
            CrewID = crewID;
            CrewName = crewName;
            CrewStatus = crewStatus;
            OwnerID = ownerID;
            blocks = new EBEBlock[2, Width, Height];
        }
        public EBELVL(int width, int height, int borderID = 9) : this(0, "", "Untitled World", width, height, 1, "", "", "", 1, "")
        {
            EBEBlock border = new EBEBlock(borderID);
            for (int x = 0; x < Width; x++)
            {
                blocks[0, x, 0] = border;
                blocks[0, x, Height - 1] = border;
            }
            for (int y = 1; y < Height - 1; y++)
            {
                blocks[0, 0, y] = border;
                blocks[0, Width - 1, y] = border;
            }
        }
        public static EBELVL Open(string file)
        {
            using (FileStream compressedFile = File.OpenRead(file))
            {

                using (System.IO.Compression.DeflateStream deflateStream = new System.IO.Compression.DeflateStream(compressedFile, CompressionMode.Decompress))
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {

                        byte[] buffer = new byte[256]; //4096 You can adjust the buffer size as needed.

                        int bytesRead;
                        while ((bytesRead = deflateStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            memoryStream.Write(buffer, 0, bytesRead);
                        }

                        byte[] bytes = memoryStream.ToArray();
                        EBELVL level = MetaData(bytes);
                        level.MetaBlocks(bytes, Position);
                        return level;
                    }
                }
            }
        }
        public void MetaBlocks(byte[] bytes,int position)
        {
            EBEDataChunk[] chunks = EBELVLParser.Parse(bytes, position);
            foreach (var c in chunks)
            {
                foreach (var pos in c.Locations)
                    blocks[c.Layer, pos.X, pos.Y] = new EBEBlock(Convert.ToInt32(c.Type), c.Args);
            }

        }
        public static EBELVL MetaData(byte[] meta)
        {
            int i = 0;

            var length = (short)0;
            var version = BitConverter.ToInt32(meta, i, false); i += 4;
            length = BitConverter.ToInt16(meta, i, false); i += 2;
            var owner = Encoding.UTF8.GetString(meta, i, length); i += length;
            length = BitConverter.ToInt16(meta, i, false); i += 2;
            var name = Encoding.UTF8.GetString(meta, i, length); i += length;
            var width = BitConverter.ToInt32(meta, i, false); i += 4;
            var height = BitConverter.ToInt32(meta, i, false); i += 4;
            //var gravity = BitConverter.ToDouble(bytes, i, bigEndian); i += 8;
            i += 4;
            var bg = BitConverter.ToInt32(meta, i, false); i += 4;
            length = BitConverter.ToInt16(meta, i, false); i += 2;
            var desc = Encoding.UTF8.GetString(meta, i, length); i += length;
            //var camp = BitConverter.ToBoolean(bytes, i, bigEndian); i += 2;
            i++;
            length = BitConverter.ToInt16(meta, i, false); i += 2;
            var crew = Encoding.UTF8.GetString(meta, i, length); i += length;
            length = BitConverter.ToInt16(meta, i, false); i += 2;
            var crewName = Encoding.UTF8.GetString(meta, i, length); i += length;
            var crewStatus = BitConverter.ToInt32(meta, i, false); i += 4;
            //var minimap = BitConverter.ToBoolean(bytes, i, bigEndian); i += 2;
            i++;
            length = BitConverter.ToInt16(meta, i, false); i += 2;
            var ownerId = Encoding.UTF8.GetString(meta, i, length); i += length;
            i += 4;
            Position = i;

            return new EBELVL(
                version,
                owner,
               name,
                width,
                height,
                bg,
                desc,
                crew,
                crewName,
                crewStatus,
                ownerId
            ); 
        }
    }
    public class EBEBlock
    {
        public int id { get; set; }

        public int x, y, layer;
        public object[] args { get; set; }
        public EBEBlock(int blockID, params object[] args)
        {
            this.id = blockID;
            this.args = args;
        }
    }
}
