using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EEditor
{
    internal class EBELVLReader
    {
        public static EBEDataChunk[] Parse(byte[] bytes, int position)
        {
            int i = position;
            var chunks = new List<EBEDataChunk>();
            while (i < bytes.Length)
            {
                var args = new List<object>();
                var type = BitConverter.ToUInt32(bytes, i, true); i += 4;
                var layerNum = BitConverter.ToInt32(bytes, i, true); i += 4;

                var xsLength = BitConverter.ToUInt32(bytes, i, true); i += 4;
                var xs = new byte[xsLength];
                for (int x = 0; x < xsLength; x++)
                {
                    xs[x] = bytes[i++];
                }

                var ysLength = BitConverter.ToUInt32(bytes, i, true); i += 4;
                var ys = new byte[ysLength];
                for (int y = 0; y < ysLength; y++)
                {
                    ys[y] = bytes[i++];
                }

                if (goalNew.Contains((int)type))
                {
                    args.Add(BitConverter.ToUInt32(bytes, i, true)); //goal
                    i += 4;
                }
                if (rotationNew.Contains((int)type))
                {
                    args.Add(BitConverter.ToUInt32(bytes, i, true)); //rotation
                    i += 4;
                }
                if (type == 381 || type == 242) //Portals
                {
                    args.Add(BitConverter.ToUInt32(bytes, i, true)); //rotation
                    args.Add(BitConverter.ToUInt32(bytes, i + 4, true)); //id
                    args.Add(BitConverter.ToUInt32(bytes, i + 8, true)); //target
                    i += 12;
                }
                if (type == 374) //World Portal
                {
                    var targetLength = BitConverter.ToInt32(bytes, i, true);
                    args.Add(Encoding.UTF8.GetString(bytes, i + 4, targetLength)); //worldID
                    args.Add(BitConverter.ToUInt32(bytes, i + 4 + targetLength, true)); //spawnpoint ID
                    i += (8 + targetLength);
                }
                if (type == 1582) //World portal spawn point
                {
                    args.Add(BitConverter.ToUInt32(bytes, i, true)); //spawnpoint id
                    i += 4;
                }
                if (coloredBlocks.Contains((int)type) || type == 1200) //Coloured blocks
                {
                    args.Add(BitConverter.ToUInt32(bytes, i, true)); //colour
                    i += 4;
                }
                if (type == 1000) //Label
                {
                    var textLength = BitConverter.ToInt32(bytes, i, true);
                    args.Add(Encoding.UTF8.GetString(bytes, i + 4, textLength)); //text

                    var textColourLength = BitConverter.ToInt32(bytes, i + 4 + textLength, true);
                    args.Add(Encoding.UTF8.GetString(bytes, i + 8 + textLength, textColourLength)); //colour

                    args.Add(BitConverter.ToUInt32(bytes, i + 8 + textLength + textColourLength, true)); //wrap

                    i += (12 + textLength + textColourLength);
                }
                if (type == 77 || type == 83 || type == 1520) //Music blocks
                {
                    args.Add(BitConverter.ToUInt32(bytes, i, true)); //note id
                    i += 4;
                }
                if (type == 385) //Sign blocks
                {
                    var textLength = BitConverter.ToInt32(bytes, i, true);
                    args.Add(Encoding.UTF8.GetString(bytes, i + 4, textLength)); //text
                    args.Add(BitConverter.ToUInt32(bytes, i + 4 + textLength, true)); //sign type
                    i += (8 + textLength);
                }
                if (isNPC((int)type)) //Npc blocks
                {
                    var nameLength = BitConverter.ToInt32(bytes, i, true);
                    args.Add(Encoding.UTF8.GetString(bytes, i + 4, nameLength)); //npc name

                    var msg1Length = BitConverter.ToInt32(bytes, i + 4 + nameLength, true);
                    args.Add(Encoding.UTF8.GetString(bytes, i + 8 + nameLength, msg1Length)); //message 1

                    var msg2Length = BitConverter.ToInt32(bytes, i + 8 + nameLength + msg1Length, true);
                    args.Add(Encoding.UTF8.GetString(bytes, i + 12 + nameLength + msg1Length, msg2Length)); //message 2

                    var msg3Length = BitConverter.ToInt32(bytes, i + 12 + nameLength + msg1Length + msg2Length, true);
                    args.Add(Encoding.UTF8.GetString(bytes, i + 16 + nameLength + msg1Length + msg2Length, msg3Length)); //message 3

                    i += (16 + nameLength + msg1Length + msg2Length + msg3Length);

                }
                chunks.Add(new EBEDataChunk(layerNum, type, xs, ys, args.ToArray()));
            }
            return chunks.ToArray();
        }
        public static bool isNPC(int id)
        {
            if (id >= 1550 && id <= 1559 || id >= 1569 && id <= 1579) return true;
            else return false;
        }
        public static int[] coloredBlocks =
{
            631, 632, 633
        };
        public static int[] goalNew =
{
            //Coin door/gates
            43, 165, 213, 214,

            //Death door/gate
            1011, 1012,

            //Purple switches
            184, 185, 113, 1619,

            //orange switches
            1079, 1080, 467, 1620,
                    

            //Team Effects 
            423, 1027, 1028,

            /*Effects: Curse, Zombie, Poison, Fly, 
            Protection, Lowgravity, Jump, Speed, Multijump*/
            421, 422, 1584, 418, 420, 453, 417, 419, 461

        };

        public static int[] rotationNew =
        {
            //Glowy Lines
            376, 375, 379, 380, 377, 378, 438, 439,


            //OneWay
            1001,1002, 1003,1004, 1052,
            1053, 1054, 1055, 1056, 1092,


            //Spikes
            361, 1625, 1627, 1629, 1631, 1633, 1635,

            //## Morph

            //Medival
            275, 327, 328, 273, 329,440,

            //Monster
            338, 339, 340,

            //Domestic
            447, 448, 449, 450, 451, 452, 1537, 1538,

            //Halloween 2015
            456, 457, 458,

            //Fairytale
            471,

            //Spring
            475, 476, 477,

            //Summer
            483, 481, 482,

            //Restaurant
            494, 492, 493,

            //Halloween 2016
            1502, 499, 1500,

            //Toxic
            1587,

            //Sewer
            1588, 1155,

            //dungeon
            1592, 1593, 1160, 1594, 1595, 1597,


            //New year 2015
            464, 465,

            //Cave 
            497, 

            //Christmas 2016
            1506, 1507,

            //Dojo
            276, 277, 279, 280,

            //Industrial
            1134, 1135, 1535, 1536,

            //Fireworks
            1581,

            //Shadow
            1596, 1605, 1606, 1607, 1609, 1610,
            1611, 1612, 1614, 1615, 1616, 1617,

            //## Half Blocks

            //Domestic
            1042, 1043, 1041,

            //Fairytale
            1075, 1076, 1077, 1078,

            //Basic
            1116,1117, 1118, 1119, 1120, 1121,
            1122, 1123, 1124, 1125,

            //Winter 2018
            1140, 1141,

            //## Effects

            //Gravity 
            1517,
        };
    }
    public class EBEDataChunk
    {
        public int Layer { get; set; }
        public uint Type { get; set; }
        public Point[] Locations { get; set; }
        public object[] Args { get; set; }

        public EBEDataChunk(int layer, uint type, byte[] xs, byte[] ys, object[] args)
        {
            Layer = layer;
            Type = type;
            Args = args;
            Locations = GetLocations(xs, ys);
        }

        private static Point[] GetLocations(byte[] xs, byte[] ys)
        {
            var points = new List<Point>();
            for (var i = 0; i < xs.Length; i += 2) points.Add(new Point((xs[i] << 8) | xs[i + 1], (ys[i] << 8) | ys[i + 1]));
            return points.ToArray();

        }

        public class Point
        {
            public int X { get; set; }
            public int Y { get; set; }

            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

    }
}
