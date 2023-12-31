﻿using System;
using System.Collections.Generic;
using System.Linq;
using PlayerIOClient;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Drawing;
using System.IO;
using Newtonsoft.Json.Linq;
using EELVL;
using static System.Windows.Forms.MonthCalendar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using System.Text;
using System.Linq.Expressions;
using System.IO.Compression;
using SharpCompress.Compressors.Deflate;
using SharpCompress.Compressors.Deflate64;
using System.Xml;
using static EELVL.Blocks;
using static World;

namespace EEditor
{
    public class Frame
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public int[,] Foreground { get; set; }
        public int[,] Background { get; set; }
        public int[,] BlockData { get; set; }
        public int[,] BlockData1 { get; set; }
        public int[,] BlockData2 { get; set; }
        public string[,] BlockData3 { get; set; }
        public string[,] BlockData4 { get; set; }
        public string[,] BlockData5 { get; set; }
        public string[,] BlockData6 { get; set; }

        public uint[,] BlockData7 { get; set; }
        public string nickname { get; set; }
        public string owner { get; set; }
        public string levelname { get; set; }
        public bool toobig { get; set; }
        public uint backgroundColor { get; set; }
        public static byte[] xx;
        public static byte[] yy;
        public static byte[] xx1;
        public static byte[] yy1;
        public static string[] split1;


        public Frame(int width, int height)
        {
            Width = width;
            Height = height;
            Foreground = new int[Height, Width];
            Background = new int[Height, Width];
            BlockData = new int[Height, Width];
            BlockData1 = new int[Height, Width];
            BlockData2 = new int[Height, Width];
            BlockData7 = new uint[Height, Width];
            BlockData3 = new string[height, width];
            BlockData4 = new string[height, width];
            BlockData5 = new string[height, width];
            BlockData6 = new string[height, width];
            nickname = null;
            owner = null;
            levelname = null;
        }

        public event EventHandler<StatusChangedArgs> StatusChanged;

        protected void OnStatusChanged(string msg, DateTime time, bool done = false, int totallines = 0, int countlines = 0)
        {
            StatusChanged?.Invoke(this, new StatusChangedArgs(msg, time, done, totallines, countlines));
        }

        public void Reset(bool frame)
        {
            for (int i = 0; i < Height; ++i)
            {
                for (int j = 0; j < Width; ++j)
                {
                    if (i == 0 || j == 0 || i == Height - 1 || j == Width - 1)
                    {
                        if (Width == 110 && Height == 110)
                        {
                            if (!frame) { Foreground[i, j] = 182; }
                            else { Foreground[i, j] = -1; }
                        }
                        else
                        {
                            if (!frame) { Foreground[i, j] = 9; }
                            else { Foreground[i, j] = -1; }
                        }
                    }
                    else
                    {
                        if (!frame) { Foreground[i, j] = 0; }
                        else { Foreground[i, j] = -1; }
                    }
                }
            }
        }

        public static Frame FromMessage2(DatabaseObject dbo)
        {
            var w = 0;
            var h = 0;
            if (dbo.Contains("width") && dbo.Contains("height"))
            {
                return FromMessage1(dbo, dbo.GetInt("width"), dbo.GetInt("height"));
            }
            else
            {
                if (dbo.Contains("type"))
                {
                    switch ((int)dbo["type"])
                    {
                        case 1:
                            w = 50;
                            h = 50;
                            break;
                        case 2:
                            w = 100;
                            h = 100;
                            break;
                        default:
                        case 3:
                            w = 200;
                            h = 200;
                            break;
                        case 4:
                            w = 400;
                            h = 50;
                            break;
                        case 5:
                            w = 400;
                            h = 200;
                            break;
                        case 6:
                            w = 100;
                            h = 400;
                            break;
                        case 7:
                            w = 636;
                            h = 50;
                            break;
                        case 8:
                            w = 110;
                            h = 110;
                            break;
                        case 11:
                            w = 300;
                            h = 300;
                            break;
                        case 12:
                            w = 250;
                            h = 150;
                            break;
                    }
                    return FromMessage1(dbo, w, h);
                }
                else
                {
                    return FromMessage1(dbo, 200, 200);
                }
            }
        }

        public static Frame FromMessage(PlayerIOClient.Message e)
        {
            if (bdata.ParamNumbers(e, 25, "System.Int32") && bdata.ParamNumbers(e, 26, "System.Int32"))
            {
                return FromMessage(e, e.GetInt(25), e.GetInt(26));
            }
            else
            {
                return null;
            }
        }

        #region read from init


