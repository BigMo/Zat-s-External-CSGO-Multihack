using laExternalMulti.Objects.Implementation.CSGO.Data;
using laExternalMulti.Objects.Implementation.CSGO.Data.BSP;
using laExternalMulti.Objects.Implementation.CSGO.Data.BSP.Enums;
using laExternalMulti.Objects.Implementation.CSGO.Data.BSP.Structs;
using laExternalMulti.Objects.Implementation.CSGO.Data.Enums;
using laExternalMulti.Objects.UI;
using laExternalMulti.Properties;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace laExternalMulti.Objects.Implementation.CSGO.UI
{
    class ESP : CSGOControl
    {
        #region VARIABLES
        private Color foreColor = new Color(0.8f, 0.8f, 0.8f, 0.9f);
        private Color backColor = new Color(0.0f, 0.0f, 0.0f, 0.9f);
        private Color lifeBarForeground = new Color(0.2f, 0.8f, 0.2f, 0.9f);
        private Color lifeBarBackground = new Color(0.2f, 0.2f, 0.2f, 0.9f);

        private Color colorCTSpotted = new Color(0.1f, 0.5f, 0.8f, 0.9f);
        private Color colorCT = new Color(0.5f, 0.8f, 0.9f, 0.9f);

        private Color colorTSpotted = new Color(0.5f, 0.1f, 0.1f, 0.9f);
        private Color colorT = new Color(0.9f, 0.5f, 0.1f, 0.9f);

        private Color viewColor = new Color(1f, 0.75f, 0.1f, 0.33f);
        private Color viewColorOutline = new Color(1f, 0.75f, 0.1f, 0.9f);
        private Color enemyColor = new Color(0.9f, 0.9f, 0.9f, 0.2f);
        private CSGOImplementation csgo;
        private const float lifeBarHeight = 6f, lifeBarWidth = 5f;
        private const float UNIT_TO_METERS = 0.01905f;
        float aspect = 80f / 200f;

        private SharpDX.Direct2D1.Bitmap ranksBmp;
        private bool once;
        #endregion

        #region CONSTRUCTOR
        public ESP(CSGOTheme theme, TextFormat font)
            : base(theme, font)
        {
            once = true;
        }
        #endregion

        #region METHODS
        #region HELPER
        private SharpDX.Direct2D1.Bitmap SDXBitmapFromSysBitmap(WindowRenderTarget device, System.Drawing.Bitmap bitmap)
        {
            var sourceArea = new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height);
            var bitmapProperties = new BitmapProperties(new PixelFormat(SharpDX.DXGI.Format.R8G8B8A8_UNorm, AlphaMode.Premultiplied));
            var size = new Size2(bitmap.Width, bitmap.Height);

            // Transform pixels from BGRA to RGBA
            int stride = bitmap.Width * sizeof(int);
            using (var tempStream = new DataStream(bitmap.Height * stride, true, true))
            {
                // Lock System.Drawing.Bitmap
                var bitmapData = bitmap.LockBits(sourceArea, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                // Convert all pixels 
                for (int y = 0; y < bitmap.Height; y++)
                {
                    int offset = bitmapData.Stride * y;
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        // Not optimized 
                        byte B = Marshal.ReadByte(bitmapData.Scan0, offset++);
                        byte G = Marshal.ReadByte(bitmapData.Scan0, offset++);
                        byte R = Marshal.ReadByte(bitmapData.Scan0, offset++);
                        byte A = Marshal.ReadByte(bitmapData.Scan0, offset++);
                        int rgba = R | (G << 8) | (B << 16) | (A << 24);
                        tempStream.Write(rgba);
                    }

                }
                bitmap.UnlockBits(bitmapData);
                tempStream.Position = 0;

                return new SharpDX.Direct2D1.Bitmap(device, size, tempStream, stride, bitmapProperties);
            }
        }
        #endregion
        protected override void OnDraw(SharpDX.Direct2D1.WindowRenderTarget device)
        {
            if (once)
            {
                System.Drawing.Bitmap bmp = (System.Drawing.Bitmap)Resources.ResourceManager.GetObject("ranks");
                try
                {
                    this.ranksBmp = SDXBitmapFromSysBitmap(device, bmp);
                    once = false;
                }
                catch { }
            }
            csgo = (CSGOImplementation)Program.GameImplementation;
            Player currentPlayer = csgo.GetCurrentPlayer();
            if (csgo.SignOnState < SignOnState.SIGNONSTATE_PRESPAWN || csgo.SignOnState > SignOnState.SIGNONSTATE_FULL)
                return;
            if (csgo.Players == null)
                return;
            if (csgo.Players.Length == 0)
                return;
            if (currentPlayer == null)
                return;
            if (csgo.GetValue<YesNo>("espEnabled") == YesNo.No)
                return;

            try { DrawWorld(device, csgo); }
            catch { }

            try
            {
                foreach (Player player in csgo.Players)
                    DrawPlayer(device, currentPlayer, player);
                foreach (Entity entity in csgo.Entities)
                    DrawEntity(device, currentPlayer, entity);
                for (int i = csgo.Damages.Count - 1; i >= 0; i--)
                    DrawDamage(device, currentPlayer, csgo.Damages[i]);
            }
            catch { }
            //DrawPunchAngles(device, csgo);
        }
        private void DrawWorld(WindowRenderTarget device, CSGOImplementation csgo)
        {
            if (csgo.CurrentMap == null)
                return;
            if (csgo.GetValue<YesNo>("wireframeEnabled") == YesNo.No)
                return;

            BSP map = csgo.CurrentMap;
            Color color = new Color((int)csgo.GetValue<float>("wireframeColorR"), (int)csgo.GetValue<float>("wireframeColorG"), (int)csgo.GetValue<float>("wireframeColorB"), (int)csgo.GetValue<float>("wireframeColorA"));
            float distanceMeters = csgo.GetValue<float>("wireframeDistance");
            float scale = 1f;
            bool drawOnlyMe = (csgo.GetValue<YesNo>("wireframeDrawAroundMe") == YesNo.Yes);
            Target drawTarget = csgo.GetValue<Target>("wireframeDrawTarget");

            for (int f = 0; f < map.OriginalFaces.Length; f++)
            {
                Face face = map.OriginalFaces[f];
                if (map.TextureInfo.Length > face.texinfo)
                {
                    if ((map.TextureInfo[face.texinfo] & SurfFlag.SURF_NODRAW) == SurfFlag.SURF_NODRAW)
                        continue;
                }
                for (int e = 0; e < face.numEdges; e++)
                {

                    float dist = 1f;
                    int surfedge = map.Surfedges[face.firstEdge + e];
                    ushort[] edge = map.Edges[Math.Abs(surfedge)];
                    Vector3 line3d1, line3d2;
                    if (surfedge > 0)
                    {
                        line3d1 = map.Vertices[edge[0]];
                        line3d2 = map.Vertices[edge[1]];
                    }
                    else
                    {
                        line3d1 = map.Vertices[edge[1]];
                        line3d2 = map.Vertices[edge[0]];
                    }

                    bool valid = false;
                    dist = Geometry.GetDistanceToPoint(line3d1, csgo.LocalPlayer.Vector3) * UNIT_TO_METERS;
                    //valid = true;
                    //if (dist > distanceMeters)
                    //    break;
                    if (!drawOnlyMe)
                    {
                        foreach (Player player in csgo.Players)
                        {
                            if (player == null)
                                continue;
                            if (!player.IsValid())
                                continue;
                            if (drawTarget != Target.Everyone)
                            {
                                if (drawTarget == Target.Allies && (player.InTeam != csgo.LocalPlayer.InTeam))
                                    continue;
                                if (drawTarget == Target.Enemies && (player.InTeam == csgo.LocalPlayer.InTeam))
                                    continue;
                            }
                            float tmpDist = Geometry.GetDistanceToPoint(line3d1, player.Vector3) * UNIT_TO_METERS;
                            if (
                                tmpDist < distanceMeters/* ||
                                Geometry.GetDistanceToPoint(line3d2, player.Vector3) * UNIT_TO_METERS < distanceMeters*/
                            )
                            {
                                valid = true;
                                if (tmpDist < dist)
                                    dist = tmpDist;
                            }
                        }
                        if (!valid)
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (dist > distanceMeters)
                        {
                            break;
                        }
                    }
                    Vector2[] line = Geometry.WorldToScreen(csgo.ViewMatrix, csgo.ScreenSize, line3d1, line3d2);
                    if (line[0] == Vector2.Zero || line[1] == Vector2.Zero)
                        continue;
                    Color col = color;
                    if (csgo.GetValue<OnOff>("wireframeFading") == OnOff.On)
                    {
                        scale = 1f - 1f / distanceMeters * dist;
                        col = color;
                        col.A = (byte)(((float)col.A) * scale);
                    }
                    DrawLine(device, col, line[0].X, line[0].Y, line[1].X, line[1].Y, 1f);
                }

            }
        }
        private void DrawPunchAngles(WindowRenderTarget device, CSGOImplementation csgo)
        {
            DrawAngle(device, 700f, 300f, csgo.LocalPlayer.PunchVector.X, "Pitch"); //Y
            DrawAngle(device, 800f, 300f, csgo.LocalPlayer.PunchVector.Y, "Yaw");   //X
            DrawAngle(device, 900f, 300f, csgo.LocalPlayer.PunchVector.Z, "Roll");
        }
        private void DrawAngle(WindowRenderTarget device, float x, float y, float angle, string text)
        {
            Vector2 vecOrigin = new Vector2(x, y);
            Vector2 vecRotated = new Vector2(x, y - 100);
            vecRotated = Geometry.RotatePoint(vecRotated, vecOrigin, angle);
            float pixelPerDeg = 1680f / 90f;
            this.DrawLine(device, Color.Red, vecOrigin.X, vecOrigin.Y, vecRotated.X, vecRotated.Y, 1f);
            this.DrawText(device, Color.Red, vecOrigin.X, vecOrigin.Y + 10f, 100f, 20f, String.Format("{0}: {1}, {2}px", text, Math.Round(angle, 2), Math.Round(pixelPerDeg * angle, 2)), this.Font);
        }
        private void DrawPlayer(WindowRenderTarget device, Player currentPlayer, Player player)
        {
            if (player == null)
                return;
            if (player.Index == currentPlayer.Index)
                return;
            if (!player.IsValid())
                return;
            if (csgo.GetValue<Target>("espDrawTarget") == Target.Enemies && player.InTeam == currentPlayer.InTeam)
                return;
            if (csgo.GetValue<Target>("espDrawTarget") == Target.Allies && player.InTeam != currentPlayer.InTeam)
                return;

            player.CheckYaw();

            Vector2 point = Geometry.WorldToScreen(csgo.ViewMatrix, csgo.ScreenSize, (player.Vector3));
            if (point == Vector2.Zero)
                return;
            float distance = Geometry.GetDistanceToPoint(currentPlayer.Vector3, player.Vector3);
            float height = 36000f / distance;
            float width = 18100f / distance;
            Color colorPlayer = player.InTeam == Team.Terrorists ?
                (player.SeenBy(currentPlayer) ? colorTSpotted : colorT) :
                (player.SeenBy(currentPlayer) ? colorCTSpotted : colorCT);

            float distanceMeter = currentPlayer.DistanceToOtherEntityInMetres(player);
            float head = 3200f / distance;
            float boxBorder = 6400f / distance;
            bool highlighted = csgo.Highlighted[player.Index - 1];
            //Paint bones
            List<Vector3> allBones = new List<Vector3>();

            allBones.AddRange(player.Skeleton.Legs);
            allBones.AddRange(player.Skeleton.Spine);
            allBones.Add(player.Skeleton.HeadEnd);

            Vector2[] all = Geometry.WorldToScreen(csgo.ViewMatrix, csgo.ScreenSize, allBones.ToArray());
            foreach (Vector2 vec in all)
                if (vec == Vector2.Zero)
                    return;
            Vector2[] arms = Geometry.WorldToScreen(csgo.ViewMatrix, csgo.ScreenSize, player.Skeleton.Arms);
            Vector2[] legs = Geometry.WorldToScreen(csgo.ViewMatrix, csgo.ScreenSize, player.Skeleton.Legs);
            Vector2[] spine = Geometry.WorldToScreen(csgo.ViewMatrix, csgo.ScreenSize, player.Skeleton.Spine);
            allBones.Clear();
            allBones = null;

            float rectX = GetSmallestX(all) + 5, rectW = GetBiggestX(all) + 5, rectY = GetSmallestY(all) + 5, rectH = GetBiggestY(all) + 5;
            //rectW -= rectX;
            rectH -= rectY;
            rectW = rectH * 0.3f;
            rectX = point.X - rectW / 2f;
            #region glow
            if (csgo.GetValue<YesNo>("espGlowEnabled") == YesNo.Yes)
            {
                if (csgo.GlowObjects != null)
                {
                    CSGOGameController controller = (CSGOGameController)Program.GameController;
                    int idx = controller.MemoryUpdater.GetGlowObjectByAddress((int)player.Address);
                    if (idx != -1)
                    {
                        GlowObjectDefinition def = csgo.GlowObjects[idx];
                        def.a = (float)(colorPlayer.A / 255f);
                        def.r = (float)(colorPlayer.R / 255f);
                        def.g = (float)(colorPlayer.G / 255f);
                        def.b = (float)(colorPlayer.B / 255f);
                        def.m_bRenderWhenOccluded = true;
                        def.m_bRenderWhenUnoccluded = true;
                        def.m_bFullBloom = false;
                        if (csgo.GetValue<YesNo>("espGlowFadingEnabled") == YesNo.Yes)
                        {
                            float dist = currentPlayer.DistanceToOtherEntityInMetres(player);
                            float range = csgo.GetValue<float>("espGlowFadingDistance");
                            if (dist <= range)
                            {
                                def.a = 1f - 1f / range * dist;
                                controller.MemoryUpdater.WriteGlowObject(def, idx);
                            }
                        }
                        else
                        {
                            controller.MemoryUpdater.WriteGlowObject(def, idx);
                        }
                    }
                }
            }
            #endregion
            #region skeleton
            if (csgo.GetValue<OnOff>("espDrawSkeleton") == OnOff.On)
            {
                if (distanceMeter < 20)
                    DrawBones(device,
                        colorPlayer,
                        arms,
                        1.5f,
                        player.InTeam
                        );
                DrawBones(device,
                    colorPlayer,
                    legs,
                    1.5f,
                    player.InTeam
                    );
                DrawBones(device,
                    colorPlayer,
                    spine,
                    1.5f,
                    player.InTeam
                    );
            }
            if (player.Skeleton.AllBones != null)
            {
                for (int i = 0; i < player.Skeleton.AllBones.Length; i++)
                {
                    Vector2 boneHead = Geometry.WorldToScreen(csgo.ViewMatrix, csgo.ScreenSize, player.Skeleton.AllBones[i]);
                    DrawText(device,
                        foreColor,
                        backColor,
                        boneHead.X,
                        boneHead.Y,
                        100f,
                        20f,
                        1f,
                        1f,
                        i.ToString(),
                        FactoryManager.GetFont("largeSegoe"));
                }
            }
            #endregion
            #region lines
            if (csgo.GetValue<OnOff>("espDrawLines") == OnOff.On)
            {
                Color color = colorPlayer;
                if (!player.IsSpotted)
                {
                    color *= 0.5f;
                }
                else
                {
                    color *= (0.75f + 0.25f * GetColorMultiplier());
                }
                FillPolygon(device,
                    color,
                    new Vector2(csgo.ScreenSize.Width / 2f, csgo.ScreenSize.Height),
                    new Vector2(point.X - width / 2f, point.Y),
                    new Vector2(point.X + width / 2f, point.Y));
            }
            #endregion
            #region box
            //Draw box
            if (csgo.GetValue<OnOff>("espDrawBox") == OnOff.On)
            {
                this.DrawRectangle(device,
                    colorPlayer,
                    rectX,
                    rectY,
                    rectW,
                    rectH,
                    1f);
            }
            #endregion
            #region circle
            if (csgo.GetValue<OnOff>("espDrawCircle") == OnOff.On)
            {
                Vector3[] circPoints = Geometry.Create3DFlatCircle(
                    player.Vector3,
                    32f + (player.IsSpotted ? 16f * GetColorMultiplier() : 0f),
                    32);
                Vector2[] scrCircPoints = Geometry.WorldToScreen(csgo.ViewMatrix, csgo.ScreenSize, circPoints);
                DrawLines(device, colorPlayer, 15f / distanceMeter, scrCircPoints);
            }
            #endregion
            #region distance
            if (csgo.GetValue<OnOff>("espDrawDistance") == OnOff.On)
            {
                string distString = Math.Round(distanceMeter, 0).ToString() + "m";
                RectangleF distSize = Control.MeasureString(this.Font, distString);
                //Draw text
                DrawText(device,
                    foreColor,
                    backColor,
                    rectX + rectW / 2f - distSize.Width / 2f,
                    rectY + rectH,
                    100f,
                    20f,
                    1f,
                    1f,
                    distString,
                    FactoryManager.GetFont("smallSegoe"));
            }
            #endregion
            #region highlight
            if (highlighted)
            {
                float size = (float)Math.Max(rectW + boxBorder, rectH + boxBorder) * 2f;
                Color color = (colorPlayer) * (0.5f + 0.5f * GetColorMultiplier());
                FillEllipse(
                    device,
                    color * 0.5f,
                    rectX + rectW / 2f,
                    rectY + rectH / 2f,
                    size,
                    size,
                    true);
                DrawEllipse(device,
                    color, //* (DateTime.Now.Millisecond % 1000f / 1000f),
                    rectX + rectW / 2f,
                    rectY + rectH / 2f,
                    size,
                    size,
                    true,
                    1.5f);
            }
            #endregion
            #region name
            if (csgo.GetValue<OnOff>("espDrawName") == OnOff.On)
            {
                ////Name
                //DrawText(device,
                //    foreColor,
                //    backColor,
                //    rectX + rectW,
                //    rectY - boxBorder,
                //    100f,
                //    20f,
                //    1f,
                //    1f,
                //    (player.IsDormant ? "[DORMANT] " : "") + player.Name,
                //    FactoryManager.GetFont("smallSegoe"));
                ////Info
                //string weaponInfo = "-";
                //if (player.WeaponIndex >= 0 && player.WeaponIndex < csgo.Entities.Length)
                //{
                //    if (csgo.Entities[player.WeaponIndex] != null)
                //        weaponInfo = csgo.Entities[player.WeaponIndex].Name;
                //}

                //string data = String.Format(
                //"Weapon: {0}",
                //    /*player.Health, player.Armor,*/ weaponInfo);

                //if (csgo.GetValue<OnOff>("espDrawDetails") == OnOff.On)
                //    data = String.Format("{0}\n" +
                //    "Balance: ${1}\n" +
                //    "Kills: {2}\n" +
                //    "Assists: {3}\n" +
                //    "Deaths: {4}\n" +
                //    "Score: {5}",
                //    data, player.Money, player.Kills, player.Assists, player.Deaths, player.Score);

                //DrawText(device,
                //    foreColor,
                //    backColor,
                //    rectX + rectW,
                //    rectY - boxBorder + 16f,
                //    100f,
                //    20f,
                //    1f,
                //    1f,
                //    data,
                //    FactoryManager.GetFont("tinySegoe"));
                string weaponInfo = "-";
                if (player.WeaponIndex >= 0 && player.WeaponIndex < csgo.Entities.Length)
                {
                    if (csgo.Entities[player.WeaponIndex] != null)
                        weaponInfo = csgo.Entities[player.WeaponIndex].Name;
                }
                string str = string.Format("{0}\n[{1}]", player.Name, weaponInfo);
                RectangleF size = Control.MeasureString(this.Font, str);
                //Name
                DrawText(device,
                    foreColor,
                    backColor,
                    rectX + rectW / 2f - size.Width / 2f,
                    rectY - 40,
                    100f,
                    20f,
                    2f,
                    2f,
                    str,
                    FactoryManager.GetFont("smallSegoe"));
            }
            #endregion
            #region health
            if (csgo.GetValue<OnOff>("espDrawHealth") == OnOff.On)
            {
                ////HP
                //FillRectangle(device,
                //        lifeBarBackground,
                //        rectX,
                //        rectY - lifeBarHeight * 3f,
                //        width, //rectW,
                //        lifeBarHeight /*10f*/);
                //FillRectangle(device,
                //        lifeBarForeground,
                //        rectX,
                //        rectY - lifeBarHeight * 3f,
                //    /*rectW*/ width / 100f * player.Health,
                //        lifeBarHeight /*10f*/);
                //DrawText(device,
                //    lifeBarForeground,
                //    backColor,
                //    rectX + width,
                //    rectY - lifeBarHeight * 3f - 2f,
                //    100f,
                //    20f,
                //    1f,
                //    1f,
                //    player.Health.ToString(),
                //    FactoryManager.GetFont("miniSegoe"));
                ////Armor
                //FillRectangle(device,
                //        lifeBarBackground,
                //        rectX,
                //        rectY - lifeBarHeight * 2f,
                //        width, //rectW,
                //        lifeBarHeight /*10f*/);
                //FillRectangle(device,
                //        viewColorOutline,
                //        rectX,
                //        rectY - lifeBarHeight * 2f,
                //    /*rectW*/ width / 100f * player.Armor,
                //        lifeBarHeight /*10f*/);
                //DrawText(device,
                //    viewColorOutline,
                //    backColor,
                //    rectX + width,
                //    rectY - lifeBarHeight * 2f - 2f,
                //    100f,
                //    20f,
                //    1f,
                //    1f,
                //    player.Armor.ToString(),
                //    FactoryManager.GetFont("miniSegoe"));
                //HP
                FillRectangle(device,
                        lifeBarBackground,
                        rectX - lifeBarWidth,
                        rectY,
                        lifeBarWidth, //rectW,
                        rectH /*10f*/);
                FillRectangle(device,
                        lifeBarForeground,
                        rectX - lifeBarWidth,
                        rectY,
                        lifeBarWidth,
                        rectH / 100f * player.Health /*10f*/);
                DrawText(device,
                    lifeBarForeground,
                    backColor,
                    rectX - lifeBarWidth,
                    rectY + rectH,
                    100f,
                    20f,
                    1f,
                    1f,
                    player.Health.ToString(),
                    FactoryManager.GetFont("miniSegoe"));
                //Armor
                FillRectangle(device,
                        lifeBarBackground,
                        rectX + rectW,
                        rectY,
                        lifeBarWidth, //rectW,
                        rectH /*10f*/);
                FillRectangle(device,
                        viewColorOutline,
                        rectX + rectW,
                        rectY,
                        lifeBarWidth,
                        rectH / 100f * player.Armor /*10f*/);
                DrawText(device,
                    viewColorOutline,
                    backColor,
                    rectX + rectW,
                    rectY + rectH,
                    100f,
                    20f,
                    1f,
                    1f,
                    player.Armor.ToString(),
                    FactoryManager.GetFont("miniSegoe"));
            }
            #endregion
            #region rank
            if (player.Rank > 0)
            {
                //Scaling
                float boxWidth = rectW, boxHeight = rectW * aspect;
                /*
                 * Args:
                 * 1 actual bitmap
                 * 2 destination-rectangle
                 * 3 opacity
                 * 4 interpolation mode
                 * 5 source-rectangle - easy access using rank as index
                 */
                if (ranksBmp != null)
                {
                    device.DrawBitmap(
                    ranksBmp,
                    new RectangleF(point.X - boxWidth / 2f, point.Y + 20f, boxWidth, boxHeight),
                    0.7f,
                    BitmapInterpolationMode.Linear,
                    new RectangleF(0f, 80f * (player.Rank - 1), 200f, 80f));
                    if (boxWidth > 50f)
                        DrawText(device, foreColor, point.X - boxWidth / 2f, point.Y + 20f + boxHeight + 4f, 200f, 100f, string.Format("MM-wins: {0}", player.Wins.ToString()), this.Font);
                }
            }
            #endregion
        }
        private void DrawEntity(WindowRenderTarget device, Player currentPlayer, Entity entity)
        {
            if (entity == null)
                return;
            if (entity.Address == currentPlayer.Address)
                return;
            if (!entity.IsValid())
                return;
            //if (!Geometry.PointSeesPoint(currentPlayer.Vector2, entity.Vector2, Player.FOV_DEGREE, currentPlayer.Yaw))
            //    return;

            Color brushColor = lifeBarForeground;
            Vector2 point = Geometry.WorldToScreen(csgo.ViewMatrix, csgo.ScreenSize, entity.Vector3);
            if (point == Vector2.Zero)
                return;
            Vector3 entPoint = entity.Vector3;
            float distance = Geometry.GetDistanceToPoint(currentPlayer.Vector3, entity.Vector3);
            float distanceMeter = currentPlayer.DistanceToOtherEntityInMetres(entity);
            float boxBorder = 6400f / distance;
            float scale = 0.5f / distanceMeter;
            float rectX = point.X, rectY = point.Y, rectW = 24f * scale, rectH = 24f * scale;

            switch (entity.ClassID)
            {
                case Data.Enums.ClassID.Weapon:
                    brushColor = Color.Green;
                    if (csgo.GetValue<YesNo>("espDrawWeapons") == YesNo.No)
                        return;
                    if (entity.OwnerEntity != -1)
                        return;
                    break;
                case Data.Enums.ClassID.Hostage:
                    brushColor = Color.LemonChiffon;
                    return;
                case Data.Enums.ClassID.C4:
                    if (currentPlayer.Index == entity.OwnerEntity)
                        return;
                    brushColor = Color.DarkRed;
                    //if (entity.OwnerEntity > -1 && entity.OwnerEntity < csgo.Players.Length - 1)
                    //{
                    //    point = Geometry.WorldToScreen(csgo.ViewMatrix, csgo.ScreenSize, csgo.Players[entity.OwnerEntity - 1].Skeleton.Spine4);
                    //    rectX = point.X;
                    //    rectY = point.Y;
                    //    FillEllipse(device, Color.DarkRed * 0.25f, point.X, point.Y, csgo.ScreenSize.Height / 2f, csgo.ScreenSize.Height / 2f, true);
                    //    entPoint = csgo.Players[entity.OwnerEntity - 1].Skeleton.Spine4;
                    //}
                    break;
                case Data.Enums.ClassID.PlantedC4:
                    brushColor = Color.DarkRed;
                    //FillEllipse(device, Color.DarkRed * 0.25f, point.X, point.Y, csgo.ScreenSize.Height / 2f, csgo.ScreenSize.Height / 2f, true);
                    break;
                default:
                    return;
            }
            if (entity.ClassID == Data.Enums.ClassID.Chicken || entity.ClassID == Data.Enums.ClassID.Hostage)
            {
                //Paint bones
                //List<Vector3> allBones = new List<Vector3>();
                //if (distanceMeter < 20)
                //    allBones.AddRange(entity.Skeleton.Arms);
                //allBones.AddRange(entity.Skeleton.Legs);
                //allBones.AddRange(entity.Skeleton.Spine);
                //allBones.Add(entity.Skeleton.Neck);
                //for (int i = allBones.Count - 1; i >= 0; i--)
                //    if (allBones[i] == Vector3.Zero)
                //        allBones.RemoveAt(i);

                //Vector2[] all = Geometry.WorldToScreen(csgo.ViewMatrix, csgo.ScreenSize, allBones.ToArray());
                //allBones.Clear();
                //allBones = null;

                //rectX = GetSmallestX(all);
                //rectW = GetBiggestX(all);
                //rectY = GetSmallestY(all);
                //rectH = GetBiggestY(all);
                //rectW -= rectX;
                //rectH -= rectY;
            }

            rectX -= boxBorder;
            rectY -= boxBorder;
            rectW += boxBorder * 2f;
            rectH += boxBorder * 2f;

            #region glow
            if (csgo.GetValue<YesNo>("espGlowEnabled") == YesNo.Yes)
            {
                if (csgo.GlowObjects != null)
                {
                    CSGOGameController controller = (CSGOGameController)Program.GameController;
                    int idx = controller.MemoryUpdater.GetGlowObjectByAddress((int)entity.Address);
                    if (idx != -1)
                    {
                        GlowObjectDefinition def = csgo.GlowObjects[idx];
                        def.a = 1f;// (float)(brushColor.A / 255f);
                        def.r = (float)(brushColor.R / 255f);
                        def.g = (float)(brushColor.G / 255f);
                        def.b = (float)(brushColor.B / 255f);
                        def.m_bRenderWhenOccluded = true;
                        def.m_bRenderWhenUnoccluded = true;
                        controller.MemoryUpdater.WriteGlowObject(def, idx);
                    }
                }
            }
            #endregion

            if (point != Vector2.Zero)
            {
                #region box
                if (csgo.GetValue<OnOff>("espDrawBox") == OnOff.On)
                {
                    //Draw box
                    DrawW2SBox(device,
                        brushColor,
                        entPoint - Vector3.UnitZ * 8,
                        16f,
                        16f,
                        entity.Yaw + 45f,
                        15f / distanceMeter);
                }
                #endregion

                #region distance
                if (csgo.GetValue<OnOff>("espDrawDistance") == OnOff.On)
                {
                    ////Draw text
                    DrawText(device,
                        foreColor,
                        backColor,
                        rectX,
                        rectY + rectH,
                        100f,
                        20f,
                        1f,
                        1f,
                        Math.Round(distanceMeter, 0).ToString() + "m",
                        FactoryManager.GetFont("smallSegoe"));
                }
                #endregion
                string name = entity.Name;
                if (
                    entity.ClassID != Data.Enums.ClassID.Chicken &&
                    entity.ClassID != Data.Enums.ClassID.Hostage &&
                    entity.ClassID != Data.Enums.ClassID.CSPlayer &&
                    entity.ClassID != Data.Enums.ClassID.Weapon &&
                    entity.ClassID != Data.Enums.ClassID.C4 &&
                    entity.ClassID != Data.Enums.ClassID.PlantedC4)
                    name = String.Format("{0} (ID#{1})", name, entity.ClassID);
                DrawText(device,
                    foreColor,
                    backColor,
                    rectX,
                    rectY - 20f,
                    100f,
                    20f,
                    1f,
                    1f,
                    name,
                    FactoryManager.GetFont("smallSegoe"));

            }
        }
        private void DrawDamage(WindowRenderTarget device, Player currentPlayer, Damage damage)
        {
            if (damage == null)
                return;
            if (!damage.Alive)
            {
                csgo.Damages.Remove(damage);
            }

            if (!Geometry.PointSeesPoint(currentPlayer.Vector2, damage.Position2D, Player.FOV_DEGREE, currentPlayer.Yaw))
                return;
            damage.Update();
            Vector2 point = Geometry.WorldToScreen(csgo.ViewMatrix, csgo.ScreenSize, (damage.Position));
            float distance = Geometry.GetDistanceToPoint(currentPlayer.Vector3, damage.Position);

            RectangleF size = Control.MeasureString(FactoryManager.GetFont("largeSegoe"), damage.Text);
            float max = Math.Max(size.Width, size.Height);
            float multiplier = damage.Multiplier;
            FillEllipse(device,
                lifeBarBackground * multiplier,
                point.X,
                point.Y,
                max,
                max,
                true);
            DrawText(device,
                colorT * multiplier,
                lifeBarBackground,
                point.X - size.Width / 2f,
                point.Y - size.Height / 2f,
                size.Width,
                size.Height,
                this.Theme.ShadowOffsetX,
                this.Theme.ShadowOffsetY,
                damage.Text,
                FactoryManager.GetFont("largeSegoe"));
        }
        private void Draw2DCircle(WindowRenderTarget device, Vector3 origin, float radius, int segments, Color color, float thickness, CSGOImplementation csgo)
        {
            Vector3[] circPoints = Geometry.Create3DFlatCircle(
                origin,
                radius,
                segments);
            Vector2[] scrCircPoints = Geometry.WorldToScreen(csgo.ViewMatrix, csgo.ScreenSize, circPoints);
            DrawLines(device, color, thickness, scrCircPoints);
        }
        #endregion

        #region HELP METHODS
        private Vector2[] CreateW2SFlatBox(Vector3 bottomCenter, int width, float degreeRotation)
        {

            return Geometry.WorldToScreen(
                csgo.ViewMatrix,
                csgo.ScreenSize,
                Geometry.Create3DFlatBox(
                    bottomCenter,
                    width,
                    degreeRotation)
                );
        }
        private void DrawW2SArrow(WindowRenderTarget device, Color color, Vector3 bottomCenter, float width, float length, float height, float degreeRotation, float strokeWidth = 1f)
        {
            Vector3[] pointsBottom = Geometry.Create3DFlatArrow(bottomCenter, width, length, degreeRotation);
            Vector3[] pointsUpper =
                Geometry.OffsetVectors(
                    Vector3.UnitZ * height,
                    Geometry.CopyVectors(pointsBottom)
                );

            Vector2[] pointsBottom2D = Geometry.WorldToScreen(csgo.ViewMatrix, csgo.ScreenSize, pointsBottom);
            Vector2[] pointsUpper2D = Geometry.WorldToScreen(csgo.ViewMatrix, csgo.ScreenSize, pointsUpper);

            this.DrawLines(device, color, strokeWidth, pointsBottom2D);
            this.DrawLines(device, color, strokeWidth, pointsUpper2D);
            this.ConnectLines(device, color, strokeWidth, pointsBottom2D, pointsUpper2D);

            foreach (Vector2 pnt in pointsBottom2D)
                if (pnt == Vector2.Zero)
                    return;
            this.FillPolygon(device,
                color,
                pointsBottom2D);
        }
        private void DrawW2SBox(WindowRenderTarget device, Color color, Vector3 bottomCenter, float width, float height, float degreeRotation, float strokeWidth = 1f)
        {
            Vector3[] pointsBottom = Geometry.Create3DFlatBox(bottomCenter, width, degreeRotation);
            Vector3[] pointsUpper =
                Geometry.OffsetVectors(
                    Vector3.UnitZ * height,
                    Geometry.CopyVectors(pointsBottom)
                );

            Vector2[] pointsBottom2D = Geometry.WorldToScreen(csgo.ViewMatrix, csgo.ScreenSize, pointsBottom);
            Vector2[] pointsUpper2D = Geometry.WorldToScreen(csgo.ViewMatrix, csgo.ScreenSize, pointsUpper);

            foreach (Vector2 pnt in pointsBottom2D)
                if (pnt == Vector2.Zero)
                    return;
            foreach (Vector2 pnt in pointsUpper2D)
                if (pnt == Vector2.Zero)
                    return;
            this.DrawLines(device, color, strokeWidth, pointsBottom2D);
            this.DrawLines(device, color, strokeWidth, pointsUpper2D);
            this.ConnectLines(device, color, strokeWidth, pointsBottom2D, pointsUpper2D);
        }
        private float GetBiggestX(Vector2[] coords)
        {
            float val = coords[0].X;
            foreach (Vector2 vec in coords)
                if (vec.X > val && vec != Vector2.Zero)
                    val = vec.X;
            return val;
        }
        private float GetBiggestY(Vector2[] coords)
        {
            float val = coords[0].Y;
            foreach (Vector2 vec in coords)
                if (vec.Y > val && vec != Vector2.Zero)
                    val = vec.Y;
            return val;
        }
        private float GetSmallestX(Vector2[] coords)
        {
            float val = coords[0].X;
            foreach (Vector2 vec in coords)
                if (vec.X < val && vec != Vector2.Zero)
                    val = vec.X;
            return val;
        }
        private float GetSmallestY(Vector2[] coords)
        {
            float val = coords[0].Y;
            foreach (Vector2 vec in coords)
                if (vec.Y < val && vec != Vector2.Zero)
                    val = vec.Y;
            return val;
        }
        private void DrawBones(WindowRenderTarget device, Color clr, Vector2[] bones, float thickness, Team team)
        {
            Vector2 last = bones[0];
            for (int i = 1; i < bones.Length; i++)
            {
                DrawLine(
                    device,
                    clr,
                    bones[i].X,
                    bones[i].Y,
                    last.X,
                    last.Y,
                    thickness);
                last = bones[i];
            }
        }
        #endregion
    }
}