        public static Frame FromMessage(PlayerIOClient.Message e, int width, int height)
        {
            Frame frame;
            int width1 = width;
            int height1 = height;
            frame = new Frame(width, height);
            //var chunks = InitParse.Parse(e);


            if (MainForm.userdata.level.StartsWith("OW"))
            {
                if (e.GetBoolean(21)) { MainForm.OpenWorld = true; MainForm.OpenWorldCode = false; }
                else if (!e.GetBoolean(21)) { MainForm.OpenWorld = true; MainForm.OpenWorldCode = true; }
            }
            var bytes = Decompress.Decompression(e.GetByteArray(49));
            for (var i = 0; i < bytes.Length;)
            {
                var type = BitConverter.ToUInt32(bytes, i, true); i += 4;
                var layerNum = BitConverter.ToInt32(bytes, i, true); i += 4;

                var rotation = 0u;
                var datta = 0u;
                var target = 0u;
                var id = 0u;
                var colour = 0u;
                var signType = 0u;
                var wrapLength = 0u;
                var targetP = "";
                var text = "";
                var textColour = "";
                var name = "";
                var message1 = "";
                var message2 = "";
                var message3 = "";


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
                if (bdata.goalNew.Contains((int)type) || bdata.rotationNew.Contains((int)type))

                {
                    datta = BitConverter.ToUInt32(bytes, i, true);
                    i += 4;
                }
                if (type == 381 || type == 242)
                {
                    rotation = BitConverter.ToUInt32(bytes, i, true);
                    id = BitConverter.ToUInt32(bytes, i + 4, true);
                    target = BitConverter.ToUInt32(bytes, i + 8, true);
                    i += 12;
                }
                if (type == 374)
                {
                    var targetLength = BitConverter.ToInt32(bytes, i, true);
                    targetP = Encoding.UTF8.GetString(bytes, i + 4, targetLength);
                    id = BitConverter.ToUInt32(bytes, i + 4 + targetLength, true);
                    i += (8 + targetLength);
                }
                if (type == 1582)
                {
                    datta = BitConverter.ToUInt32(bytes, i, true);
                    i += 4;
                }
                if (bdata.coloredBlocks.Contains((int)type))
                {
                    colour = BitConverter.ToUInt32(bytes, i, true);
                    i += 4;
                }
                if ((int)type == 1645)
                {
                    colour = BitConverter.ToUInt32(bytes, i, true);
                    rotation = BitConverter.ToUInt32(bytes, i + 4, true);
                    i += 8;
                }
                if (type == 1000)
                {
                    var textLength = BitConverter.ToInt32(bytes, i, true);
                    text = Encoding.UTF8.GetString(bytes, i + 4, textLength);

                    var textColorLength = BitConverter.ToInt32(bytes, i + 4 + textLength, true);
                    textColour = Encoding.UTF8.GetString(bytes, i + 8 + textLength, textColorLength);

                    wrapLength = BitConverter.ToUInt32(bytes, i + 8 + textLength + textColorLength, true);

                    i += (12 + textLength + textColorLength);
                }
                if (type == 77 || type == 83 || type == 1520)
                {
                    id = BitConverter.ToUInt32(bytes, i, true);
                    i += 4;
                }
                if (type == 385)
                {
                    var textLength = BitConverter.ToInt32(bytes, i, true);
                    text = Encoding.UTF8.GetString(bytes, i + 4, textLength);
                    signType = BitConverter.ToUInt32(bytes, i + 4 + textLength, true);
                    i += (8 + textLength);
                }
                if (bdata.isNPC((int)type))
                {
                    var nameLength = BitConverter.ToInt32(bytes, i, true);
                    name = Encoding.UTF8.GetString(bytes, i + 4, nameLength);

                    var msg1Length = BitConverter.ToInt32(bytes, i + 4 + nameLength, true);
                    message1 = Encoding.UTF8.GetString(bytes, i + 8 + nameLength, msg1Length);

                    var msg2Length = BitConverter.ToInt32(bytes, i + 8 + nameLength + msg1Length, true);
                    message2 = Encoding.UTF8.GetString(bytes, i + 12 + nameLength + msg1Length, msg2Length);

                    var msg3Length = BitConverter.ToInt32(bytes, i + 12 + nameLength + msg1Length + msg2Length, true);
                    message3 = Encoding.UTF8.GetString(bytes, i + 16 + nameLength + msg1Length + msg2Length, msg3Length);
                    i += (16 + nameLength + msg1Length + msg2Length + msg3Length);
                }
                /*switch (type)
                {
                    case (uint)ItemTypes.CoinDoor:
                    case (uint)ItemTypes.CoinGate:
                    case (uint)ItemTypes.BlueCoinDoor:
                    case (uint)ItemTypes.BlueCoinGate:
                    case (uint)ItemTypes.DeathDoor:
                    case (uint)ItemTypes.DeathGate:
                    case (uint)ItemTypes.DoorPurple:
                    case (uint)ItemTypes.GatePurple:
                    case (uint)ItemTypes.SwitchPurple:
                    case (uint)ItemTypes.ResetPurple:
                    case (uint)ItemTypes.DoorOrange:
                    case (uint)ItemTypes.GateOrange:
                    case (uint)ItemTypes.SwitchOrange:
                    case (uint)ItemTypes.ResetOrange:
                    case (uint)ItemTypes.EffectTeam:
                    case (uint)ItemTypes.TeamDoor:
                    case (uint)ItemTypes.TeamGate:
                    case (uint)ItemTypes.EffectCurse:
                    case (uint)ItemTypes.EffectZombie:
                    case (uint)ItemTypes.EffectPoison:
                    case (uint)ItemTypes.EffectFly:
                    case (uint)ItemTypes.EffectProtection:
                    case (uint)ItemTypes.EffectLowGravity:
                    case (uint)ItemTypes.EffectJump:
                    case (uint)ItemTypes.EffectSpeed:
                    case (uint)ItemTypes.EffectMultijump:
                        goal = BitConverter.ToUInt32(bytes, i);
                        i += 4;
                        break;

                    case (uint)ItemTypes.PortalInvisible:
                    case (uint)ItemTypes.Portal:
                        rotation = BitConverter.ToUInt32(bytes, i);
                        id = BitConverter.ToUInt32(bytes, i + 4);
                        target = BitConverter.ToUInt32(bytes, i + 8);
                        i += 12;
                        break;

                    case (uint)ItemTypes.WorldPortal:
                        var targetLength = BitConverter.ToInt32(bytes, i);
                        targetP = Encoding.UTF8.GetString(bytes, i + 4, targetLength);
                        id = BitConverter.ToUInt32(bytes, i + 4 + targetLength);
                        i += (8 + targetLength);
                        break;

                    case (uint)ItemTypes.WorldPortalSpawn:
                        id = BitConverter.ToUInt32(bytes, i);
                        i += 4;
                        break;

                    case (uint)ItemTypes.ColourWheelPlainBG:
                    case (uint)ItemTypes.ColourWheelCanvasBG:
                    case (uint)ItemTypes.ColourWheelCheckerBG:
                    case (uint)ItemTypes.ColourWheelCaveBG:
                    case (uint)ItemTypes.ColourWheelTileBG:
                    case (uint)ItemTypes.ColourWheelBasic:
                        colour = BitConverter.ToUInt32(bytes, i);
                        i += 4;
                        break;

                    case (uint)ItemTypes.GlowyLineBlueStraight:
                    case (uint)ItemTypes.GlowyLineBlueSlope:
                    case (uint)ItemTypes.GlowyLineGreenSlope:
                    case (uint)ItemTypes.GlowyLineGreenStraight:
                    case (uint)ItemTypes.GlowyLineYellowSlope:
                    case (uint)ItemTypes.GlowyLineYellowStraight:
                    case (uint)ItemTypes.GlowyLineRedSlope:
                    case (uint)ItemTypes.GlowyLineRedStraight:
                    case (uint)ItemTypes.OnewayCyan:
                    case (uint)ItemTypes.OnewayOrange:
                    case (uint)ItemTypes.OnewayYellow:
                    case (uint)ItemTypes.OnewayPink:
                    case (uint)ItemTypes.OnewayGray:
                    case (uint)ItemTypes.OnewayBlue:
                    case (uint)ItemTypes.OnewayRed:
                    case (uint)ItemTypes.OnewayGreen:
                    case (uint)ItemTypes.OnewayBlack:
                    case (uint)ItemTypes.OnewayWhite:
                    case (uint)ItemTypes.Spike:
                    case (uint)ItemTypes.SpikeSilver:
                    case (uint)ItemTypes.SpikeBlack:
                    case (uint)ItemTypes.SpikeRed:
                    case (uint)ItemTypes.SpikeGold:
                    case (uint)ItemTypes.SpikeGreen:
                    case (uint)ItemTypes.SpikeBlue:
                    case (uint)ItemTypes.MedievalAxe:
                    case (uint)ItemTypes.MedievalBanner:
                    case (uint)ItemTypes.MedievalCoatOfArms:
                    case (uint)ItemTypes.MedievalShield:
                    case (uint)ItemTypes.MedievalSword:
                    case (uint)ItemTypes.ToothBig:
                    case (uint)ItemTypes.ToothSmall:
                    case (uint)ItemTypes.ToothTriple:
                    case (uint)ItemTypes.DomesticLightBulb:
                    case (uint)ItemTypes.DomesticTap:
                    case (uint)ItemTypes.DomesticPainting:
                    case (uint)ItemTypes.DomesticVase:
                    case (uint)ItemTypes.DomesticTv:
                    case (uint)ItemTypes.DomesticWindow:
                    case (uint)ItemTypes.HalfBlockDomesticBrown:
                    case (uint)ItemTypes.HalfBlockDomesticWhite:
                    case (uint)ItemTypes.HalfBlockDomesticYellow:
                    case (uint)ItemTypes.Halloween2015WindowRect:
                    case (uint)ItemTypes.Halloween2015WindowCircle:
                    case (uint)ItemTypes.Halloween2015Lamp:
                    case (uint)ItemTypes.FairytaleFlowers:
                    case (uint)ItemTypes.HalfBlockFairytaleOrange:
                    case (uint)ItemTypes.HalfBlockFairytaleGreen:
                    case (uint)ItemTypes.HalfBlockFairytaleBlue:
                    case (uint)ItemTypes.HalfBlockFairytalePink:
                    case (uint)ItemTypes.HalfBlockWhite:
                    case (uint)ItemTypes.HalfBlockGray:
                    case (uint)ItemTypes.HalfBlockBlack:
                    case (uint)ItemTypes.HalfBlockRed:
                    case (uint)ItemTypes.HalfBlockOrange:
                    case (uint)ItemTypes.HalfBlockYellow:
                    case (uint)ItemTypes.HalfBlockGreen:
                    case (uint)ItemTypes.HalfBlockCyan:
                    case (uint)ItemTypes.HalfBlockBlue:
                    case (uint)ItemTypes.HalfBlockPurple:
                    case (uint)ItemTypes.SpringDaisy:
                    case (uint)ItemTypes.SpringTulip:
                    case (uint)ItemTypes.SpringDaffodil:
                    case (uint)ItemTypes.SummerIceCream:
                    case (uint)ItemTypes.RestaurantBowl:
                    case (uint)ItemTypes.RestaurantCup:
                    case (uint)ItemTypes.RestaurantPlate:
                    case (uint)ItemTypes.Halloween2016Eyes:
                    case (uint)ItemTypes.Halloween2016Rotatable:
                    case (uint)ItemTypes.Halloween2016Pumpkin:
                    case (uint)ItemTypes.EffectGravity:
                    case (uint)ItemTypes.HalfBlockWinter2018Snow:
                    case (uint)ItemTypes.HalfBlockWinter2018Glacier:
                    case (uint)ItemTypes.ToxicWasteBarrel:
                    case (uint)ItemTypes.SewerPipe:
                    case (uint)ItemTypes.MetalPlatform:
                    case (uint)ItemTypes.DungeonPillarBottom:
                    case (uint)ItemTypes.DungeonPillarMiddle:
                    case (uint)ItemTypes.DungeonPillarTop:
                    case (uint)ItemTypes.DungeonArchLeft:
                    case (uint)ItemTypes.DungeonArchRight:
                    case (uint)ItemTypes.DungeonTorch:
                    case (uint)ItemTypes.NewYear2015Balloon:
                    case (uint)ItemTypes.NewYear2015Streamer:
                    case (uint)ItemTypes.Christmas2016LightsDown:
                    case (uint)ItemTypes.Christmas2016LightsUp:
                    case (uint)ItemTypes.MedievalTimber:
                    case (uint)ItemTypes.SummerFlag:
                    case (uint)ItemTypes.SummerAwning:
                    case (uint)ItemTypes.CaveCrystal:
                    case (uint)ItemTypes.DojoLightLeft:
                    case (uint)ItemTypes.DojoLightRight:
                    case (uint)ItemTypes.DojoDarkLeft:
                    case (uint)ItemTypes.DojoDarkRight:
                    case (uint)ItemTypes.IndustrialTable:
                    case (uint)ItemTypes.IndustrialPipeThick:
                    case (uint)ItemTypes.IndustrialPipeThin:
                    case (uint)ItemTypes.DomesticPipeStraight:
                    case (uint)ItemTypes.ShadowC:
                    case (uint)ItemTypes.ShadowH:
                    case (uint)ItemTypes.DomesticPipeT:
                    case (uint)ItemTypes.ShadowA:
                    case (uint)ItemTypes.ShadowB:
                    case (uint)ItemTypes.ShadowD:
                    case (uint)ItemTypes.ShadowF:
                    case (uint)ItemTypes.ShadowG:
                    case (uint)ItemTypes.ShadowI:
                    case (uint)ItemTypes.ShadowK:
                    case (uint)ItemTypes.ShadowL:
                    case (uint)ItemTypes.ShadowM:
                    case (uint)ItemTypes.ShadowN:
                    case (uint)ItemTypes.DomesticFrameBorder:
                    case (uint)ItemTypes.Fireworks:
                        rotation = BitConverter.ToUInt32(bytes, i);
                        i += 4;
                        break;

                    case (int)ItemTypes.Label:
                        {
                            var textLength = BitConverter.ToInt32(bytes, i);
                            text = Encoding.UTF8.GetString(bytes, i + 4, textLength);

                            var textColorLength = BitConverter.ToInt32(bytes, i + 4 + textLength);
                            textColour = Encoding.UTF8.GetString(bytes, i + 8 + textLength, textColorLength);

                            wrapLength = BitConverter.ToUInt32(bytes, i + 8 + textLength + textColorLength);

                            i += (12 + textLength + textColorLength);
                            break;
                        }

                    case (int)ItemTypes.Piano:
                    case (int)ItemTypes.Drums:
                    case (int)ItemTypes.Guitar:
                        id = BitConverter.ToUInt32(bytes, i);
                        i += 4;
                        break;
                    case (int)ItemTypes.TextSign:
                        {
                            var textLength = BitConverter.ToInt32(bytes, i);
                            text = Encoding.UTF8.GetString(bytes, i + 4, textLength);
                            signType = BitConverter.ToUInt32(bytes, i + 4 + textLength);
                            i += (8 + textLength);
                            break;
                        }
                    case (int)ItemTypes.NpcSmile:
                    case (int)ItemTypes.NpcSad:
                    case (int)ItemTypes.NpcOld:
                    case (int)ItemTypes.NpcAngry:
                    case (int)ItemTypes.NpcSlime:
                    case (int)ItemTypes.NpcRobot:
                    case (int)ItemTypes.NpcKnight:
                    case (int)ItemTypes.NpcMeh:
                    case (int)ItemTypes.NpcCow:
                    case (int)ItemTypes.NpcFrog:
                    case (int)ItemTypes.NpcBruce:
                    case (uint)ItemTypes.NpcStarfish:
                    case (uint)ItemTypes.NpcDT:
                    case (uint)ItemTypes.NpcSkeleton:
                    case (uint)ItemTypes.NpcZombie:
                    case (uint)ItemTypes.NpcGhost:
                    case (uint)ItemTypes.NpcAstronaut:
                    case (uint)ItemTypes.NpcSanta:
                    case (uint)ItemTypes.NpcSnowman:
                    case (uint)ItemTypes.NpcWalrus:
                    case (uint)ItemTypes.NpcCrab:
                        {
                            var nameLength = BitConverter.ToInt32(bytes, i);
                            name = Encoding.UTF8.GetString(bytes, i + 4, nameLength);

                            var msg1Length = BitConverter.ToInt32(bytes, i + 4 + nameLength);
                            message1 = Encoding.UTF8.GetString(bytes, i + 8 + nameLength, msg1Length);

                            var msg2Length = BitConverter.ToInt32(bytes, i + 8 + nameLength + msg1Length);
                            message2 = Encoding.UTF8.GetString(bytes, i + 12 + nameLength + msg1Length, msg2Length);

                            var msg3Length = BitConverter.ToInt32(bytes, i + 12 + nameLength + msg1Length + msg2Length);
                            message3 = Encoding.UTF8.GetString(bytes, i + 16 + nameLength + msg1Length + msg2Length, msg3Length);
                            i += (16 + nameLength + msg1Length + msg2Length + msg3Length);
                            break;
                        }
                }*/

                for (var b = 0; b < xs.Length; b += 2)
                {
                    var nx = (uint)((xs[b] << 8) + xs[b + 1]);
                    var ny = (uint)((ys[b] << 8) + ys[b + 1]);
                    if (layerNum == 1)
                    {
                        frame.Background[ny, nx] = (int)type;
                        if (bdata.coloredBlocks.Contains((int)type))
                        {
                            frame.BlockData7[ny, nx] = colour;
                        }

                    }
                    else
                    {
                        frame.Foreground[ny, nx] = (int)type;
                        if (bdata.coloredBlocks.Contains((int)type))
                        {
                            frame.BlockData7[ny, nx] = colour;
                        }
                        if (1645 == (int)type)
                        {
                            frame.BlockData[ny, nx] = (int)rotation;
                            frame.BlockData7[ny, nx] = colour;
                        }
                        if (EELVL.Blocks.IsType((int)type, Blocks.BlockType.Morphable) || EELVL.Blocks.IsType((int)type, Blocks.BlockType.Rotatable) || EELVL.Blocks.IsType((int)type, Blocks.BlockType.Number) || EELVL.Blocks.IsType((int)type, Blocks.BlockType.Enumerable))
                        {
                            frame.BlockData[ny, nx] = (int)datta;
                        }
                        if (type == 381 || type == 242)
                        {
                            frame.BlockData[ny, nx] = (int)rotation;
                            frame.BlockData1[ny, nx] = (int)id;
                            frame.BlockData2[ny, nx] = (int)target;
                        }
                        if (type == 374)
                        {
                            frame.BlockData[ny, nx] = (int)id;
                            frame.BlockData3[ny, nx] = targetP;
                        }
                        if (type == 1000)
                        {
                            frame.BlockData[ny, nx] = (int)wrapLength;
                            frame.BlockData3[ny, nx] = text;
                            frame.BlockData4[ny, nx] = textColour;
                        }
                        if (type == 77 || type == 83 || type == 1520)
                        {
                            frame.BlockData[ny, nx] = (int)id;
                        }
                        if (type == 385)
                        {
                            frame.BlockData[ny, nx] = (int)signType;
                            frame.BlockData3[ny, nx] = text;
                        }
                        if (bdata.isNPC((int)type))
                        {
                            frame.BlockData3[ny, nx] = name;
                            frame.BlockData4[ny, nx] = message1;
                            frame.BlockData5[ny, nx] = message2;
                            frame.BlockData6[ny, nx] = message3;
                        }
                    }
                    //this.SetBlock(nx, ny, type, layerNum, goal, rotation, target, id, colour, signType, wrapLength, targetP, text, textColour, name, message1, message2, message3);
                }

            }
            Console.WriteLine("Finished");


            /*foreach (var chunk in chunks)
            {
                foreach (var pos in chunk.Locations)
                {
                    if (chunk.Args.Length == 0)
                    {
                        if ((int)chunk.Layer == 1 && (int)chunk.Type != 631 && (int)chunk.Type != 632 && (int)chunk.Type != 633)
                        {
                            int x = pos.X;
                            int y = pos.Y;
                            frame.Background[y, x] = Convert.ToInt32(chunk.Type);
                        }
                        else
                        {
                            int x = pos.X;
                            int y = pos.Y;
                            frame.Foreground[y, x] = Convert.ToInt32(chunk.Type);
                        }
                    }
                    if (chunk.Args.Length == 1)
                    {
                        if (Convert.ToString(chunk.Args[0]) != "we")
                        {
                            if (bdata.goal.Contains((int)chunk.Type) || bdata.morphable.Contains((int)chunk.Type) || bdata.rotate.Contains((int)chunk.Type) && (int)chunk.Type != 385 && (int)chunk.Type != 374)
                            {
                                frame.Foreground[pos.Y, pos.X] = Convert.ToInt32(chunk.Type);
                                frame.BlockData[pos.Y, pos.X] = Convert.ToInt32(chunk.Args[0]);
                            }
                            else
                            {
                                if ((int)chunk.Type == 385)
                                {
                                    frame.Foreground[pos.Y, pos.X] = Convert.ToInt32(chunk.Type);
                                    frame.BlockData[pos.Y, pos.X] = Convert.ToInt32(chunk.Args[1]);
                                    frame.BlockData3[pos.Y, pos.X] = chunk.Args[0].ToString();
                                }
                                else if ((int)chunk.Type == 374)
                                {

                                    frame.Foreground[pos.Y, pos.X] = Convert.ToInt32(chunk.Type);
                                    frame.BlockData[pos.Y, pos.X] = Convert.ToInt32(chunk.Args[1]);
                                    frame.BlockData3[pos.Y, pos.X] = chunk.Args[0].ToString();
                                }
                                else if ((int)chunk.Type == 1000)
                                {
                                    frame.Foreground[pos.Y, pos.X] = Convert.ToInt32(chunk.Type);
                                    frame.BlockData[pos.Y, pos.X] = Convert.ToInt32(chunk.Args[1]);
                                    frame.BlockData3[pos.Y, pos.X] = chunk.Args[0].ToString();
                                    frame.BlockData4[pos.Y, pos.X] = chunk.Args[2].ToString();
                                }
                                else if ((int)chunk.Type == 1200 && (int)chunk.Layer == 0)
                                {

                                    frame.Foreground[pos.Y, pos.X] = Convert.ToInt32(chunk.Type);
                                    frame.BlockData7[pos.Y, pos.X] = Convert.ToUInt32(chunk.Args[0]);
                                }
                                else if ((int)chunk.Type == 631 || (int)chunk.Type == 632 || (int)chunk.Type == 633)
                                {
                                    
                                    frame.Background[pos.Y, pos.X] = Convert.ToInt32(chunk.Type);
                                    frame.BlockData7[pos.Y, pos.X] = Convert.ToUInt32(chunk.Args[0]);
                                }
                                else if ((int)chunk.Type != 374 && (int)chunk.Type != 385 && (int)chunk.Type != 631 && (int)chunk.Type != 632 && (int)chunk.Type != 633)
                                {
                                    if ((int)chunk.Layer == 1)
                                    {
                                        int x = pos.X;
                                        int y = pos.Y;
                                        frame.Background[y, x] = Convert.ToInt32(chunk.Type);
                                    }
                                    else
                                    {
                                        int x = pos.X;
                                        int y = pos.Y;
                                        frame.Foreground[y, x] = Convert.ToInt32(chunk.Type);
                                    }
                                }
                            }
                        }
                        else if (Convert.ToString(chunk.Args[0]) == "we")
                        {
                            if ((int)chunk.Layer == 1 && (int)chunk.Type != 631 && (int)chunk.Type != 632 && (int)chunk.Type != 633)
                            {
                                int x = pos.X;
                                int y = pos.Y;
                                frame.Background[y, x] = Convert.ToInt32(chunk.Type);
                            }
                            else
                            {
                                int x = pos.X;
                                int y = pos.Y;
                                frame.Foreground[y, x] = Convert.ToInt32(chunk.Type);
                            }
                        }
                    }
                    if (chunk.Args.Length == 2)
                    {
                        if (Convert.ToString(chunk.Args[0]) != "we")
                        {

                            if (chunk.Type == 385)
                            {
                                frame.Foreground[pos.Y, pos.X] = Convert.ToInt32(chunk.Type);
                                frame.BlockData[pos.Y, pos.X] = Convert.ToInt32(chunk.Args[1]);
                                frame.BlockData3[pos.Y, pos.X] = chunk.Args[0].ToString();
                            }
                            else if (chunk.Type == 374)
                            {
                                frame.Foreground[pos.Y, pos.X] = Convert.ToInt32(chunk.Type);
                                frame.BlockData3[pos.Y, pos.X] = chunk.Args[0].ToString();
                                frame.BlockData[pos.Y, pos.X] = Convert.ToInt32(chunk.Args[1]);
                            }
                            else if ((int)chunk.Type == 1200)
                            {
                                frame.Foreground[pos.Y, pos.X] = Convert.ToInt32(chunk.Type);
                                frame.BlockData7[pos.Y, pos.X] = Convert.ToUInt32(chunk.Args[0]);
                            }
                            else if ((int)chunk.Type == 631 || (int)chunk.Type == 632 || (int)chunk.Type == 633)
                            {
                                
                                frame.Background[pos.Y, pos.X] = (int)chunk.Type;
                                frame.BlockData7[pos.Y, pos.X] = Convert.ToUInt32(chunk.Args[0]);
                            }
                            else if (bdata.goal.Contains((int)chunk.Type) || bdata.morphable.Contains((int)chunk.Type) || bdata.rotate.Contains((int)chunk.Type))
                            {
                                frame.Foreground[pos.Y, pos.X] = Convert.ToInt32(chunk.Type);
                                frame.BlockData[pos.Y, pos.X] = Convert.ToInt32(chunk.Args[0]);
                            }
                        }
                    }
                    //if (chunk.Args.Length == 2 && (int)chunk.Type == 1000) { Chunk.Args[0] Chunk.Args[1] (Colored Text) }
                    if (chunk.Args.Length == 3)
                    {
                        if ((int)chunk.Type == 242 || (int)chunk.Type == 381)
                        {
                            if (Convert.ToString(chunk.Args[0]) != "we" && Convert.ToString(chunk.Args[1]) != "we" && Convert.ToString(chunk.Args[2]) != "we")
                            {
                                frame.Foreground[pos.Y, pos.X] = Convert.ToInt32(chunk.Type);
                                frame.BlockData[pos.Y, pos.X] = Convert.ToInt32(chunk.Args[0]);
                                frame.BlockData1[pos.Y, pos.X] = Convert.ToInt32(chunk.Args[1]);
                                frame.BlockData2[pos.Y, pos.X] = Convert.ToInt32(chunk.Args[2]);
                            }
                        }
                        else if ((int)chunk.Type == 385 || (int)chunk.Type == 374)
                        {
                            if (Convert.ToString(chunk.Args[0]) != "we" && Convert.ToString(chunk.Args[1]) != "we")
                            {
                                frame.Foreground[pos.Y, pos.X] = Convert.ToInt32(chunk.Type);
                                frame.BlockData[pos.Y, pos.X] = Convert.ToInt32(chunk.Args[1]);
                                frame.BlockData3[pos.Y, pos.X] = Convert.ToString(chunk.Args[0]);
                            }
                        }
                        else if (chunk.Type == 1000)
                        {
                            if (Convert.ToString(chunk.Args[0]) != "we" && Convert.ToString(chunk.Args[1]) != "we")
                            {
                                frame.Foreground[pos.Y, pos.X] = Convert.ToInt32(chunk.Type);
                                frame.BlockData3[pos.Y, pos.X] = chunk.Args[0].ToString();
                                frame.BlockData4[pos.Y, pos.X] = chunk.Args[1].ToString();
                                frame.BlockData[pos.Y, pos.X] = Convert.ToInt32(chunk.Args[2]);

                            }
                        }
                    }
                    if (chunk.Args.Length == 4)
                    {
                        if ((int)chunk.Type == 242 || (int)chunk.Type == 381)
                        {
                            if (Convert.ToString(chunk.Args[0]) != "we" && Convert.ToString(chunk.Args[1]) != "we" && Convert.ToString(chunk.Args[2]) != "we")
                            {
                                frame.Foreground[pos.Y, pos.X] = Convert.ToInt32(chunk.Type);
                                frame.BlockData[pos.Y, pos.X] = Convert.ToInt32(chunk.Args[0]);
                                frame.BlockData1[pos.Y, pos.X] = Convert.ToInt32(chunk.Args[1]);
                                frame.BlockData2[pos.Y, pos.X] = Convert.ToInt32(chunk.Args[2]);
                            }
                        }

                    }
                    if (bdata.isNPC((int)chunk.Type))
                    {
                        if (chunk.Args.Length == 4 || chunk.Args.Length == 5)
                        {
                            frame.Foreground[pos.Y, pos.X] = Convert.ToInt32(chunk.Type);
                            frame.BlockData3[pos.Y, pos.X] = Convert.ToString(chunk.Args[0]);
                            frame.BlockData4[pos.Y, pos.X] = Convert.ToString(chunk.Args[1]);
                            frame.BlockData5[pos.Y, pos.X] = Convert.ToString(chunk.Args[2]);
                            frame.BlockData6[pos.Y, pos.X] = Convert.ToString(chunk.Args[3]);
                        }
                    }

                }*/



            return frame;

        }
        #endregion

        public static Color UIntToColor(uint color)
        {
            byte a = (byte)(color >> 24);
            byte r = (byte)(color >> 16);
            byte g = (byte)(color >> 8);
            byte b = (byte)(color >> 0);
            return Color.FromArgb(a, r, g, b);
        }

        #region read from database
        public static Frame FromMessage1(DatabaseObject worlds, int width, int height)
        {
            if (worlds == null) return null;
            else
            {
                Frame frame = new Frame(1, 1);
                DatabaseArray worlddata = worlds.GetArray("blocks");
                if (worlds.Contains("blocks"))
                {
                    frame = new Frame(width, height);
                    int width1 = width;
                    int height1 = height;
                    for (int i = 0; i < worlddata.Count; i++)
                    {
                        if (worlddata[i] != null)
                        {
                            DatabaseObject worldinfo = worlddata.GetObject(i);
                            xx1 = worldinfo.GetBytes("x1");
                            yy1 = worldinfo.GetBytes("y1");
                            xx = worldinfo.GetBytes("x");
                            yy = worldinfo.GetBytes("y");
                            int bid = worldinfo.GetInt("type");
                            int layer = worldinfo.Contains("layer") ? worldinfo.GetInt("layer") : 0;
                            if (xx != null && yy != null)
                            {
                                for (int xxx = 0; xxx < xx.Length; xxx += 2)
                                {
                                    int tmpxx = xx[xxx] << 8 | xx[xxx + 1];
                                    int tmpyy = yy[xxx] << 8 | yy[xxx + 1];
                                    if (layer == 0)
                                    {
                                        object goal, signtype, text, rotation, id, target, name, mes1, mes2, mes3;
                                        frame.Foreground[tmpyy, tmpxx] = Convert.ToInt32(worldinfo["type"]);
                                        if (worldinfo.TryGetValue("name", out name)) frame.BlockData3[tmpyy, tmpxx] = name.ToString();
                                        if (worldinfo.TryGetValue("mes1", out mes1)) frame.BlockData4[tmpyy, tmpxx] = mes1.ToString();
                                        if (worldinfo.TryGetValue("mes2", out mes2)) frame.BlockData5[tmpyy, tmpxx] = mes2.ToString();
                                        if (worldinfo.TryGetValue("mes3", out mes3)) frame.BlockData6[tmpyy, tmpxx] = mes3.ToString();
                                        if (worldinfo.TryGetValue("five", out goal)) frame.BlockData[tmpyy, tmpxx] = Convert.ToInt32(goal);
                                        if (worldinfo.TryGetValue("five", out signtype)) frame.BlockData[tmpyy, tmpxx] = Convert.ToInt32(signtype);
                                        if (worldinfo.TryGetValue("text", out text)) frame.BlockData3[tmpyy, tmpxx] = text.ToString();
                                        if (worldinfo.TryGetValue("five", out rotation)) frame.BlockData[tmpyy, tmpxx] = Convert.ToInt32(rotation);
                                        if (worldinfo.TryGetValue("six", out id))
                                        {
                                            if (bdata.sound.Contains(Convert.ToInt32(worldinfo["type"])))
                                            {
                                                frame.BlockData[tmpyy, tmpxx] = (int)Convert.ToUInt32(id);
                                            }
                                            else
                                            {
                                                frame.BlockData1[tmpyy, tmpxx] = Convert.ToInt32(id);
                                            }
                                        }
                                        if (Convert.ToInt32(worldinfo["type"]) == 242 || Convert.ToInt32(worldinfo["type"]) == 381) frame.BlockData2[tmpyy, tmpxx] = Convert.ToInt32(worldinfo["seven"]);
                                        if (Convert.ToInt32(worldinfo["type"]) == 374) frame.BlockData3[tmpyy, tmpxx] = worldinfo["seven"].ToString();
                                        if (bid == 1000)
                                        {

                                            frame.Foreground[tmpyy, tmpxx] = (int)bid;
                                            if (worldinfo.Contains("text"))
                                            {
                                                int wrap = 200;
                                                string hexcolor = "#FFFFFF";
                                                frame.BlockData3[tmpyy, tmpxx] = worldinfo.GetString("text");
                                                if (worldinfo.Contains("text_color"))
                                                {
                                                    hexcolor = worldinfo.GetString("text_color");
                                                }
                                                if (worldinfo.Contains("five"))
                                                {
                                                    wrap = Convert.ToInt32(worldinfo["five"]);
                                                }
                                                frame.BlockData4[tmpyy, tmpxx] = hexcolor;
                                                frame.BlockData[tmpyy, tmpxx] = wrap;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        frame.Background[tmpyy, tmpxx] = (int)bid;
                                    }
                                }
                            }

                            if (xx1 != null && yy1 != null)
                            {
                                for (int xxxx = 0; xxxx < xx1.Length; xxxx++)
                                {
                                    int tmpxx0 = xx1[xxxx];
                                    int tmpyy0 = yy1[xxxx];
                                    if (layer == 0)
                                    {
                                        object goal, signtype, text, rotation, id, target, name, mes1, mes2, mes3;
                                        frame.Foreground[tmpyy0, tmpxx0] = Convert.ToInt32(worldinfo["type"]);
                                        if (worldinfo.TryGetValue("name", out name)) frame.BlockData3[tmpyy0, tmpxx0] = name.ToString();
                                        if (worldinfo.TryGetValue("mes1", out mes1)) frame.BlockData4[tmpyy0, tmpxx0] = mes1.ToString();
                                        if (worldinfo.TryGetValue("mes2", out mes2)) frame.BlockData5[tmpyy0, tmpxx0] = mes2.ToString();
                                        if (worldinfo.TryGetValue("mes3", out mes3)) frame.BlockData6[tmpyy0, tmpxx0] = mes3.ToString();
                                        if (worldinfo.TryGetValue("five", out goal)) frame.BlockData[tmpyy0, tmpxx0] = Convert.ToInt32(goal);
                                        if (worldinfo.TryGetValue("five", out signtype)) frame.BlockData[tmpyy0, tmpxx0] = Convert.ToInt32(signtype);
                                        if (worldinfo.TryGetValue("text", out text)) frame.BlockData3[tmpyy0, tmpxx0] = text.ToString();
                                        if (worldinfo.TryGetValue("five", out rotation)) frame.BlockData[tmpyy0, tmpxx0] = Convert.ToInt32(rotation);
                                        if (worldinfo.TryGetValue("six", out id))
                                        {
                                            if (bdata.sound.Contains(Convert.ToInt32(worldinfo["type"])))
                                            {
                                                frame.BlockData[tmpyy0, tmpxx0] = (int)Convert.ToUInt32(id);
                                            }
                                            else
                                            {
                                                frame.BlockData1[tmpyy0, tmpxx0] = Convert.ToInt32(id);
                                            }
                                        }
                                        if (Convert.ToInt32(worldinfo["type"]) == 242 || Convert.ToInt32(worldinfo["type"]) == 381) frame.BlockData2[tmpyy0, tmpxx0] = Convert.ToInt32(worldinfo["seven"]);
                                        if (Convert.ToInt32(worldinfo["type"]) == 374) frame.BlockData3[tmpyy0, tmpxx0] = worldinfo["seven"].ToString();
                                        if (bid == 1000)
                                        {

                                            frame.Foreground[tmpyy0, tmpxx0] = (int)bid;
                                            if (worldinfo.Contains("text"))
                                            {
                                                int wrap = 200;
                                                string hexcolor = "#FFFFFF";
                                                frame.BlockData3[tmpyy0, tmpxx0] = worldinfo.GetString("text");
                                                if (worldinfo.Contains("text_color"))
                                                {
                                                    hexcolor = worldinfo.GetString("text_color");
                                                }
                                                if (worldinfo.Contains("five"))
                                                {
                                                    wrap = Convert.ToInt32(worldinfo["five"]);
                                                }
                                                frame.BlockData4[tmpyy0, tmpxx0] = hexcolor;
                                                frame.BlockData[tmpyy0, tmpxx0] = wrap;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        frame.Background[tmpyy0, tmpxx0] = (int)bid;
                                    }
                                }
                            }
                        }
                    }
                }
                return frame;
            }
            //frame.Foreground[10, 10] = 9;
            //MainForm.editArea.Init(frame);

        }
        #endregion

        public List<string[]> Diff(Frame f)
        {
            List<string[]> res = new List<string[]>();
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    if (Foreground[y, x] != f.Foreground[y, x])
                    {
                        if (bdata.morphable.Contains(Foreground[y, x]) && !bdata.isNPC(Foreground[y, x]))
                        {
                            res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData[y, x].ToString() });
                        }
                        else if (bdata.goal.Contains(Foreground[y, x]) && Foreground[y, x] != 83 && Foreground[y, x] != 77 && Foreground[y, x] != 1520)
                        {
                            res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData[y, x].ToString() });
                        }
                        else if (bdata.rotate.Contains(Foreground[y, x]) && Foreground[y, x] != 385 && Foreground[y, x] != 374)
                        {
                            res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData[y, x].ToString() });
                        }
                        else if (bdata.portals.Contains(Foreground[y, x]))
                        {
                            res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData[y, x].ToString(), BlockData1[y, x].ToString(), BlockData2[y, x].ToString() });
                        }
                        else if (Foreground[y, x] == 83 || Foreground[y, x] == 77 || Foreground[y, x] == 1520)
                        {
                            res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData[y, x].ToString() });
                        }
                        else if (Foreground[y, x] == 385)
                        {
                            res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData[y, x].ToString(), BlockData3[y, x] });
                        }
                        else if (Foreground[y, x] == 374)
                        {
                            res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData3[y, x], BlockData[y, x].ToString() });
                        }
                        else if (Foreground[y, x] == 1000)
                        {
                            res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData[y, x].ToString(), BlockData3[y, x].ToString(), BlockData4[y, x].ToString() });
                        }
                        else if (bdata.isNPC(Foreground[y, x]))
                        {
                            res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData3[y, x], BlockData4[y, x], BlockData5[y, x], BlockData6[y, x] });
                        }
                        else if (Foreground[y, x] == 1000)
                        {
                            res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData[y, x].ToString(), BlockData3[y, x].ToString(), BlockData4[y, x].ToString() });
                        }
                        else if (Foreground[y, x] == 1200)
                        {
                            res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData7[y, x].ToString() });

                        }
                        else
                        {
                            if (MainForm.userdata.level.StartsWith("OW") && !MainForm.OpenWorldCode && MainForm.OpenWorld)
                            {
                                if (y > 4)
                                {
                                    res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0" });
                                }
                            }
                            else if (MainForm.userdata.level.StartsWith("OW") && MainForm.OpenWorldCode && MainForm.OpenWorld)
                            {
                                res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0" });
                            }
                            else
                            {
                                res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0" });
                            }
                        }
                    }
                    if (Foreground[y, x] == f.Foreground[y, x])
                    {
                        if (bdata.morphable.Contains(Foreground[y, x]) && !bdata.isNPC(Foreground[y, x]))
                        {
                            if (BlockData[y, x] != f.BlockData[y, x])
                            {
                                res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData[y, x].ToString() });
                            }
                        }
                        else if (bdata.goal.Contains(Foreground[y, x]) && Foreground[y, x] != 83 && Foreground[y, x] != 77 && Foreground[y, x] != 1520)
                        {
                            if (BlockData[y, x] != f.BlockData[y, x])
                            {
                                res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData[y, x].ToString() });
                            }
                        }
                        else if (bdata.rotate.Contains(Foreground[y, x]) && Foreground[y, x] != 385 && Foreground[y, x] != 374)
                        {
                            if (BlockData[y, x] != f.BlockData[y, x])
                            {
                                res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData[y, x].ToString() });
                            }
                        }
                        else if (bdata.portals.Contains(Foreground[y, x]))
                        {
                            if (BlockData[y, x] != f.BlockData[y, x] || BlockData1[y, x] != f.BlockData1[y, x] || BlockData2[y, x] != f.BlockData2[y, x])
                            {
                                res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData[y, x].ToString(), BlockData1[y, x].ToString(), BlockData2[y, x].ToString() });
                            }
                        }
                        else if (Foreground[y, x] == 83 || Foreground[y, x] == 77 || Foreground[y, x] == 1520)
                        {
                            if (BlockData[y, x] != f.BlockData[y, x])
                            {
                                res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData[y, x].ToString() });
                            }
                        }
                        else if (Foreground[y, x] == 385)
                        {
                            if (BlockData[y, x] != f.BlockData[y, x] || BlockData3[y, x] != f.BlockData3[y, x])
                            {
                                res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData[y, x].ToString(), BlockData3[y, x] });
                            }
                        }
                        else if (Foreground[y, x] == 374)
                        {
                            if (BlockData3[y, x] != f.BlockData3[y, x] || BlockData[y, x] != f.BlockData[y, x])
                            {
                                res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData3[y, x], BlockData[y, x].ToString() });
                            }
                        }
                        else if (bdata.isNPC(Foreground[y, x]))
                        {
                            if (BlockData3[y, x] != f.BlockData3[y, x] || BlockData4[y, x] != f.BlockData4[y, x] || BlockData5[y, x] != f.BlockData5[y, x] || BlockData6[y, x] != f.BlockData6[y, x])
                            {
                                res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData3[y, x], BlockData4[y, x], BlockData5[y, x], BlockData6[y, x] });
                            }
                        }
                        else if (Foreground[y, x] == 1000)
                        {
                            if (BlockData[y, x] != f.BlockData[y, x] || BlockData3[y, x] != f.BlockData3[y, x] || BlockData4[y, x] != f.BlockData4[y, x])
                            {
                                res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData[y, x].ToString(), BlockData3[y, x], BlockData4[y, x] });
                            }
                        }
                        else if (Foreground[y, x] == 1200)
                        {
                            if (BlockData7[y, x] != f.BlockData7[y, x])
                            {
                                res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData7[y, x].ToString() });
                            }
                        }
                    }

                    if (Background[y, x] == f.Background[y, x])
                    {
                        if (Background[y, x] == 631 || Background[y, x] == 632 || Background[y, x] == 633)
                        {
                            if (BlockData7[y, x] != f.BlockData7[y, x])
                            {
                                res.Add(new string[] { x.ToString(), y.ToString(), Background[y, x].ToString(), "1", BlockData7[y, x].ToString() });
                            }
                        }

                    }
                    if (Background[y, x] != f.Background[y, x])
                    {
                        if (Background[y, x] == 631 || Background[y, x] == 632 || Background[y, x] == 633)
                        {
                            res.Add(new string[] { x.ToString(), y.ToString(), Background[y, x].ToString(), "1", BlockData7[y, x].ToString() });

                        }
                        else
                        {
                            res.Add(new string[] { x.ToString(), y.ToString(), Background[y, x].ToString(), "1" });
                        }
                    }
                    /*if (Foreground[y, x] != f.Foreground[y, x])
                    {
                        Console.WriteLine(Foreground[y, x]);
                        if (bdata.goal.Contains(Foreground[y, x]) || bdata.rotate.Contains(Foreground[y, x]) || bdata.morphable.Contains(Foreground[y, x]) && Foreground[y, x] != 385 && Foreground[y, x] != 374 && BlockData[y, x] != f.BlockData[y, x])
                        {

                         res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData[y, x].ToString() });
                            
                        }
                        if (Foreground[y, x] == 385) res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData[y, x].ToString(), BlockData3[y, x].ToString() });
                        if (Foreground[y, x] == 374) res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData3[y, x].ToString() });
                        
                        if (bdata.portals.Contains(Foreground[y, x]))
                        {
                            res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData[y, x].ToString(), BlockData1[y, x].ToString(), BlockData2[y, x].ToString() });

                        }
                        else { res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0" }); }
                        
                    }
                    if (Foreground[y, x] == f.Foreground[y, x])
                    {
                        if (bdata.goal.Contains(Foreground[y, x]) || bdata.rotate.Contains(Foreground[y, x]) || bdata.morphable.Contains(Foreground[y, x]) && Foreground[y, x] != 385 && Foreground[y, x] != 374 && BlockData[y, x] != f.BlockData[y, x]) res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData[y, x].ToString() });
                        if (Foreground[y, x] == 385 && BlockData[y, x] != f.BlockData[y, x] || BlockData3[y, x] != f.BlockData3[y, x]) res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData[y, x].ToString(), BlockData3[y, x].ToString() });
                        if (Foreground[y, x] == 374 && BlockData3[y, x] != f.BlockData3[y, x]) res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData3[y, x].ToString() });
                        if (bdata.portals.Contains(Foreground[y, x]) && BlockData[y, x] != f.BlockData[y, x])
                        {
                            res.Add(new string[] { x.ToString(), y.ToString(), Foreground[y, x].ToString(), "0", BlockData[y, x].ToString(), BlockData1[y, x].ToString(), BlockData2[y, x].ToString() });

                        }
                    }
                    if (Background[y, x] != f.Background[y, x])
                    {
                        res.Add(new string[] { x.ToString(), y.ToString(), Background[y, x].ToString(), "1" });
                    }*/
                }
            }
            /*for (int i = 0; i < res.Count; i++)
            {
                for (int o = 0; o < res[i].Length; o++)
                {
                    Console.WriteLine("Id: " + i + "Data: " + res[i][o]);
                }
            }*/

            return res;
        }
        public void SaveEBELVL(FileStream file)
        {
            /*EELVL.Level savelvl = new Level(Width, Height);
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    int fid = Foreground[y, x];
                    int bid = Background[y, x];
                    if (Blocks.IsType(fid, Blocks.BlockType.Normal))
                    {
                        savelvl[0, x, y] = new Blocks.Block(fid);
                    }
                    if (Blocks.IsType(fid, Blocks.BlockType.Number))
                    {
                        savelvl[0, x, y] = new Blocks.NumberBlock(fid, BlockData[y, x]);
                    }
                    if (Blocks.IsType(fid, Blocks.BlockType.NPC))
                    {
                        savelvl[0, x, y] = new Blocks.NPCBlock(fid, BlockData3[y, x], BlockData4[y, x], BlockData5[y, x], BlockData6[y, x]);
                    }
                    if (Blocks.IsType(fid, Blocks.BlockType.Morphable))
                    {
                        savelvl[0, x, y] = new Blocks.MorphableBlock(fid, BlockData[y, x]);
                    }
                    if (Blocks.IsType(fid, Blocks.BlockType.Enumerable))
                    {
                        savelvl[0, x, y] = new Blocks.EnumerableBlock(fid, BlockData[y, x]);
                    }
                    if (Blocks.IsType(fid, Blocks.BlockType.Sign))
                    {
                        savelvl[0, x, y] = new Blocks.SignBlock(fid, BlockData3[y, x], BlockData[y, x]);
                    }
                    if (Blocks.IsType(fid, Blocks.BlockType.Rotatable))
                    {
                        int bdata = BlockData[y, x];
                        savelvl[0, x, y] = new Blocks.RotatableBlock(fid, bdata);
                    }
                    if (Blocks.IsType(fid, Blocks.BlockType.Portal))
                    {
                        savelvl[0, x, y] = new Blocks.PortalBlock(fid, BlockData[y, x], BlockData1[y, x], BlockData2[y, x]);
                    }
                    if (Blocks.IsType(fid, Blocks.BlockType.WorldPortal))
                    {
                        savelvl[0, x, y] = new Blocks.WorldPortalBlock(fid, BlockData3[y, x], BlockData[y, x]);
                    }
                    if (Blocks.IsType(fid, Blocks.BlockType.Music))
                    {
                        savelvl[0, x, y] = new Blocks.MusicBlock(fid, BlockData[y, x]);
                    }
                    if (Blocks.IsType(bid, Blocks.BlockType.Normal))
                    {
                        savelvl[1, x, y] = new Blocks.Block(bid);
                    }
                    if (Blocks.IsType(bid, Blocks.BlockType.BlockColor))
                    {
                        savelvl[1, x, y] = new Blocks.ColoredBlock(bid, BlockData7[y, x]);
                    }
                }
            }
            savelvl.Save(file);*/
        }
        public void SaveLVL(FileStream file)
        {
            EELVL.Level savelvl = new Level(Width, Height);
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    int fid = Foreground[y, x];
                    int bid = Background[y, x];
                    if (Blocks.IsType(fid, Blocks.BlockType.Normal))
                    {
                        savelvl[0, x, y] = new Blocks.Block(fid);
                    }
                    if (Blocks.IsType(fid, Blocks.BlockType.Number))
                    {
                        savelvl[0, x, y] = new Blocks.NumberBlock(fid, BlockData[y, x]);
                    }
                    if (Blocks.IsType(fid, Blocks.BlockType.NPC))
                    {
                        savelvl[0, x, y] = new Blocks.NPCBlock(fid, BlockData3[y, x], BlockData4[y, x], BlockData5[y, x], BlockData6[y, x]);
                    }
                    if (Blocks.IsType(fid, Blocks.BlockType.Morphable))
                    {
                        savelvl[0, x, y] = new Blocks.MorphableBlock(fid, BlockData[y, x]);
                    }
                    if (Blocks.IsType(fid, Blocks.BlockType.Enumerable))
                    {
                        savelvl[0, x, y] = new Blocks.EnumerableBlock(fid, BlockData[y, x]);
                    }
                    if (Blocks.IsType(fid, Blocks.BlockType.Sign))
                    {
                        savelvl[0, x, y] = new Blocks.SignBlock(fid, BlockData3[y, x], BlockData[y, x]);
                    }
                    if (Blocks.IsType(fid, Blocks.BlockType.Rotatable))
                    {
                        int bdata = BlockData[y, x];
                        savelvl[0, x, y] = new Blocks.RotatableBlock(fid, bdata);
                    }
                    if (Blocks.IsType(fid, Blocks.BlockType.Portal))
                    {
                        savelvl[0, x, y] = new Blocks.PortalBlock(fid, BlockData[y, x], BlockData1[y, x], BlockData2[y, x]);
                    }
                    if (Blocks.IsType(fid, Blocks.BlockType.WorldPortal))
                    {
                        savelvl[0, x, y] = new Blocks.WorldPortalBlock(fid, BlockData3[y, x], BlockData[y, x]);
                    }
                    if (Blocks.IsType(fid, Blocks.BlockType.Music))
                    {
                        savelvl[0, x, y] = new Blocks.MusicBlock(fid, BlockData[y, x]);
                    }
                    if (Blocks.IsType(bid, Blocks.BlockType.Normal))
                    {
                        savelvl[1, x, y] = new Blocks.Block(bid);
                    }
                }
            }
            savelvl.Save(file);

        }
        public void Save(System.IO.BinaryWriter writer)
        {
            writer.Write(Width);
            writer.Write(Height);
            for (int y = 0; y < Height; ++y)
                for (int x = 0; x < Width; ++x)
                {
                    int t = Foreground[y, x];
                    writer.Write((short)t);
                    writer.Write((short)Background[y, x]);
                    if (bdata.goal.Contains(t) || bdata.rotate.Contains(t) || bdata.morphable.Contains(t) && t != 385 && t != 374)
                    {
                        writer.Write((short)BlockData[y, x]);
                    }
                    if (t == 385)
                    {
                        writer.Write((short)BlockData[y, x]);
                        writer.Write(BlockData3[y, x]);
                    }
                    if (t == 374)
                    {
                        writer.Write(BlockData3[y, x]);
                        writer.Write((short)BlockData[y, x]);
                    }
                    if (bdata.portals.Contains(t))
                    {
                        writer.Write(BlockData[y, x]);
                        writer.Write(BlockData1[y, x]);
                        writer.Write(BlockData2[y, x]);
                    }
                    if (bdata.isNPC(t))
                    {
                        writer.Write(BlockData3[y, x]);
                        writer.Write(BlockData4[y, x]);
                        writer.Write(BlockData5[y, x]);
                        writer.Write(BlockData6[y, x]);
                    }
                }
            writer.Close();
        }

        public static bool[] detectWorlds(string file)
        {
            bool[] corrects = new bool[10];
            using (BinaryReader reader = new BinaryReader(File.Open(file, FileMode.Open)))
            {
                var width = reader.ReadInt32();
                var height = reader.ReadInt32();
                corrects[0] = width >= 25 && width <= 636 || height >= 25 && height <= 400;

                if (corrects[0])
                {
                    for (int y = 0; y < height; y++)
                    {
                        var fg = reader.ReadInt16();
                        corrects[1] = fg < 500 || fg >= 1001;

                        if (corrects[1])
                        {
                            var bg = reader.ReadInt16();
                            corrects[2] = bg >= 500 || bg <= 999;
                            if (bdata.goal.Contains(fg) || bdata.rotate.Contains(fg) || bdata.morphable.Contains(fg) && fg != 385 && fg != 374)
                            {
                                var rotation = reader.ReadInt16();
                                corrects[3] = rotation >= 0 || rotation <= 999;
                            }
                            if (fg == 385)
                            {
                                var rotation = reader.ReadInt16();
                                corrects[4] = rotation >= 0 || rotation <= 5;

                                var text = reader.ReadString();
                                corrects[5] = text.Length != 0;
                            }
                            if (fg == 374)
                            {
                                var text = reader.ReadString();
                                corrects[6] = text.Length != 0;
                            }
                            if (bdata.portals.Contains(fg))
                            {

                            }
                        }
                    }
                }
                reader.Close();
            }
            return corrects;
        }

        public static Frame LoadJSONDatabaseWorld(string input, bool isFilePath = true)
        {
            int width = 0, height = 0;
            var world = JObject.Parse(isFilePath ? File.ReadAllText(input) : input);

            width = world.TryGetValue("width", out var w) ? (int)world.GetValue("width") : 200;
            height = world.TryGetValue("height", out var h) ? (int)world.GetValue("height") : 200;

            if (world.TryGetValue("worlddata", out var wd))
            {
                var f = new Frame(width, height);
                var array = wd.Values().AsJEnumerable();
                var temp = new DatabaseArray();

                foreach (var block in array)
                {
                    var dbo = new DatabaseObject();

                    foreach (var token in block)
                    {
                        var property = (JProperty)token;
                        var value = property.Value;

                        switch (value.Type)
                        {
                            case JTokenType.Integer:
                                dbo.Set(property.Name, (uint)value);
                                break;
                            case JTokenType.Boolean:
                                dbo.Set(property.Name, (bool)value);
                                break;
                            case JTokenType.Float:
                                dbo.Set(property.Name, (double)value);
                                break;
                            default:
                                dbo.Set(property.Name, (string)value);
                                break;
                        }
                    }
                    temp.Add(dbo);
                }
                if (temp == null || temp.Count == 0) { f = null; }
                else
                {
                    for (int i = 0; i < temp.Count; i++)
                    {
                        if (temp.Contains(i) && temp.GetObject(i).Count != 0)
                        {
                            var obj = temp.GetObject(i);
                            byte[] x = TryGetBytes(obj, "x", new byte[0]), y = TryGetBytes(obj, "y", new byte[0]);
                            byte[] x1 = TryGetBytes(obj, "x1", new byte[0]), y1 = TryGetBytes(obj, "y1", new byte[0]);

                            for (int j = 0; j < x1.Length; j++)
                            {

                                if (y1[j] < height && x1[j] < width)
                                {
                                    try
                                    {
                                        if (Convert.ToInt32(obj["type"]) < 500 || Convert.ToInt32(obj["type"]) >= 1001)
                                        {
                                            f.Foreground[y1[j], x1[j]] = Convert.ToInt32(obj["type"]);
                                            object goal, signtype, text, rotation, id, target;
                                            if (obj.TryGetValue("goal", out goal)) f.BlockData[y1[j], x1[j]] = Convert.ToInt32(goal);
                                            if (obj.TryGetValue("signtype", out signtype)) f.BlockData[y1[j], x1[j]] = Convert.ToInt32(signtype);
                                            if (obj.TryGetValue("text", out text)) f.BlockData3[y1[j], x1[j]] = text.ToString();
                                            if (obj.TryGetValue("rotation", out rotation)) f.BlockData[y1[j], x1[j]] = Convert.ToInt32(rotation);
                                            if (obj.TryGetValue("id", out id))
                                            {
                                                if (bdata.sound.Contains(Convert.ToInt32(obj["type"])))
                                                {
                                                    f.BlockData[y1[j], x1[j]] = (int)Convert.ToUInt32(id);
                                                }
                                                else
                                                {
                                                    f.BlockData[y1[j], x1[j]] = Convert.ToInt32(id);
                                                }
                                            }
                                            if (obj.TryGetValue("target", out target) && !(target is string)) f.BlockData2[y1[j], x1[j]] = Convert.ToInt32(target);
                                            if (obj.TryGetValue("target", out target) && (target is string)) f.BlockData3[y1[j], x1[j]] = target.ToString();
                                        }
                                        else if (Convert.ToInt32(obj["type"]) >= 500 && Convert.ToInt32(obj["type"]) <= 999)
                                        {
                                            f.Background[y1[j], x1[j]] = Convert.ToInt32(obj["type"]);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.ToString());
                                    }

                                }

                            }

                            for (int k = 0; k < x.Length; k += 2)
                            {

                                int yy = (y[k] << 8) | y[k + 1];
                                int xx = (x[k] << 8) | x[k + 1];

                                if (yy < height && xx < width)
                                {
                                    try
                                    {
                                        if (Convert.ToInt32(obj["type"]) < 500 || Convert.ToInt32(obj["type"]) >= 1001)
                                        {
                                            object goal, signtype, text, rotation, id, target;
                                            f.Foreground[yy, xx] = Convert.ToInt32(obj["type"]);
                                            if (obj.TryGetValue("goal", out goal)) f.BlockData[yy, xx] = Convert.ToInt32(obj["goal"]);
                                            if (obj.TryGetValue("signtype", out signtype)) f.BlockData[yy, xx] = Convert.ToInt32(obj["signtype"]);
                                            if (obj.TryGetValue("text", out text)) f.BlockData3[yy, xx] = text.ToString();
                                            if (obj.TryGetValue("rotation", out rotation)) f.BlockData[yy, xx] = Convert.ToInt32(obj["rotation"]);
                                            if (obj.TryGetValue("id", out id))
                                            {
                                                if (bdata.sound.Contains(Convert.ToInt32(obj["type"])))
                                                {
                                                    f.BlockData[yy, xx] = (int)Convert.ToUInt32(id);
                                                }
                                                else
                                                {
                                                    f.BlockData[yy, xx] = Convert.ToInt32(id);
                                                }
                                            }
                                            if (obj.TryGetValue("target", out target) && target.GetType().ToString() != "System.String") f.BlockData2[yy, xx] = Convert.ToInt32(target);
                                            if (obj.TryGetValue("target", out target) && target.GetType().ToString() == "System.String") f.BlockData3[yy, xx] = target.ToString();
                                        }
                                        else if (Convert.ToInt32(obj["type"]) >= 500 && Convert.ToInt32(obj["type"]) <= 999)
                                        {
                                            f.Background[yy, xx] = Convert.ToInt32(obj["type"]);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.ToString());
                                    }
                                }




                            }
                        }
                    }

                }
                return f;
            }
            else if (world.TryGetValue("world", out var wd2))
            {
                var f = new Frame(width, height);
                try
                {
                    var xwd = Convert.FromBase64String(wd2.Value<string>());

                    for (var y = 0U; y < height; y++)
                    {
                        for (var x = 0U; x < width; x++)
                        {
                            try
                            {
                                f.Foreground[x, y] = xwd[y * width + x];
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                            }
                        }
                    }
                    return f;
                }
                catch { return null; }
            }

            return null;
        }

        public static byte[] TryGetBytes(DatabaseObject input, string key, byte[] defaultValue)
        {
            if (input.TryGetValue(key, out var obj))
            {
                return (obj is string) ? Convert.FromBase64String(obj as string) : (obj is byte[]) ? obj as byte[] : defaultValue;
            }

            return defaultValue;
        }

        public static void LoadEEBuilder(string file)
        {
            if (MainForm.editArea.Frames[0].Height >= 30 && MainForm.editArea.Frames[0].Width >= 41)
            {
                bool error = false;
                string[] lines = System.IO.File.ReadAllLines(file);
                int linee = 0;
                string[,] area = new string[29, 40];
                string[,] back = new string[29, 40];
                string[,] coins = new string[29, 40];
                string[,] id = new string[29, 40];
                string[,] target = new string[29, 40];
                string[,] text1 = new string[29, 40];
                string[,] text2 = new string[29, 40];
                string[,] text3 = new string[29, 40];
                string[,] text4 = new string[29, 40];

                foreach (string line in lines)
                {
                    linee += 1;
                    if (linee == 1)
                    {
                        if (Regex.IsMatch(line, @"[0-9]{1,3}\ [0-9]{1,3}\ [0-9]{1,3}\ [0-9]{1,3}\ [0-9]{1,3}\ [0-9]{1,3}\ [0-9]{1,3}\ [0-9]{1,3}\ [0-9]{1,3}\ [0-9]{1,3}"))
                        {
                            split1 = line.Split(' ');
                            error = false;
                        }
                        else
                        {
                            error = true;
                        }
                    }
                    else if (linee > 1)
                    {
                        if (!error)
                        {
                            string[] split = line.Split(new char[] { ' ' });
                            for (int m = 0; m < eebuilderData.Length / 2; m++)
                            {
                                int s1 = eebuilderData[m, 0], s2 = eebuilderData[m, 1];
                                int abc = Convert.ToInt32(split[0]) - 1;

                                if (Convert.ToInt32(split1[abc]) == s1)
                                {
                                    area[Convert.ToInt32(split[2]) - 1, Convert.ToInt32(split[1]) - 1] = s2.ToString();
                                }
                            }
                        }
                    }
                }
                if (!error)
                {
                    Clipboard.SetData("EEData", new string[][,] { area, back, coins, id, target, text1, text2, text3, text4 });
                    MainForm.editArea.Focus();
                    SendKeys.Send("^{v}");
                }
            }
            else
            {
                MessageBox.Show("The world is too small to handle EEBuilder files.\nWorlds should be larger or equal to width 30 and height 41", "World too small", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }
        public static Frame LoadFromEBELVL(string file)
        {

            EBELVL lvl = EBELVL.Open(file);
            if (lvl.Version == 1)
            {
                Frame f = new Frame(lvl.Width, lvl.Height);
                f.levelname = lvl.WorldName == null ? "Untitled World" : lvl.WorldName;
                f.nickname = lvl.OwnerName == null ? "Uknown" : lvl.OwnerName;
                if (lvl.BackgroundColor != 0)
                {
                    MainForm.userdata.useColor = true;
                    MainForm.userdata.thisColor = UIntToColor((uint)lvl.BackgroundColor);
                }
                else
                {
                    MainForm.userdata.useColor = false;
                    MainForm.userdata.thisColor = Color.Transparent;
                }
                if (lvl.Width <= 637 && lvl.Height <= 460 || lvl.Width <= 460 && lvl.Height <= 637)
                {
                    for (int x = 0; x < lvl.Width; ++x)
                    {
                        for (int y = 0; y < lvl.Height; ++y)
                        {
                            if (lvl[0, x, y] != null)
                            {
                                int fid = lvl[0, x, y].id;
                                if (fid < 500 || fid >= 1000)
                                {
                                    f.Foreground[y, x] = lvl[0, x, y].id;
                                    if (bdata.goalNew.Contains((int)fid))
                                    {
                                        f.BlockData[y, x] = Convert.ToInt32(lvl[0, x, y].args[0]);
                                    }
                                    if (bdata.rotationNew.Contains((int)fid))
                                    {
                                        f.BlockData[y, x] = Convert.ToInt32(lvl[0, x, y].args[0]);
                                    }
                                    if (bdata.portals.Contains(fid)) //Portals
                                    {
                                        f.BlockData[y, x] = Convert.ToInt32(lvl[0, x, y].args[0]);
                                        f.BlockData1[y, x] = Convert.ToInt32(lvl[0, x, y].args[1]);
                                        f.BlockData2[y, x] = Convert.ToInt32(lvl[0, x, y].args[2]);
                                    }
                                    if (fid == 374) //World Portal
                                    {
                                        f.BlockData[y, x] = Convert.ToInt32(lvl[0, x, y].args[0]);
                                        f.BlockData3[y, x] = Convert.ToString(lvl[0, x, y].args[1]);
                                    }
                                    if (fid == 1582) //World portal spawn point
                                    {
                                        f.BlockData[y, x] = Convert.ToInt32(lvl[0, x, y].args[0]);
                                    }
                                    if (fid == 1200) //Coloured blocks
                                    {
                                        f.BlockData[y, x] = Convert.ToInt32(lvl[0, x, y].args[0]);
                                    }
                                    if (fid == 1000) //Label
                                    {
                                        f.BlockData[y, x] = Convert.ToInt32(lvl[0, x, y].args[0]);
                                        f.BlockData3[y, x] = Convert.ToString(lvl[0, x, y].args[1]);
                                        f.BlockData3[y, x] = Convert.ToString(lvl[0, x, y].args[2]);
                                    }
                                    if (fid == 77 || fid == 83 || fid == 1520) //Music blocks
                                    {
                                        f.BlockData[y, x] = Convert.ToInt32(lvl[0, x, y].args[0]);
                                    }
                                    if (fid == 385) //Sign blocks
                                    {
                                        f.BlockData[y, x] = Convert.ToInt32(lvl[0, x, y].args[1]);
                                        f.BlockData3[y, x] = Convert.ToString(lvl[0, x, y].args[0]);
                                    }
                                    if (bdata.isNPC((int)fid)) //Npc blocks
                                    {
                                        f.BlockData3[y, x] = Convert.ToString(lvl[0, x, y].args[0]);

                                        f.BlockData4[y, x] = Convert.ToString(lvl[0, x, y].args[1]);
                                        f.BlockData5[y, x] = Convert.ToString(lvl[0, x, y].args[2]);
                                        f.BlockData6[y, x] = Convert.ToString(lvl[0, x, y].args[3]);

                                    }
                                }
                            }
                            if (lvl[1, x, y] != null)
                            {
                                int bid = lvl[1, x, y].id;
                                if (bid >= 500 && bid <= 999)
                                {
                                    f.Background[y, x] = lvl[1, x, y].id;
                                    if (bdata.coloredBlocks.Contains(bid))
                                    {
                                        f.BlockData7[y, x] = Convert.ToUInt32(lvl[1, x, y].args[0]);
                                    }
                                }
                            }
                        }
                    }
                }
                return f;
            }
            else
            {
                return null;
            }

        }
        public static Frame LoadFromEELVL(string file)
        {
            using (FileStream fs = new FileStream(file, FileMode.Open))
            {
                Level lvl = Level.Open(fs);
                Frame f = new Frame(lvl.Width, lvl.Height);
                f.levelname = lvl.WorldName;
                f.nickname = lvl.OwnerName;
                if (lvl.BackgroundColor != 0)
                {
                    MainForm.userdata.useColor = true;
                    MainForm.userdata.thisColor = UIntToColor(lvl.BackgroundColor);
                }
                else
                {
                    MainForm.userdata.useColor = false;
                    MainForm.userdata.thisColor = Color.Transparent;
                }
                if (lvl.Width <= 637 && lvl.Height <= 460 || lvl.Width <= 460 && lvl.Height <= 637)
                {
                    for (int x = 0; x < lvl.Width; ++x)
                    {
                        for (int y = 0; y < lvl.Height; ++y)
                        {
                            if (Blocks.IsType(lvl[0, x, y].BlockID, Blocks.BlockType.Normal))
                            {
                                f.Foreground[y, x] = lvl[0, x, y].BlockID;
                            }
                            if (Blocks.IsType(lvl[0, x, y].BlockID, Blocks.BlockType.Rotatable))
                            {
                                f.Foreground[y, x] = lvl[0, x, y].BlockID;
                                f.BlockData[y, x] = ((Blocks.RotatableBlock)lvl[0, x, y]).Rotation;
                            }

                            if (Blocks.IsType(lvl[0, x, y].BlockID, Blocks.BlockType.RotatableButNotReally))
                            {
                                f.Foreground[y, x] = lvl[0, x, y].BlockID;
                                f.BlockData[y, x] = 0;
                            }
                            if (Blocks.IsType(lvl[0, x, y].BlockID, Blocks.BlockType.NPC))
                            {

                                f.Foreground[y, x] = lvl[0, x, y].BlockID;
                                f.BlockData3[y, x] = ((Blocks.NPCBlock)lvl[0, x, y]).Name;
                                f.BlockData4[y, x] = ((Blocks.NPCBlock)lvl[0, x, y]).Message1;
                                f.BlockData5[y, x] = ((Blocks.NPCBlock)lvl[0, x, y]).Message2;
                                f.BlockData6[y, x] = ((Blocks.NPCBlock)lvl[0, x, y]).Message3;
                            }
                            if (Blocks.IsType(lvl[0, x, y].BlockID, Blocks.BlockType.Sign))
                            {
                                f.Foreground[y, x] = lvl[0, x, y].BlockID;
                                f.BlockData3[y, x] = ((Blocks.SignBlock)lvl[0, x, y]).Text;
                                f.BlockData[y, x] = ((Blocks.SignBlock)lvl[0, x, y]).Morph;
                            }
                            if (Blocks.IsType(lvl[0, x, y].BlockID, Blocks.BlockType.Portal))
                            {
                                f.Foreground[y, x] = lvl[0, x, y].BlockID;

                                f.BlockData[y, x] = ((Blocks.PortalBlock)lvl[0, x, y]).Rotation;
                                f.BlockData1[y, x] = ((Blocks.PortalBlock)lvl[0, x, y]).ID;
                                f.BlockData2[y, x] = ((Blocks.PortalBlock)lvl[0, x, y]).Target;
                            }
                            if (Blocks.IsType(lvl[0, x, y].BlockID, Blocks.BlockType.Morphable))
                            {
                                f.Foreground[y, x] = lvl[0, x, y].BlockID;
                                f.BlockData[y, x] = ((Blocks.MorphableBlock)lvl[0, x, y]).Morph;
                            }
                            if (Blocks.IsType(lvl[0, x, y].BlockID, Blocks.BlockType.Number))
                            {
                                f.Foreground[y, x] = lvl[0, x, y].BlockID;
                                f.BlockData[y, x] = ((Blocks.NumberBlock)lvl[0, x, y]).Number;
                            }
                            if (Blocks.IsType(lvl[0, x, y].BlockID, Blocks.BlockType.Enumerable))
                            {
                                f.Foreground[y, x] = lvl[0, x, y].BlockID;
                                f.BlockData[y, x] = ((Blocks.EnumerableBlock)lvl[0, x, y]).Variant;
                            }
                            if (Blocks.IsType(lvl[0, x, y].BlockID, Blocks.BlockType.WorldPortal))
                            {
                                f.Foreground[y, x] = lvl[0, x, y].BlockID;
                                f.BlockData[y, x] = ((Blocks.WorldPortalBlock)lvl[0, x, y]).Spawn;
                                f.BlockData3[y, x] = ((Blocks.WorldPortalBlock)lvl[0, x, y]).Target;
                            }
                            if (Blocks.IsType(lvl[0, x, y].BlockID, Blocks.BlockType.Music))
                            {
                                f.Foreground[y, x] = lvl[0, x, y].BlockID;
                                int temp = ((Blocks.MusicBlock)lvl[0, x, y]).Note;
                                f.BlockData[y, x] = temp;
                            }
                            if (Blocks.IsType(lvl[1, x, y].BlockID, Blocks.BlockType.Normal))
                            {
                                f.Background[y, x] = lvl[1, x, y].BlockID;
                            }
                            if (Blocks.IsType(lvl[0, x, y].BlockID, Blocks.BlockType.Label))
                            {
                                f.Foreground[y, x] = lvl[0, x, y].BlockID;
                                f.BlockData[y, x] = ((Blocks.LabelBlock)lvl[0, x, y]).Wrap;
                                f.BlockData3[y, x] = ((Blocks.LabelBlock)lvl[0, x, y]).Text;
                                f.BlockData4[y, x] = ((Blocks.LabelBlock)lvl[0, x, y]).Color;
                            }
                        }
                    }
                    f.toobig = false;
                }
                else
                {
                    f.toobig = true;
                }
                return f;
            }
        }
        public static Frame Load(System.IO.BinaryReader reader, int num)
        {
            /*
             * Loading new world anti-hack (not done)
             * reader.Close();
            bool[] bol = detectWorlds(file);
            int missed = 0;
            int got = 0;
            for (int i = 0;i < bol.Length;i++)
            {
                Console.WriteLine(bol[i]);
                if (bol[i]) got += 1;
                else missed += 1;
            }
            Console.WriteLine(missed + " " + got);
            */
            if (num == 6)
            {
                int width = reader.ReadInt32();
                int height = reader.ReadInt32();
                Frame f = new Frame(width, height);
                for (int y = 0; y < height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        int t = reader.ReadInt16();
                        f.Foreground[y, x] = t;
                        f.Background[y, x] = reader.ReadInt16();
                        if (bdata.goal.Contains(t) || bdata.rotate.Contains(t) || bdata.morphable.Contains(t) && t != 385 && t != 374)
                        {
                            f.BlockData[y, x] = Convert.ToInt32(reader.ReadInt16());
                        }
                        if (t == 385)
                        {
                            f.BlockData[y, x] = Convert.ToInt32(reader.ReadInt16());
                            f.BlockData3[y, x] = reader.ReadString();
                        }
                        if (t == 374)
                        {
                            f.BlockData3[y, x] = reader.ReadString();
                            f.BlockData[y, x] = Convert.ToInt32(reader.ReadInt16());
                        }
                        if (bdata.portals.Contains(t))
                        {
                            f.BlockData[y, x] = Convert.ToInt32(reader.ReadInt32());
                            f.BlockData1[y, x] = reader.ReadInt32();
                            f.BlockData2[y, x] = reader.ReadInt32();
                        }
                        if (bdata.isNPC(t))
                        {
                            f.BlockData3[y, x] = reader.ReadString();
                            f.BlockData4[y, x] = reader.ReadString();
                            f.BlockData5[y, x] = reader.ReadString();
                            f.BlockData6[y, x] = reader.ReadString();
                        }
                    }
                }
                return f;
            }
            if (num == 5)
            {
                int width = reader.ReadInt32();
                int height = reader.ReadInt32();
                Frame f = new Frame(width, height);
                for (int y = 0; y < height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        int t = reader.ReadInt16();
                        f.Foreground[y, x] = t;
                        f.Background[y, x] = reader.ReadInt16();
                        if (bdata.goal.Contains(t) || bdata.rotate.Contains(t) || bdata.morphable.Contains(t) && t != 385 && t != 374)
                        {
                            f.BlockData[y, x] = Convert.ToInt32(reader.ReadInt16());
                        }
                        if (t == 385)
                        {
                            f.BlockData[y, x] = Convert.ToInt32(reader.ReadInt16());
                            f.BlockData3[y, x] = reader.ReadString();
                        }
                        if (t == 374)
                        {
                            f.BlockData3[y, x] = reader.ReadString();
                        }
                        if (bdata.portals.Contains(t))
                        {
                            f.BlockData[y, x] = Convert.ToInt32(reader.ReadInt32());
                            f.BlockData1[y, x] = reader.ReadInt32();
                            f.BlockData2[y, x] = reader.ReadInt32();
                        }
                        if (bdata.isNPC(t))
                        {
                            f.BlockData3[y, x] = reader.ReadString();
                            f.BlockData4[y, x] = reader.ReadString();
                            f.BlockData5[y, x] = reader.ReadString();
                            f.BlockData6[y, x] = reader.ReadString();
                        }
                    }
                }
                return f;
            }
            if (num == 4)
            {
                int width = reader.ReadInt32();
                int height = reader.ReadInt32();
                Frame f = new Frame(width, height);
                for (int y = 0; y < height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        int t = reader.ReadInt16();
                        f.Foreground[y, x] = t;
                        f.Background[y, x] = reader.ReadInt16();
                        if (bdata.goal.Contains(t) || bdata.rotate.Contains(t) || bdata.morphable.Contains(t) && t != 385 && t != 374)
                        {
                            f.BlockData[y, x] = Convert.ToInt32(reader.ReadInt16());
                        }
                        if (t == 385)
                        {
                            f.BlockData[y, x] = Convert.ToInt32(reader.ReadInt16());
                            f.BlockData3[y, x] = reader.ReadString();
                        }
                        if (t == 374)
                        {
                            f.BlockData3[y, x] = reader.ReadString();
                        }
                        if (bdata.portals.Contains(t))
                        {
                            f.BlockData[y, x] = Convert.ToInt32(reader.ReadInt32());
                            f.BlockData1[y, x] = reader.ReadInt32();
                            f.BlockData2[y, x] = reader.ReadInt32();
                        }
                    }
                }
                return f;
            }
            if (num == 3)
            {
                char[] filetype = reader.ReadChars(16);
                if (new string(filetype) == "ANIMATOR SAV V05")
                {
                    reader.ReadInt16();
                    int LayerCount = Convert.ToInt16(reader.ReadInt16());
                    int width = Convert.ToInt16(reader.ReadInt16());
                    int height = Convert.ToInt16(reader.ReadInt16());
                    Frame f = new Frame(width, height);
                    for (int z = 1; z >= 0; z += -1)
                    {
                        for (int y = 0; y <= height - 1; y++)
                        {
                            for (int x = 0; x <= width - 1; x++)
                            {
                                int bid = eeanimator2blocks(Convert.ToInt16(reader.ReadInt16()));
                                if (bid >= 500 && bid <= 900)
                                {
                                    f.Background[y, x] = bid;
                                }
                                else
                                {
                                    f.Foreground[y, x] = bid;
                                }
                            }
                        }
                    }
                    return f;
                }
                else
                {
                    return null;
                }
            }

            if (num >= 0 && num <= 2)
            {
                int width = reader.ReadInt32();
                int height = reader.ReadInt32();
                Frame f = new Frame(width, height);

                for (var y = 0; y < height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        if (num == 0)
                        {
                            int t = reader.ReadByte();
                            f.Foreground[y, x] = t;
                            f.Background[y, x] = 0;
                            if (bdata.goal.Contains(t) || bdata.rotate.Contains(t) || bdata.morphable.Contains(t))
                            {
                                f.BlockData[y, x] = Convert.ToInt32(reader.ReadInt16());
                            }
                            else if (bdata.portals.Contains(t))
                            {
                                var r = reader.ReadInt32();
                                var a = r >> 16;
                                var b = ((r >> 8) & 0xFF);
                                var c = (r & 0xFF);
                                f.BlockData[y, x] = Convert.ToInt32(a);
                                f.BlockData1[y, x] = b;
                                f.BlockData2[y, x] = c;
                            }
                        }
                        else if (num == 1)
                        {
                            int t = reader.ReadInt16();
                            f.Foreground[y, x] = t;
                            f.Background[y, x] = reader.ReadInt16();
                            if (bdata.goal.Contains(t) || bdata.rotate.Contains(t) || bdata.morphable.Contains(t))
                            {
                                f.BlockData[y, x] = Convert.ToInt32(reader.ReadInt16());
                            }
                            else if (bdata.portals.Contains(t))
                            {
                                var r = reader.ReadInt32();
                                var a = r >> 16;
                                var b = ((r >> 8) & 0xFF);
                                var c = (r & 0xFF);
                                f.BlockData[y, x] = Convert.ToInt32(a);
                                f.BlockData1[y, x] = b;
                                f.BlockData2[y, x] = c;
                            }
                        }
                        else if (num == 2)
                        {
                            int t = reader.ReadInt16();
                            f.Foreground[y, x] = t;
                            f.Background[y, x] = reader.ReadInt16();
                            if (bdata.goal.Contains(t) || bdata.rotate.Contains(t) || bdata.morphable.Contains(t))
                            {
                                f.BlockData[y, x] = Convert.ToInt32(reader.ReadInt16());
                            }
                            else if (t == 374)
                            {
                                f.BlockData[y, x] = 0;
                                f.BlockData3[y, x] = reader.ReadString();
                            }
                            else if (t == 385)
                            {
                                f.BlockData3[y, x] = reader.ReadString();
                            }
                            else if (bdata.portals.Contains(t))
                            {
                                f.BlockData[y, x] = Convert.ToInt32(reader.ReadInt32());
                                f.BlockData1[y, x] = reader.ReadInt32();
                                f.BlockData2[y, x] = reader.ReadInt32();
                            }
                        }
                    }
                }

                return f;
            }
            else
            {
                return null;
            }
        }

        static int eeanimator2blocks(int id)
        {
            if (id == 127)
            {
                return 0;
            }
            else if (id - 128 >= 0 && id - 128 <= 63)
            {
                return id - 128;
            }
            else if (id + 256 >= 500 && id + 256 <= 600)
            {
                return id + 256;
            }
            else
            {
                return id - 1024;
            }
        }

        static int[,] eebuilderData = new int[,]
        {
                { 1, 9 }, { 2, 10 }, { 3, 11 }, { 4, 12 }, { 5, 13 }, { 6, 14 }, { 7, 15 },
                { 8, 37 }, { 9, 38 }, { 10, 39 }, { 11, 40 }, { 12, 41 }, { 13, 42 },
                { 14, 16 }, { 15, 17 }, { 16, 18 }, { 17, 19 }, { 18, 20 }, { 19, 21 },
                { 20, 29 }, { 21, 30 }, { 22, 31 }, { 23, 34 }, { 24, 35 }, { 25, 36 },
                { 26, 22 }, { 27, 32 }, { 28, 33 }, { 29, 44 },
                { 30, 6 }, { 31, 7 }, { 32, 8 }, { 33, 23 }, { 34, 24 }, { 35, 25 },
                { 36, 0 }, { 37, 26 }, { 38, 27 }, { 39, 28 },
                { 40, 0 }, { 41, 1 }, { 42, 2 }, { 43, 3 }, { 44, 4 }, { 45, 100 }, { 46, 101 },
                { 47, 5 }, { 48, 255 },
                { 49, 0 }, { 50, 0 }, { 51, 0 }, { 52, 0 }, { 53, 0 }, { 54, 0 },
                { 55, 0 }, { 56, 0 }, { 57, 0 }, { 58, 0 }, { 59, 0 },
                { 60, 45 }, { 61, 46 }, { 62, 47 }, { 63, 48 }, { 64, 49 },
                { 65, 50 }, { 66, 243 },
                { 67, 51 }, { 68, 52 }, { 69, 53 }, { 70, 54 }, { 71, 55 }, { 72, 56 }, { 73, 57 }, { 74, 58 },
                { 75, 233 }, { 76, 234 }, { 77, 235 }, { 78, 236 }, { 79, 237 }, { 80, 238 }, { 81, 239 }, { 82, 240 },
        };
    }
    /*public class blockData : IEquatable<blockData>
    {
        public int X;
        public int Y;
        public int Bid;
        public int Layer;
        public object[] Param;
        public blockData(int X, int Y, int Bid, int Layer, object[] Param = null)
        {
            this.X = X;
            this.Y = Y;
            this.Bid = Bid;
            this.Param = Param;
        }
        public bool Equals(blockData other)
        {
            if (other != null)
            {
                return this.X == other.X &&
                       this.Y == other.Y &&
                       this.Bid == other.Bid &&
                       this.Layer == other.Layer;
            }
            else { return false; }
        }

    }*/
}
