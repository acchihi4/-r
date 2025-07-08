using System;
using System.Collections.Generic;
using System.Linq;
using AssemblyCSharp.Mod.PickMob;

// Token: 0x0200001C RID: 28
internal class AttackBoss
{
	// Token: 0x06000096 RID: 150 RVA: 0x00009EAC File Offset: 0x000080AC
	public static void AutoBongTai()
	{
		for (int i = 0; i < global::Char.myCharz().arrItemBag.Length; i++)
		{
			Item item = global::Char.myCharz().arrItemBag[i];
			bool flag = item != null && (item.template.id == 454 || item.template.id == 921);
			if (flag)
			{
				Service.gI().useItem(0, 1, -1, item.template.id);
				break;
			}
		}
	}

	// Token: 0x06000097 RID: 151 RVA: 0x0000B228 File Offset: 0x00009428
	public static void UseBT()
	{
		bool flag = global::Char.myCharz().cHP < global::Char.myCharz().cHPFull / 5L || global::Char.myCharz().cMP < global::Char.myCharz().cMPFull / 5L || !global::Char.myCharz().isNhapThe;
		if (flag)
		{
			AttackBoss.AutoBongTai();
		}
	}

	// Token: 0x06000098 RID: 152 RVA: 0x0000B284 File Offset: 0x00009484
	public static void Update()
	{
		bool flag = AttackBoss.IsWaiting();
		if (!flag)
		{
			global::Char @char = global::Char.myCharz();
			bool flag2 = @char.statusMe == 14 || @char.cHP <= 0L;
			if (!flag2)
			{
				bool flag3 = GameCanvas.gameTick % 100 == 0;
				if (flag3)
				{
					AttackBoss.UseBT();
				}
				bool isPemBoss = AttackBoss.IsPemBoss;
				if (isPemBoss)
				{
					bool isCharge = @char.isCharge;
					if (isCharge)
					{
						AttackBoss.Wait(500);
						return;
					}
					bool flag4 = @char.charFocus != null;
					if (flag4)
					{
						bool flag5 = @char.skillInfoPaint() == null;
						if (flag5)
						{
							Skill skillAttack = AttackBoss.GetSkillAttack();
							bool flag6 = skillAttack != null && !skillAttack.paintCanNotUseSkill;
							if (flag6)
							{
								GameScr.gI().doSelectSkill(skillAttack, true);
								global::Char charFocus = @char.charFocus;
								bool flag7 = charFocus.cy > 200 && charFocus.cx > 50;
								if (flag7)
								{
									bool flag8 = Res.distance(charFocus.cx, charFocus.cy, @char.cx, @char.cy) > 75;
									if (flag8)
									{
										AttackBoss.Move(charFocus.cx - 24, charFocus.cy);
									}
								}
							}
						}
					}
				}
				AttackBoss.Wait(2000);
			}
		}
	}

	// Token: 0x06000099 RID: 153 RVA: 0x0000B3E0 File Offset: 0x000095E0
	private static void Move(int x, int y)
	{
		global::Char @char = global::Char.myCharz();
		bool flag = !Pk9rPickMob.IsVuotDiaHinh;
		if (flag)
		{
			@char.currentMovePoint = new MovePoint(x, y);
		}
		else
		{
			int[] pointYsdMax = AttackBoss.GetPointYsdMax(@char.cx, x);
			bool flag2 = pointYsdMax[1] >= y || (pointYsdMax[1] >= @char.cy && (@char.statusMe == 2 || @char.statusMe == 1));
			if (flag2)
			{
				pointYsdMax[0] = x;
				pointYsdMax[1] = y;
			}
			@char.currentMovePoint = new MovePoint(pointYsdMax[0], pointYsdMax[1]);
		}
	}

	// Token: 0x0600009A RID: 154 RVA: 0x0000A8C8 File Offset: 0x00008AC8
	private static int GetYsd(int xsd)
	{
		global::Char @char = global::Char.myCharz();
		int num = TileMap.pxh;
		int num2 = -1;
		for (int i = 24; i < TileMap.pxh; i += 24)
		{
			bool flag = TileMap.tileTypeAt(xsd, i, 2);
			if (flag)
			{
				int num3 = Res.abs(i - @char.cy);
				bool flag2 = num3 < num;
				if (flag2)
				{
					num = num3;
					num2 = i;
				}
			}
		}
		return num2;
	}

	// Token: 0x0600009B RID: 155 RVA: 0x0000B468 File Offset: 0x00009668
	public static void TeleportTo(int x, int y)
	{
		global::Char @char = global::Char.myCharz();
		@char.cx = x;
		@char.cy = y;
		Service.gI().charMove();
		bool flag = !ItemTime.isExistItem(4387);
		if (flag)
		{
			@char.cx = x;
			@char.cy = y;
			Service.gI().charMove();
			@char.cx = x;
			@char.cy = y;
			Service.gI().charMove();
		}
	}

	// Token: 0x0600009C RID: 156 RVA: 0x0000B4DC File Offset: 0x000096DC
	private static int[] GetPointYsdMax(int xStart, int xEnd)
	{
		int num = TileMap.pxh;
		int num2 = -1;
		bool flag = xStart > xEnd;
		if (flag)
		{
			for (int i = xEnd; i < xStart; i += 24)
			{
				int ysd = AttackBoss.GetYsd(i);
				bool flag2 = ysd < num;
				if (flag2)
				{
					num = ysd;
					num2 = i;
				}
			}
		}
		else
		{
			for (int j = xEnd; j > xStart; j -= 24)
			{
				int ysd2 = AttackBoss.GetYsd(j);
				bool flag3 = ysd2 < num;
				if (flag3)
				{
					num = ysd2;
					num2 = j;
				}
			}
		}
		return new int[] { num2, num };
	}

	// Token: 0x0600009D RID: 157 RVA: 0x0000B578 File Offset: 0x00009778
	private static Skill GetSkillAttack()
	{
		Skill skill = null;
		SkillTemplate skillTemplate = new SkillTemplate();
		foreach (sbyte b in AttackBoss.IdSkillsTanSat)
		{
			skillTemplate.id = b;
			Skill skill2 = global::Char.myCharz().getSkill(skillTemplate);
			bool flag = AttackBoss.IsSkillBetter(skill2, skill);
			if (flag)
			{
				skill = skill2;
			}
		}
		return skill;
	}

	// Token: 0x0600009E RID: 158 RVA: 0x0000B600 File Offset: 0x00009800
	private static bool IsSkillBetter(Skill SkillBetter, Skill skill)
	{
		bool flag = SkillBetter == null;
		bool flag2;
		if (flag)
		{
			flag2 = false;
		}
		else
		{
			bool flag3 = !AttackBoss.CanUseSkill(SkillBetter);
			if (flag3)
			{
				flag2 = false;
			}
			else
			{
				bool flag4 = (SkillBetter.template.id == 17 && skill.template.id == 2) || (SkillBetter.template.id == 9 && skill.template.id == 0);
				flag2 = skill == null || skill.coolDown < SkillBetter.coolDown || flag4;
			}
		}
		return flag2;
	}

	// Token: 0x0600009F RID: 159 RVA: 0x0000B688 File Offset: 0x00009888
	private static bool CanUseSkill(Skill skill)
	{
		bool flag = mSystem.currentTimeMillis() - skill.lastTimeUseThisSkill > (long)skill.coolDown;
		if (flag)
		{
			skill.paintCanNotUseSkill = false;
		}
		return (!skill.paintCanNotUseSkill || AttackBoss.IdSkillsMelee.Contains(skill.template.id)) && !AttackBoss.IdSkillsCanNotAttack.Contains(skill.template.id) && global::Char.myCharz().cMP >= (long)AttackBoss.GetManaUseSkill(skill);
	}

	// Token: 0x060000A0 RID: 160 RVA: 0x0000B70C File Offset: 0x0000990C
	private static int GetManaUseSkill(Skill skill)
	{
		bool flag = skill.template.manaUseType == 2;
		int num;
		if (flag)
		{
			num = 1;
		}
		else
		{
			bool flag2 = skill.template.manaUseType == 1;
			if (flag2)
			{
				num = skill.manaUse * (int)global::Char.myCharz().cMPFull / 100;
			}
			else
			{
				num = skill.manaUse;
			}
		}
		return num;
	}

	// Token: 0x060000A1 RID: 161 RVA: 0x0000404F File Offset: 0x0000224F
	private static void Wait(int time)
	{
		AttackBoss.IsWait = true;
		AttackBoss.TimeStartWait = mSystem.currentTimeMillis();
		AttackBoss.TimeWait = (long)time;
	}

	// Token: 0x060000A2 RID: 162 RVA: 0x0000B768 File Offset: 0x00009968
	private static bool IsWaiting()
	{
		bool flag = AttackBoss.IsWait && mSystem.currentTimeMillis() - AttackBoss.TimeStartWait >= AttackBoss.TimeWait;
		if (flag)
		{
			AttackBoss.IsWait = false;
		}
		return AttackBoss.IsWait;
	}

	// Token: 0x04000072 RID: 114
	private static readonly sbyte[] IdSkillsMelee = new sbyte[] { 0, 9, 2, 17, 4 };

	// Token: 0x04000073 RID: 115
	private static readonly sbyte[] IdSkillsCanNotAttack = new sbyte[] { 10, 11, 14, 23, 7 };

	// Token: 0x04000074 RID: 116
	private static readonly sbyte[] IdSkillsBase = new sbyte[] { 0, 2, 17, 4, 12, 13 };

	// Token: 0x04000075 RID: 117
	public static List<sbyte> IdSkillsTanSat = new List<sbyte>(AttackBoss.IdSkillsBase);

	// Token: 0x04000076 RID: 118
	public static bool IsPickingItems;

	// Token: 0x04000077 RID: 119
	private static bool IsWait;

	// Token: 0x04000078 RID: 120
	private static long TimeStartWait;

	// Token: 0x04000079 RID: 121
	private static long TimeWait;

	// Token: 0x0400007A RID: 122
	public static bool IsPemBoss = false;
}
using System;
using System.Collections.Generic;
using System.Linq;
using AssemblyCSharp.Mod.Xmap;

// Token: 0x0200000F RID: 15
internal class AutoBroly
{
	// Token: 0x0600004E RID: 78 RVA: 0x00003EA5 File Offset: 0x000020A5
	private static void Wait(int time)
	{
		AutoBroly.IsWait = true;
		AutoBroly.TimeStartWait = mSystem.currentTimeMillis();
		AutoBroly.TimeWait = (long)time;
	}

	// Token: 0x0600004F RID: 79 RVA: 0x00003EBE File Offset: 0x000020BE
	private static bool IsWaiting()
	{
		if (AutoBroly.IsWait && mSystem.currentTimeMillis() - AutoBroly.TimeStartWait >= AutoBroly.TimeWait)
		{
			AutoBroly.IsWait = false;
		}
		return AutoBroly.IsWait;
	}

	// Token: 0x06000050 RID: 80 RVA: 0x000092DC File Offset: 0x000074DC
	public static bool IsBoss()
	{
		for (int i = 0; i < GameScr.vCharInMap.size(); i++)
		{
			global::Char @char = (global::Char)GameScr.vCharInMap.elementAt(i);
			if (@char != null && @char.cName.Contains("Broly") && @char.cName.Contains("Super") && @char.cHPFull >= 16070777L)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000051 RID: 81 RVA: 0x00009350 File Offset: 0x00007550
	public static void SearchBoss()
	{
		int currentZone = TileMap.zoneID;
		int num = GameScr.gI().zones.Length;
		if (AutoBroly.IsBoss())
		{
			AutoBroly.visitedZones.Clear();
			return;
		}
		AutoBroly.visitedZones.Add(currentZone);
		List<int> list = (from z in Enumerable.Range(0, num)
			where z != currentZone && !AutoBroly.visitedZones.Contains(z)
			select z).ToList<int>();
		if (list.Count == 0)
		{
			AutoBroly.visitedZones.Clear();
			return;
		}
		int num2 = list[AutoBroly.random.Next(list.Count)];
		Service.gI().requestChangeZone(num2, -1);
	}

	// Token: 0x06000052 RID: 82 RVA: 0x000093F8 File Offset: 0x000075F8
	public static void FocusSuperBroly()
	{
		for (int i = 0; i < GameScr.vCharInMap.size(); i++)
		{
			global::Char @char = (global::Char)GameScr.vCharInMap.elementAt(i);
			if (@char != null && @char.cName.Contains("Broly") && @char.cName.Contains("Super") && @char.cHP > 0L && global::Char.myCharz().charFocus != @char)
			{
				global::Char.myCharz().npcFocus = null;
				global::Char.myCharz().mobFocus = null;
				global::Char.myCharz().charFocus = @char;
				return;
			}
		}
	}

	// Token: 0x06000053 RID: 83 RVA: 0x00009498 File Offset: 0x00007698
	public static void Update()
	{
		if (!AutoBroly.IsWaiting())
		{
			if (global::Char.myCharz().cHP <= 0L || global::Char.myCharz().meDead)
			{
				if (AutoBroly.IsBoss())
				{
					AutoBroly.Map = TileMap.mapID;
					AutoBroly.Khu = TileMap.zoneID;
				}
				Service.gI().returnTownFromDead();
				AutoBroly.Wait(3000);
				return;
			}
			if (AutoBroly.Map != -1 && AutoBroly.Khu != -1 && TileMap.mapID == AutoBroly.Map && TileMap.zoneID == AutoBroly.Khu && !AutoBroly.IsBoss())
			{
				AutoBroly.Map = -1;
				AutoBroly.Khu = -1;
			}
			if (AutoBroly.IsBoss())
			{
				if (DataAccount.Type > 1)
				{
					AutoBroly.Map = TileMap.mapID;
					AutoBroly.Khu = TileMap.zoneID;
				}
				AutoBroly.TrangThai = "SP: " + TileMap.mapNames[TileMap.mapID].ToString() + " - " + TileMap.zoneID.ToString();
				if (AutoBroly.visitedZones.Count > 0)
				{
					AutoBroly.visitedZones.Clear();
				}
			}
			else
			{
				AutoBroly.TrangThai = "Không có thông tin ";
			}
			if (AutoBroly.Map != -1 && TileMap.mapID != AutoBroly.Map && !Pk9rXmap.IsXmapRunning)
			{
				XmapController.StartRunToMapId(AutoBroly.Map);
				AutoBroly.Wait(3000);
				return;
			}
			if (TileMap.mapID == AutoBroly.Map && TileMap.zoneID != AutoBroly.Khu && AutoBroly.Khu != -1)
			{
				Service.gI().requestChangeZone(AutoBroly.Khu, -1);
				AutoBroly.Wait(2000);
				return;
			}
			if (TileMap.mapID == AutoBroly.Map && TileMap.zoneID == AutoBroly.Khu && AutoBroly.IsBoss())
			{
				AutoBroly.FocusSuperBroly();
			}
			if (!AutoBroly.IsBoss() && AutoBroly.isDoKhu)
			{
				AutoBroly.SearchBoss();
				AutoBroly.Wait(2000);
				return;
			}
			if (DataAccount.Type == 1)
			{
				if (AutoBroly.NhayNe == 0 && AutoBroly.IsBoss())
				{
					AutoBroly.NhayNe = 1;
					AutoBroly.NhayCuoiMap();
					AutoBroly.Wait(1000);
					return;
				}
				if (!AutoBroly.IsBoss() && AutoBroly.NhayNe == 1)
				{
					AutoBroly.NhayNe = 0;
				}
			}
			if (DataAccount.Type == 3)
			{
				if (AutoBroly.NhayNe == 0 && !AutoBroly.IsBoss())
				{
					AutoBroly.NhayNe = 1;
					AutoBroly.NhayCuoiMap();
					AutoBroly.Wait(1000);
					return;
				}
				if (!AutoBroly.IsBoss() && AutoBroly.NhayNe == 1)
				{
					AutoBroly.NhayNe = 0;
				}
			}
			AutoBroly.AutoKichSP();
			AutoBroly.Wait(500);
		}
	}

	// Token: 0x06000054 RID: 84 RVA: 0x00003EEC File Offset: 0x000020EC
	public static void NhayCuoiMap()
	{
		if (GameScr.getX(2) > 0 && GameScr.getY(2) > 0)
		{
			KsSupper.TelePortTo(GameScr.getX(2) - 50, GameScr.getY(2));
		}
	}

	// Token: 0x06000055 RID: 85 RVA: 0x000096E8 File Offset: 0x000078E8
	public static bool IsBroly()
	{
		for (int i = 0; i < GameScr.vCharInMap.size(); i++)
		{
			global::Char @char = (global::Char)GameScr.vCharInMap.elementAt(i);
			if (@char != null && @char.cName.Contains("Broly") && !@char.cName.Contains("Super"))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000056 RID: 86 RVA: 0x0000974C File Offset: 0x0000794C
	public static void Painting(mGraphics g)
	{
		string text = TileMap.mapNames[TileMap.mapID];
		string text2 = " - " + TileMap.zoneID.ToString();
		string text3 = NinjaUtil.getMoneys(global::Char.myCharz().cHP).ToString() + " / " + NinjaUtil.getMoneys(global::Char.myCharz().cHPFull).ToString();
		string text4 = NinjaUtil.getMoneys(global::Char.myCharz().cMP).ToString() + " / " + NinjaUtil.getMoneys(global::Char.myCharz().cMPFull).ToString();
		if (AutoBroly.IsBoss())
		{
			for (int i = 0; i < GameScr.vCharInMap.size(); i++)
			{
				global::Char @char = (global::Char)GameScr.vCharInMap.elementAt(i);
				if (@char != null && @char.cName.Contains("Broly") && @char.cName.Contains("Super") && @char.cHPFull >= 16070777L)
				{
					string text5 = string.Concat(new string[]
					{
						@char.cName,
						" [ ",
						NinjaUtil.getMoneys(@char.cHP).ToString(),
						" / ",
						NinjaUtil.getMoneys(@char.cHPFull).ToString(),
						" ]"
					});
					mFont.tahoma_7b_yellow.drawString(g, text5, 20, GameCanvas.h - (GameCanvas.h - GameCanvas.h / 3), 0);
				}
			}
		}
		if (AutoBroly.IsBroly())
		{
			for (int j = 0; j < GameScr.vCharInMap.size(); j++)
			{
				global::Char char2 = (global::Char)GameScr.vCharInMap.elementAt(j);
				if (char2 != null && char2.cName.Contains("Broly") && !char2.cName.Contains("Super"))
				{
					string text6 = string.Concat(new string[]
					{
						char2.cName,
						" [ ",
						NinjaUtil.getMoneys(char2.cHP).ToString(),
						" / ",
						NinjaUtil.getMoneys(char2.cHPFull).ToString(),
						" ]"
					});
					mFont.tahoma_7b_white.drawString(g, text6, 20, GameCanvas.h - (GameCanvas.h - GameCanvas.h / 3), 0);
				}
			}
		}
		mFont.tahoma_7b_white.drawString(g, "HP: " + text3, 30, GameCanvas.h - (GameCanvas.h - 25), 0);
		mFont.tahoma_7b_white.drawString(g, "MP: " + text4, 30, GameCanvas.h - (GameCanvas.h - 35), 0);
		mFont.tahoma_7b_white.drawString(g, text + " " + text2 + " ", 30, GameCanvas.h - (GameCanvas.h - 10), 0);
		mFont.tahoma_7b_white.drawString(g, AutoBroly.Map.ToString() + " " + AutoBroly.Khu.ToString() + " ", GameCanvas.w - 30, GameCanvas.h - (GameCanvas.h - 10), 0);
	}

	// Token: 0x06000059 RID: 89 RVA: 0x00003F60 File Offset: 0x00002160
	private static int MyMax(int a, int b)
	{
		if (a <= b)
		{
			return b;
		}
		return a;
	}

	// Token: 0x0600005A RID: 90 RVA: 0x00003F69 File Offset: 0x00002169
	private static int MyMin(int a, int b)
	{
		if (a >= b)
		{
			return b;
		}
		return a;
	}

	// Token: 0x0600005B RID: 91 RVA: 0x00009A80 File Offset: 0x00007C80
	private static double MySqrt(double x)
	{
		if (x < 0.0)
		{
			return double.NaN;
		}
		double num = x;
		double num2;
		do
		{
			num2 = num;
			num = (num + x / num) / 2.0;
		}
		while (global::System.Math.Abs(num - num2) > 1E-07);
		return num;
	}

	// Token: 0x0600005C RID: 92 RVA: 0x00009ACC File Offset: 0x00007CCC
	public static void AutoKichSP()
	{
		if (DataAccount.Type != 1)
		{
			return;
		}
		global::Char @char = global::Char.myCharz();
		global::Char char2 = null;
		for (int i = 0; i < GameScr.vCharInMap.size(); i++)
		{
			global::Char char3 = (global::Char)GameScr.vCharInMap.elementAt(i);
			if (char3 != null && char3.cName.Contains("Broly") && !char3.cName.Contains("Super"))
			{
				char2 = char3;
				break;
			}
		}
		if (char2 == null)
		{
			return;
		}
		int num = @char.cx - char2.cx;
		int num2 = @char.cy - char2.cy;
		int num3 = (int)AutoBroly.MySqrt((double)(num * num + num2 * num2));
		long num4 = mSystem.currentTimeMillis();
		int num5 = 0;
		int num6 = 3840;
		int num7 = 100;
		int num8 = 10;
		if (num3 <= num7)
		{
			if (num4 - AutoBroly.lastAutoKichSPTime >= 500L)
			{
				int num9 = @char.cx;
				if (@char.cx <= num5 + num8 && @char.cx <= char2.cx)
				{
					num9 = char2.cx + num7;
					if (num9 > num6 - num8)
					{
						num9 = num6 - num8;
					}
				}
				else if (@char.cx >= num6 - num8 && @char.cx >= char2.cx)
				{
					num9 = char2.cx - num7;
					if (num9 < num5 + num8)
					{
						num9 = num5 + num8;
					}
				}
				else if (@char.cx < char2.cx)
				{
					num9 = char2.cx - num7;
					if (num9 < num5 + num8)
					{
						num9 = num5 + num8;
					}
				}
				else if (@char.cx > char2.cx)
				{
					num9 = char2.cx + num7;
					if (num9 > num6 - num8)
					{
						num9 = num6 - num8;
					}
				}
				KsSupper.TelePortTo(num9, @char.cy);
				AutoBroly.lastAutoKichSPTime = num4;
				return;
			}
		}
		else
		{
			AutoBroly.lastAutoKichSPTime = num4;
		}
	}

	// Token: 0x0400004D RID: 77
	public static string TrangThai = "Không có thông tin";

	// Token: 0x0400004E RID: 78
	public static int Map = -1;

	// Token: 0x0400004F RID: 79
	public static int Khu = -1;

	// Token: 0x04000050 RID: 80
	private static bool IsWait;

	// Token: 0x04000051 RID: 81
	private static long TimeStartWait;

	// Token: 0x04000052 RID: 82
	private static long TimeWait;

	// Token: 0x04000053 RID: 83
	public static bool isDoKhu = false;

	// Token: 0x04000054 RID: 84
	private static HashSet<int> visitedZones = new HashSet<int>();

	// Token: 0x04000055 RID: 85
	private static Random random = new Random();

	// Token: 0x04000056 RID: 86
	public static int NhayNe = 0;

	// Token: 0x04000057 RID: 87
	private static long lastAutoKichSPTime = 0L;
}
using System;

// Token: 0x02000033 RID: 51
internal class AutoOut
{
	// Token: 0x060002B8 RID: 696 RVA: 0x0002C168 File Offset: 0x0002A368
	public static void TimeOut()
	{
		bool flag = !AutoOut.meow;
		if (flag)
		{
			Random random = new Random();
			AutoOut.timechecklag = random.Next(AutoOut.Min, AutoOut.Max);
			AutoOut.meow = true;
		}
		bool flag2 = mSystem.currentTimeMillis() - AutoOut.thoigian >= 1000L;
		if (flag2)
		{
			AutoOut.checklag++;
		}
		bool flag3 = AutoOut.checklag >= AutoOut.timechecklag;
		if (flag3)
		{
			GameCanvas.startOKDlg("Let him cooooook !!!");
			Main.exit();
		}
	}

	// Token: 0x0400039A RID: 922
	public static bool meow = false;

	// Token: 0x0400039B RID: 923
	public static int checklag;

	// Token: 0x0400039C RID: 924
	public static int timechecklag;

	// Token: 0x0400039D RID: 925
	public static long thoigian;

	// Token: 0x0400039E RID: 926
	public static int Min = 360000;

	// Token: 0x0400039F RID: 927
	public static int Max = 540000;
}
using System;
using System.IO;

// Token: 0x02000011 RID: 17
internal class ChucNangPhu
{
	// Token: 0x0600005F RID: 95 RVA: 0x00003F8D File Offset: 0x0000218D
	private static void Wait(int time)
	{
		ChucNangPhu.IsWait = true;
		ChucNangPhu.TimeStartWait = mSystem.currentTimeMillis();
		ChucNangPhu.TimeWait = (long)time;
	}

	// Token: 0x06000060 RID: 96 RVA: 0x00009C90 File Offset: 0x00007E90
	private static bool IsWaiting()
	{
		bool flag = ChucNangPhu.IsWait && mSystem.currentTimeMillis() - ChucNangPhu.TimeStartWait >= ChucNangPhu.TimeWait;
		if (flag)
		{
			ChucNangPhu.IsWait = false;
		}
		return ChucNangPhu.IsWait;
	}

	// Token: 0x06000061 RID: 97 RVA: 0x00009CD4 File Offset: 0x00007ED4
	public static void AnBoHuyet()
	{
		bool flag = ItemTime.isExistItem(2755);
		if (!flag)
		{
			for (int i = 0; i < global::Char.myCharz().arrItemBag.Length; i++)
			{
				Item item = global::Char.myCharz().arrItemBag[i];
				bool flag2 = item != null && item.template.id == 382;
				if (flag2)
				{
					Service.gI().useItem(0, 1, -1, item.template.id);
					break;
				}
			}
		}
	}

	// Token: 0x06000062 RID: 98 RVA: 0x00009D58 File Offset: 0x00007F58
	public static void UseTDLT()
	{
		for (int i = 0; i < global::Char.myCharz().arrItemBag.Length; i++)
		{
			Item item = global::Char.myCharz().arrItemBag[i];
			bool flag = item != null && item.template.id == 521;
			if (flag)
			{
				Service.gI().useItem(0, 1, -1, item.template.id);
			}
		}
	}

	// Token: 0x06000063 RID: 99 RVA: 0x00009DC8 File Offset: 0x00007FC8
	public static void AutoTdlt()
	{
		bool flag = File.Exists(ChucNangPhu.tdlt) && !ItemTime.isExistItem(4387);
		if (flag)
		{
			ChucNangPhu.UseTDLT();
		}
		bool flag2 = !File.Exists(ChucNangPhu.tdlt) && ItemTime.isExistItem(4387);
		if (flag2)
		{
			ChucNangPhu.UseTDLT();
		}
	}

	// Token: 0x06000064 RID: 100 RVA: 0x00009E24 File Offset: 0x00008024
	public static bool IsBoss()
	{
		for (int i = 0; i < GameScr.vCharInMap.size(); i++)
		{
			global::Char @char = (global::Char)GameScr.vCharInMap.elementAt(i);
			bool flag = @char != null && @char.cName.Contains("Broly") && @char.cName.Contains("Super") && @char.cHPFull >= 16070777L;
			if (flag)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000065 RID: 101 RVA: 0x00009EAC File Offset: 0x000080AC
	public static void AutoBongTai()
	{
		for (int i = 0; i < global::Char.myCharz().arrItemBag.Length; i++)
		{
			Item item = global::Char.myCharz().arrItemBag[i];
			bool flag = item != null && (item.template.id == 454 || item.template.id == 921);
			if (flag)
			{
				Service.gI().useItem(0, 1, -1, item.template.id);
				break;
			}
		}
	}

	// Token: 0x06000066 RID: 102 RVA: 0x00009F34 File Offset: 0x00008134
	public static void Update()
	{
		bool flag = GameCanvas.gameTick % 200 == 0;
		if (flag)
		{
			ChucNangPhu.AutoTdlt();
		}
		bool flag2 = GameCanvas.gameTick % 20 == 0 && TileMap.mapID == global::Char.myCharz().cgender + 21;
		if (flag2)
		{
			KsSupper.autoitem();
		}
		bool flag3 = global::Char.myCharz().cspeed != 8;
		if (flag3)
		{
			global::Char.myCharz().cspeed = 8;
		}
	}

	// Token: 0x04000059 RID: 89
	private static bool IsWait;

	// Token: 0x0400005A RID: 90
	private static long TimeStartWait;

	// Token: 0x0400005B RID: 91
	private static long TimeWait;

	// Token: 0x0400005C RID: 92
	public static string tdlt = "Nro_244_Data/Resources/tdlt";
}
using System;

// Token: 0x02000012 RID: 18
internal class ChucNangPhu2
{
	// Token: 0x06000069 RID: 105 RVA: 0x00003FB3 File Offset: 0x000021B3
	private static void Wait(int time)
	{
		ChucNangPhu2.IsWait = true;
		ChucNangPhu2.TimeStartWait = mSystem.currentTimeMillis();
		ChucNangPhu2.TimeWait = (long)time;
	}

	// Token: 0x0600006A RID: 106 RVA: 0x00009FAC File Offset: 0x000081AC
	private static bool IsWaiting()
	{
		bool flag = ChucNangPhu2.IsWait && mSystem.currentTimeMillis() - ChucNangPhu2.TimeStartWait >= ChucNangPhu2.TimeWait;
		if (flag)
		{
			ChucNangPhu2.IsWait = false;
		}
		return ChucNangPhu2.IsWait;
	}

	// Token: 0x0600006B RID: 107 RVA: 0x00009FF0 File Offset: 0x000081F0
	public static bool IsBroly()
	{
		for (int i = 0; i < GameScr.vCharInMap.size(); i++)
		{
			global::Char @char = (global::Char)GameScr.vCharInMap.elementAt(i);
			bool flag = @char != null && @char.cName.Contains("Broly") && !@char.cName.Contains("Super") && Res.abs(@char.cx - global::Char.myCharz().cx) < 100;
			if (flag)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600006C RID: 108 RVA: 0x0000A080 File Offset: 0x00008280
	public static void Update()
	{
		bool flag = ChucNangPhu2.IsWaiting();
		if (!flag)
		{
			bool flag2 = !global::Char.myCharz().isNhapThe;
			if (flag2)
			{
				ChucNangPhu.AutoBongTai();
				ChucNangPhu2.Wait(3000);
			}
			else
			{
				ChucNangPhu2.Wait(500);
			}
		}
	}

	// Token: 0x0400005D RID: 93
	private static bool IsWait;

	// Token: 0x0400005E RID: 94
	private static long TimeStartWait;

	// Token: 0x0400005F RID: 95
	private static long TimeWait;
}
using System;
using System.IO;

// Token: 0x02000013 RID: 19
internal class ChucNangPhu3
{
	// Token: 0x0600006E RID: 110 RVA: 0x00003FCD File Offset: 0x000021CD
	private static void Wait(int time)
	{
		ChucNangPhu3.IsWait = true;
		ChucNangPhu3.TimeStartWait = mSystem.currentTimeMillis();
		ChucNangPhu3.TimeWait = (long)time;
	}

	// Token: 0x0600006F RID: 111 RVA: 0x0000A0CC File Offset: 0x000082CC
	private static bool IsWaiting()
	{
		bool flag = ChucNangPhu3.IsWait && mSystem.currentTimeMillis() - ChucNangPhu3.TimeStartWait >= ChucNangPhu3.TimeWait;
		if (flag)
		{
			ChucNangPhu3.IsWait = false;
		}
		return ChucNangPhu3.IsWait;
	}

	// Token: 0x06000070 RID: 112 RVA: 0x00009FF0 File Offset: 0x000081F0
	public static bool IsBroly()
	{
		for (int i = 0; i < GameScr.vCharInMap.size(); i++)
		{
			global::Char @char = (global::Char)GameScr.vCharInMap.elementAt(i);
			bool flag = @char != null && @char.cName.Contains("Broly") && !@char.cName.Contains("Super") && Res.abs(@char.cx - global::Char.myCharz().cx) < 100;
			if (flag)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000071 RID: 113 RVA: 0x0000A110 File Offset: 0x00008310
	public static void Update()
	{
		bool flag = ChucNangPhu3.IsWaiting();
		if (!flag)
		{
			bool flag2 = ChucNangPhu3.IsBroly() && DataAccount.Type > 1;
			if (flag2)
			{
				ChucNangPhu3.NhayCuoiMap();
				ChucNangPhu3.Wait(3000);
			}
			else
			{
				bool flag3 = DataAccount.Type == 3 && File.Exists("Nro_244_Data//Resources//dokhu");
				if (flag3)
				{
					bool flag4 = !DovaBaoKhu.IsBoss();
					if (flag4)
					{
						int cy = global::Char.myCharz().cy;
						global::Char.myCharz().cy = cy - 50;
						Service.gI().charMove();
						ChucNangPhu3.Wait(10000);
						return;
					}
				}
				ChucNangPhu3.Wait(500);
			}
		}
	}

	// Token: 0x06000072 RID: 114 RVA: 0x0000A1C0 File Offset: 0x000083C0
	public static void NhayCuoiMap()
	{
		bool flag = GameScr.getX(0) > 0 && GameScr.getY(0) > 0;
		if (flag)
		{
			KsSupper.TelePortTo(GameScr.getX(0) + 50, GameScr.getY(0));
		}
	}

	// Token: 0x04000060 RID: 96
	private static bool IsWait;

	// Token: 0x04000061 RID: 97
	private static long TimeStartWait;

	// Token: 0x04000062 RID: 98
	private static long TimeWait;
}
using System;

// Token: 0x02000014 RID: 20
internal class ChucNangPhu4
{
	// Token: 0x06000074 RID: 116 RVA: 0x00003FE7 File Offset: 0x000021E7
	private static void Wait(int time)
	{
		ChucNangPhu4.IsWait = true;
		ChucNangPhu4.TimeStartWait = mSystem.currentTimeMillis();
		ChucNangPhu4.TimeWait = (long)time;
	}

	// Token: 0x06000075 RID: 117 RVA: 0x0000A200 File Offset: 0x00008400
	private static bool IsWaiting()
	{
		bool flag = ChucNangPhu4.IsWait && mSystem.currentTimeMillis() - ChucNangPhu4.TimeStartWait >= ChucNangPhu4.TimeWait;
		if (flag)
		{
			ChucNangPhu4.IsWait = false;
		}
		return ChucNangPhu4.IsWait;
	}

	// Token: 0x06000076 RID: 118 RVA: 0x0000A244 File Offset: 0x00008444
	public static void Update()
	{
		bool flag = ChucNangPhu4.IsWaiting();
		if (!flag)
		{
			bool flag2 = TileMap.mapID != global::Char.myCharz().cgender + 21;
			if (flag2)
			{
				Service.gI().openUIZone();
				ChucNangPhu4.Wait(1000);
			}
		}
	}

	// Token: 0x04000063 RID: 99
	private static bool IsWait;

	// Token: 0x04000064 RID: 100
	private static long TimeStartWait;

	// Token: 0x04000065 RID: 101
	private static long TimeWait;
}
using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

// Token: 0x02000086 RID: 134
public class DataAccount
{
	// Token: 0x0600063A RID: 1594 RVA: 0x0006D590 File Offset: 0x0006B790
	public static void Doc()
	{
		try
		{
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			bool flag = commandLineArgs.Length == 0;
			if (!flag)
			{
				string text = null;
				string text2 = null;
				string text3 = null;
				string text4 = null;
				string text5 = null;
				string text6 = null;
				string text7 = null;
				for (int i = 0; i < commandLineArgs.Length; i++)
				{
					string text8 = commandLineArgs[i];
					string text9 = text8;
					uint num = <PrivateImplementationDetails>.ComputeStringHash(text9);
					if (num <= 1405861912U)
					{
						if (num != 915181618U)
						{
							if (num != 1135419332U)
							{
								if (num == 1405861912U)
								{
									if (text9 == "--acc")
									{
										bool flag2 = i + 1 < commandLineArgs.Length;
										if (flag2)
										{
											text = commandLineArgs[i + 1];
											i++;
										}
									}
								}
							}
							else if (text9 == "--server")
							{
								bool flag3 = i + 1 < commandLineArgs.Length;
								if (flag3)
								{
									text3 = commandLineArgs[i + 1];
									i++;
								}
							}
						}
						else if (text9 == "--pass")
						{
							bool flag4 = i + 1 < commandLineArgs.Length;
							if (flag4)
							{
								text2 = commandLineArgs[i + 1];
								i++;
							}
						}
					}
					else if (num <= 2635071343U)
					{
						if (num != 2357181198U)
						{
							if (num == 2635071343U)
							{
								if (text9 == "--prx")
								{
									bool flag5 = i + 1 < commandLineArgs.Length;
									if (flag5)
									{
										text5 = commandLineArgs[i + 1];
										i++;
									}
								}
							}
						}
						else if (text9 == "--team")
						{
							bool flag6 = i + 1 < commandLineArgs.Length;
							if (flag6)
							{
								text7 = commandLineArgs[i + 1];
								i++;
							}
						}
					}
					else if (num != 4071210487U)
					{
						if (num == 4142942274U)
						{
							if (text9 == "--id")
							{
								bool flag7 = i + 1 < commandLineArgs.Length;
								if (flag7)
								{
									text4 = commandLineArgs[i + 1];
									i++;
								}
							}
						}
					}
					else if (text9 == "--type")
					{
						bool flag8 = i + 1 < commandLineArgs.Length;
						if (flag8)
						{
							text6 = commandLineArgs[i + 1];
							i++;
						}
					}
				}
				DataAccount.Account = text;
				DataAccount.PassWord = text2;
				DataAccount.Server = int.Parse(text3) - 1;
				DataAccount.Team = int.Parse(text7);
				DataAccount.Type = int.Parse(text6.Split(new char[] { '.' })[0]);
				DataAccount.Proxy = text5;
				DataAccount.ID = int.Parse(text4);
				ThreadStart threadStart;
				if ((threadStart = DataAccount.<>O.<0>__Login) == null)
				{
					threadStart = (DataAccount.<>O.<0>__Login = new ThreadStart(DataAccount.Login));
				}
				new Thread(threadStart).Start();
			}
		}
		catch
		{
			DataAccount.startOKDlgWithStar("");
		}
	}

	// Token: 0x0600063B RID: 1595 RVA: 0x0006D89C File Offset: 0x0006BA9C
	public static void startOKDlgWithStar(string message)
	{
		StringBuilder stringBuilder = new StringBuilder();
		string[] array = new string[]
		{
			"................................:-....:-==**=:..................:...................................", ".................................=:.        ..-+=-:..............=-:................................", ".................................-=.            ..-+-............=-:=-..............................", "..................................=:              ...++:.........--..-+.............................", "..................................-+.                 .-+-:......=-.  .*-...........................", "...................................*:                   .:==:....=:    .==..........................", "...................................-+.                    ..-=:..+.     .==.........................", "....................::::--------::::=.                       .::-+.       -+:.......................", "..............:====--:::..............                         .=:         :+:......................", "...........--:....                                             ...         .-=.....::...............",
			"..........:+-.                                       .:.....                .=-....=+:..............", "............:+=.                                     ..::::...               .*:..:=.+-.............", "..............:+:.                                    ...:...:..              .=.-*. .*:............", "...............:=-.                                      ........              :==.   -=............", ".................-+.                                       ...  .             .:..    :=:...........", ".................:::.   .....                                                         .=:...........", "...........:-=*+:.  ..::::.....                                                       .=:...........", ".......:=+=:..       ...:....                                                         .+:...........", ".....-+-..             ......                                                         .=:...........", "...:=..                                                                               :=............",
			"...:-=+=:.                                              ........                      --............", "........:=+:.                                    ......:::::::::::........            -:-+*++===-...", "...........:+=.                         ......::::::::::::::::::::::::::::::....    .-=-::::=*-.....", ".............:-.                    .....::::::::::::::::::::-------:::::::::::......::::-+=:.......", "...........:=-.                 ....:::::::::::::::::-=*##%%%%%%%%%%%%##*=::::::::.::::-*-..........", "...........+-.              ....::::::::::::::-=*##%%%%%%%%%%%%%%%%%%%%%%%%%#+-:::::::=+:...........", ".........:*:             ...:::::::::::::-+*%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%*-:::+=.............", "........:+:           ...:::::::::::-+*#%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%#---..............", "........=-.        ...:::::::::-+#%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%+...............", ".......-=. .--:.  ..:::::-=*#%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%*:..............",
			".......*::+=::+  ..:::=*#%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%*:..............", ".......==-...-*..::-#%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%+...............", ".......-:....-+.::+#%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%+...............", ".............:*::*:-*%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%*:..............", ".............:#:=-..-#%%%%%%%#*++====+++***########%%%%%%%%%%%%%%%%%%%%%%%%####*=:..:-..............", "..............-*=:...*%#**:..:.......................................................+..............", "...............:-....:=-*:..................................:....::.................-+..............", "......................:+++...................................:+**:.................:+:..............", "........................=#-.......................................................:+:...............", ".........................-**++++-...............................................-+-.................",
			"..........................=*+*-:*#+-:........................................-#*:...................", "..........................:+=:+=*=#@@@%%@%%%%%%%%%###*++====----------:::-=*%%%#-...................", "...........................:...-:.:+%@@@@@@@@@@@@@@@@@@@@@@@@@@@@%%@@@@%@@@@@%=:....................", "...................................:#@@@@@@@@@@@@@@@@@@@@@@@@@@@%%@@@@@@@@@%*:......................", "..................................=%@@@@@@@@@@@@@@@@@@@@@@@@@@@@%%%%%%%%@%#:.......:=+:.............", ".................................=%@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@%%@%*:....=+==:+=:...........", "................................:%@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@%%@@%*:...-##+:=*-...........", "...............................:#@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@%%@@@%+...=*++-=+-...........", "..............................:*%@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@%%@@@@%-..-+-::.:=...........", "..............................-#@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@#@@@@@@#::*.:=..--...........",
			"................-=------=---::-%@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@%%@@@@@@%-:+.--..::..........."
		};
		foreach (string text in array)
		{
			stringBuilder.Append(text).Append("\n");
		}
		string text2 = message + "\n" + stringBuilder.ToString();
		GameCanvas.startOKDlg(text2, true);
	}

	// Token: 0x0600063C RID: 1596 RVA: 0x0006DAC4 File Offset: 0x0006BCC4
	public static void Login()
	{
		Thread.Sleep(1000);
		for (;;)
		{
			try
			{
				bool flag = string.IsNullOrEmpty(global::Char.myCharz().cName);
				if (flag)
				{
					Thread.Sleep(1000);
					while (!ServerListScreen.loadScreen)
					{
						Thread.Sleep(10);
					}
					Thread.Sleep(500);
					ServerListScreen.ipSelect = DataAccount.Server;
					bool flag2 = ServerListScreen.nameServer[ServerListScreen.ipSelect].Replace(" ", "") != DataAccount.Server.ToString();
					if (flag2)
					{
						GameCanvas.serverScreen.selectServer();
						while (!ServerListScreen.loadScreen)
						{
							Thread.Sleep(10);
						}
						while (!Session_ME.gI().isConnected())
						{
							Thread.Sleep(100);
						}
						Thread.Sleep(100);
						while (!ServerListScreen.loadScreen)
						{
							Thread.Sleep(10);
						}
					}
					Thread.Sleep(1000);
					GameCanvas.serverScreen.perform(3, null);
					Thread.Sleep(30000);
				}
			}
			catch
			{
			}
			Thread.Sleep(5000);
		}
	}

	// Token: 0x04000B60 RID: 2912
	public static string Account;

	// Token: 0x04000B61 RID: 2913
	public static string PassWord;

	// Token: 0x04000B62 RID: 2914
	public static int Server;

	// Token: 0x04000B63 RID: 2915
	public static int Type;

	// Token: 0x04000B64 RID: 2916
	public static string Proxy;

	// Token: 0x04000B65 RID: 2917
	public static int ID;

	// Token: 0x04000B66 RID: 2918
	public static int Team;

	// Token: 0x02000087 RID: 135
	[CompilerGenerated]
	private static class <>O
	{
		// Token: 0x04000B67 RID: 2919
		public static ThreadStart <0>__Login;
	}
}
using System;
using System.IO;
using System.Threading;
using AssemblyCSharp.Mod.Xmap;

// Token: 0x02000015 RID: 21
internal class DovaBaoKhu
{
	// Token: 0x06000078 RID: 120 RVA: 0x00004001 File Offset: 0x00002201
	private static void Wait(int time)
	{
		DovaBaoKhu.IsWait = true;
		DovaBaoKhu.TimeStartWait = mSystem.currentTimeMillis();
		DovaBaoKhu.TimeWait = (long)time;
	}

	// Token: 0x06000079 RID: 121 RVA: 0x0000A294 File Offset: 0x00008494
	private static bool IsWaiting()
	{
		bool flag = DovaBaoKhu.IsWait && mSystem.currentTimeMillis() - DovaBaoKhu.TimeStartWait >= DovaBaoKhu.TimeWait;
		if (flag)
		{
			DovaBaoKhu.IsWait = false;
		}
		return DovaBaoKhu.IsWait;
	}

	// Token: 0x0600007A RID: 122 RVA: 0x0000A2D8 File Offset: 0x000084D8
	public static void GetMapBoss()
	{
		bool flag = DovaBaoKhu.TbBoss != null && DovaBaoKhu.TbBoss.Contains("BOSS") && DovaBaoKhu.TbBoss.Contains("Broly");
		if (flag)
		{
			DovaBaoKhu.TbBoss = DovaBaoKhu.TbBoss.Replace("Boss ", "");
			DovaBaoKhu.TbBoss = DovaBaoKhu.TbBoss.Replace(" vừa xuất hiện tại", "|");
			DovaBaoKhu.TbBoss = DovaBaoKhu.TbBoss.Replace("Khu ", "|");
			string[] array = DovaBaoKhu.TbBoss.Split(new char[] { '|' });
			string text = array[1].Trim();
			AutoBroly.Map = DovaBaoKhu.MapID(text);
			DovaBaoKhu.TbBoss = "";
		}
	}

	// Token: 0x0600007B RID: 123 RVA: 0x00009E24 File Offset: 0x00008024
	public static bool IsBoss()
	{
		for (int i = 0; i < GameScr.vCharInMap.size(); i++)
		{
			global::Char @char = (global::Char)GameScr.vCharInMap.elementAt(i);
			bool flag = @char != null && @char.cName.Contains("Broly") && @char.cName.Contains("Super") && @char.cHPFull >= 16070777L;
			if (flag)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600007C RID: 124 RVA: 0x0000A398 File Offset: 0x00008598
	public static void Out()
	{
		string text = string.Format("Nro_244_Data/Resources/Status/xong{0}", DataAccount.ID);
		File.Create(text).Close();
		Thread.Sleep(10000);
		Main.exit();
		DovaBaoKhu.IsPet = false;
	}

	// Token: 0x0600007D RID: 125 RVA: 0x0000A3E0 File Offset: 0x000085E0
	public static int MapID(string a)
	{
		for (int i = 0; i < TileMap.mapNames.Length; i++)
		{
			bool flag = TileMap.mapNames[i].Equals(a);
			if (flag)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x0600007E RID: 126 RVA: 0x0000A424 File Offset: 0x00008624
	public static void Update()
	{
		bool flag = DovaBaoKhu.IsWaiting();
		if (!flag)
		{
			bool flag2 = global::Char.myCharz().cHP <= 0L || global::Char.myCharz().meDead;
			if (flag2)
			{
				bool flag3 = DovaBaoKhu.IsBoss();
				if (flag3)
				{
					AutoBroly.Map = TileMap.mapID;
					AutoBroly.Khu = TileMap.zoneID;
				}
				Service.gI().returnTownFromDead();
				DovaBaoKhu.Wait(3000);
			}
			else
			{
				bool havePet = global::Char.myCharz().havePet;
				if (havePet)
				{
					AutoBroly.TrangThai = "Đã có Đệ";
					bool flag4 = !DovaBaoKhu.IsPet;
					if (flag4)
					{
						DovaBaoKhu.IsPet = true;
						new Thread(new ThreadStart(DovaBaoKhu.Out)).Start();
					}
					DovaBaoKhu.Wait(500);
				}
				else
				{
					bool flag5 = DovaBaoKhu.IsBoss();
					if (flag5)
					{
						string text = string.Format("Super Broly - {0} khu {1} - [{2:yyyy-MM-dd HH:mm:ss}]", TileMap.mapNames[TileMap.mapID], TileMap.zoneID, DateTime.Now);
						File.WriteAllText("Nro_244_Data//Resources//thongbao", text);
						DovaBaoKhu.Wait(3000);
					}
					else
					{
						bool flag6 = File.Exists("Nro_244_Data//Resources//dokhu");
						if (flag6)
						{
							bool flag7 = !DovaBaoKhu.IsBoss() && !Pk9rXmap.IsXmapRunning;
							if (flag7)
							{
								DovaBaoKhu.GetMapBoss();
							}
							bool flag8 = !DovaBaoKhu.IsBoss();
							if (flag8)
							{
								AutoBroly.TrangThai = "Đang dò boss";
								bool flag9 = TileMap.mapID == AutoBroly.Map;
								if (flag9)
								{
									AutoBroly.SearchBoss();
								}
								DovaBaoKhu.Wait(2000);
								return;
							}
						}
						DovaBaoKhu.Wait(500);
					}
				}
			}
		}
	}

	// Token: 0x04000066 RID: 102
	private static bool IsWait;

	// Token: 0x04000067 RID: 103
	private static long TimeStartWait;

	// Token: 0x04000068 RID: 104
	private static long TimeWait;

	// Token: 0x04000069 RID: 105
	public static string TbBoss;

	// Token: 0x0400006A RID: 106
	public static bool isDoKhu;

	// Token: 0x0400006B RID: 107
	public static bool IsPet;
}
using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;

// Token: 0x0200005B RID: 91
public class GameCanvas : IActionListener
{
	// Token: 0x060003EF RID: 1007 RVA: 0x0004C098 File Offset: 0x0004A298
	public GameCanvas()
	{
		int num = Rms.loadRMSInt("languageVersion");
		int num2 = num;
		if (num2 != -1)
		{
			if (num2 != 2)
			{
				Main.main.doClearRMS();
				Rms.saveRMSInt("languageVersion", 2);
			}
		}
		else
		{
			Rms.saveRMSInt("languageVersion", 2);
		}
		GameCanvas.clearOldData = Rms.loadRMSInt(GameMidlet.VERSION);
		bool flag = GameCanvas.clearOldData != 1;
		if (flag)
		{
			Main.main.doClearRMS();
			Rms.saveRMSInt(GameMidlet.VERSION, 1);
		}
		this.initGame();
	}

	// Token: 0x060003F0 RID: 1008 RVA: 0x0004C164 File Offset: 0x0004A364
	public static string getPlatformName()
	{
		return "Pc platform xxx";
	}

	// Token: 0x060003F1 RID: 1009 RVA: 0x0004C17C File Offset: 0x0004A37C
	public void initGame()
	{
		try
		{
			MotherCanvas.instance.setChildCanvas(this);
			GameCanvas.w = MotherCanvas.instance.getWidthz();
			GameCanvas.h = MotherCanvas.instance.getHeightz();
			GameCanvas.hw = GameCanvas.w / 2;
			GameCanvas.hh = GameCanvas.h / 2;
			GameCanvas.isTouch = true;
			bool flag = GameCanvas.w >= 240;
			if (flag)
			{
				GameCanvas.isTouchControl = true;
			}
			bool flag2 = GameCanvas.w < 320;
			if (flag2)
			{
				GameCanvas.isTouchControlSmallScreen = true;
			}
			bool flag3 = GameCanvas.w >= 320;
			if (flag3)
			{
				GameCanvas.isTouchControlLargeScreen = true;
			}
			GameCanvas.msgdlg = new MsgDlg();
			bool flag4 = GameCanvas.h <= 160;
			if (flag4)
			{
				Paint.hTab = 15;
				mScreen.cmdH = 17;
			}
			GameScr.d = ((GameCanvas.w <= GameCanvas.h) ? GameCanvas.h : GameCanvas.w) + 20;
			GameCanvas.instance = this;
			mFont.init();
			mScreen.ITEM_HEIGHT = mFont.tahoma_8b.getHeight() + 8;
			this.initPaint();
			this.loadDust();
			this.loadWaterSplash();
			GameCanvas.panel = new Panel();
			GameCanvas.imgShuriken = GameCanvas.loadImage("/mainImage/myTexture2df.png");
			int num = Rms.loadRMSInt("clienttype");
			bool flag5 = num != -1;
			if (flag5)
			{
				bool flag6 = num > 7;
				if (flag6)
				{
					Rms.saveRMSInt("clienttype", mSystem.clientType);
				}
				else
				{
					mSystem.clientType = num;
				}
			}
			bool flag7 = mSystem.clientType == 7 && (Rms.loadRMSString("fake") == null || Rms.loadRMSString("fake") == string.Empty);
			if (flag7)
			{
				GameCanvas.imgShuriken = GameCanvas.loadImage("/mainImage/wait.png");
			}
			GameCanvas.imgClear = GameCanvas.loadImage("/mainImage/myTexture2der.png");
			GameCanvas.img12 = GameCanvas.loadImage("/mainImage/12+.png");
			GameCanvas.debugUpdate = new MyVector();
			GameCanvas.debugPaint = new MyVector();
			GameCanvas.debugSession = new MyVector();
			for (int i = 0; i < 3; i++)
			{
				GameCanvas.imgBorder[i] = GameCanvas.loadImage("/mainImage/myTexture2dbd" + i.ToString() + ".png");
			}
			GameCanvas.borderConnerW = mGraphics.getImageWidth(GameCanvas.imgBorder[0]);
			GameCanvas.borderConnerH = mGraphics.getImageHeight(GameCanvas.imgBorder[0]);
			GameCanvas.borderCenterW = mGraphics.getImageWidth(GameCanvas.imgBorder[1]);
			GameCanvas.borderCenterH = mGraphics.getImageHeight(GameCanvas.imgBorder[1]);
			Panel.graphics = Rms.loadRMSInt("lowGraphic");
			GameCanvas.lowGraphic = Rms.loadRMSInt("lowGraphic") == 1;
			GameScr.isPaintChatVip = Rms.loadRMSInt("serverchat") != 1;
			global::Char.isPaintAura = Rms.loadRMSInt("isPaintAura") == 1;
			global::Char.isPaintAura2 = Rms.loadRMSInt("isPaintAura2") == 1;
			Res.init();
			SmallImage.loadBigImage();
			Panel.WIDTH_PANEL = 176;
			bool flag8 = Panel.WIDTH_PANEL > GameCanvas.w;
			if (flag8)
			{
				Panel.WIDTH_PANEL = GameCanvas.w;
			}
			InfoMe.gI().loadCharId();
			Command.btn0left = GameCanvas.loadImage("/mainImage/btn0left.png");
			Command.btn0mid = GameCanvas.loadImage("/mainImage/btn0mid.png");
			Command.btn0right = GameCanvas.loadImage("/mainImage/btn0right.png");
			Command.btn1left = GameCanvas.loadImage("/mainImage/btn1left.png");
			Command.btn1mid = GameCanvas.loadImage("/mainImage/btn1mid.png");
			Command.btn1right = GameCanvas.loadImage("/mainImage/btn1right.png");
			GameCanvas.serverScreen = new ServerListScreen();
			GameCanvas.img12 = GameCanvas.loadImage("/mainImage/12+.png");
			for (int j = 0; j < 7; j++)
			{
				GameCanvas.imgBlue[j] = GameCanvas.loadImage("/effectdata/blue/" + j.ToString() + ".png");
				GameCanvas.imgViolet[j] = GameCanvas.loadImage("/effectdata/violet/" + j.ToString() + ".png");
			}
			ServerListScreen.createDeleteRMS();
			GameCanvas.serverScr = new ServerScr();
			GameCanvas.loginScr = new LoginScr();
			GameCanvas._SelectCharScr = new SelectCharScr();
		}
		catch (Exception)
		{
			Debug.LogError("----------------->>>>>>>>>>errr");
		}
	}

	// Token: 0x060003F2 RID: 1010 RVA: 0x0004C5B8 File Offset: 0x0004A7B8
	public static GameCanvas gI()
	{
		return GameCanvas.instance;
	}

	// Token: 0x060003F3 RID: 1011 RVA: 0x00004A8A File Offset: 0x00002C8A
	public void initPaint()
	{
		GameCanvas.paintz = new Paint();
	}

	// Token: 0x060003F4 RID: 1012 RVA: 0x00004A97 File Offset: 0x00002C97
	public static void closeKeyBoard()
	{
		mGraphics.addYWhenOpenKeyBoard = 0;
		GameCanvas.timeOpenKeyBoard = 0;
		Main.closeKeyBoard();
	}

	// Token: 0x060003F5 RID: 1013 RVA: 0x0004C5D0 File Offset: 0x0004A7D0
	public void update()
	{
		bool flag = GameCanvas.currentScreen == GameCanvas._SelectCharScr;
		if (flag)
		{
			bool flag2 = GameCanvas.gameTick % 2 == 0 && SmallImage.vt_images_watingDowload.size() > 0;
			if (flag2)
			{
				Small small = (Small)SmallImage.vt_images_watingDowload.elementAt(0);
				Service.gI().requestIcon(small.id);
				SmallImage.vt_images_watingDowload.removeElementAt(0);
			}
		}
		else
		{
			bool flag3 = GameCanvas.isRequestMapID == 2 && GameCanvas.waitingTimeChangeMap < mSystem.currentTimeMillis() && GameCanvas.gameTick % 2 == 0 && GameCanvas.currentScreen != null;
			if (flag3)
			{
				bool flag4 = GameCanvas.currentScreen == GameScr.gI();
				if (flag4)
				{
					bool isLoadingMap = global::Char.isLoadingMap;
					if (isLoadingMap)
					{
						global::Char.isLoadingMap = false;
					}
					bool waitToLogin = ServerListScreen.waitToLogin;
					if (waitToLogin)
					{
						ServerListScreen.waitToLogin = false;
					}
				}
				bool flag5 = SmallImage.vt_images_watingDowload.size() > 0;
				if (flag5)
				{
					Small small2 = (Small)SmallImage.vt_images_watingDowload.elementAt(0);
					Service.gI().requestIcon(small2.id);
					SmallImage.vt_images_watingDowload.removeElementAt(0);
				}
				bool flag6 = Effect.dowloadEff.size() <= 0;
				if (flag6)
				{
				}
			}
		}
		bool flag7 = mSystem.currentTimeMillis() > this.timefps;
		if (flag7)
		{
			this.timefps += 1000L;
			GameCanvas.max = GameCanvas.fps;
			GameCanvas.fps = 0;
		}
		GameCanvas.fps++;
		bool flag8 = GameCanvas.messageServer.size() > 0 && GameCanvas.thongBaoTest == null;
		if (flag8)
		{
			GameCanvas.startserverThongBao((string)GameCanvas.messageServer.elementAt(0));
			GameCanvas.messageServer.removeElementAt(0);
		}
		bool flag9 = GameCanvas.gameTick % 5 == 0;
		if (flag9)
		{
			GameCanvas.timeNow = mSystem.currentTimeMillis();
		}
		Res.updateOnScreenDebug();
		try
		{
			bool visible = global::TouchScreenKeyboard.visible;
			if (visible)
			{
				GameCanvas.timeOpenKeyBoard++;
				bool flag10 = GameCanvas.timeOpenKeyBoard > ((!Main.isWindowsPhone) ? 10 : 5);
				if (flag10)
				{
					mGraphics.addYWhenOpenKeyBoard = 94;
				}
			}
			else
			{
				mGraphics.addYWhenOpenKeyBoard = 0;
				GameCanvas.timeOpenKeyBoard = 0;
			}
			GameCanvas.debugUpdate.removeAllElements();
			long num = mSystem.currentTimeMillis();
			bool flag11 = num - GameCanvas.timeTickEff1 >= 780L && !GameCanvas.isEff1;
			if (flag11)
			{
				GameCanvas.timeTickEff1 = num;
				GameCanvas.isEff1 = true;
			}
			else
			{
				GameCanvas.isEff1 = false;
			}
			bool flag12 = num - GameCanvas.timeTickEff2 >= 7800L && !GameCanvas.isEff2;
			if (flag12)
			{
				GameCanvas.timeTickEff2 = num;
				GameCanvas.isEff2 = true;
			}
			else
			{
				GameCanvas.isEff2 = false;
			}
			bool flag13 = GameCanvas.taskTick > 0;
			if (flag13)
			{
				GameCanvas.taskTick--;
			}
			GameCanvas.gameTick++;
			bool flag14 = GameCanvas.gameTick > 10000;
			if (flag14)
			{
				bool flag15 = mSystem.currentTimeMillis() - GameCanvas.lastTimePress > 20000L && GameCanvas.currentScreen == GameCanvas.loginScr;
				if (flag15)
				{
					GameMidlet.instance.exit();
				}
				GameCanvas.gameTick = 0;
			}
			bool flag16 = GameCanvas.currentScreen != null;
			if (flag16)
			{
				bool flag17 = ChatPopup.serverChatPopUp != null;
				if (flag17)
				{
					ChatPopup.serverChatPopUp.update();
					ChatPopup.serverChatPopUp.updateKey();
				}
				else
				{
					bool flag18 = ChatPopup.currChatPopup != null;
					if (flag18)
					{
						ChatPopup.currChatPopup.update();
						ChatPopup.currChatPopup.updateKey();
					}
					else
					{
						bool flag19 = GameCanvas.currentDialog != null;
						if (flag19)
						{
							GameCanvas.debug("B", 0);
							GameCanvas.currentDialog.update();
						}
						else
						{
							bool showMenu = GameCanvas.menu.showMenu;
							if (showMenu)
							{
								GameCanvas.debug("C", 0);
								GameCanvas.menu.updateMenu();
								GameCanvas.debug("D", 0);
								GameCanvas.menu.updateMenuKey();
							}
							else
							{
								bool isShow = GameCanvas.panel.isShow;
								if (isShow)
								{
									GameCanvas.panel.update();
									bool flag20 = GameCanvas.isPointer(GameCanvas.panel.X, GameCanvas.panel.Y, GameCanvas.panel.W, GameCanvas.panel.H);
									if (flag20)
									{
										GameCanvas.isFocusPanel2 = false;
									}
									bool flag21 = GameCanvas.panel2 != null && GameCanvas.panel2.isShow;
									if (flag21)
									{
										GameCanvas.panel2.update();
										bool flag22 = GameCanvas.isPointer(GameCanvas.panel2.X, GameCanvas.panel2.Y, GameCanvas.panel2.W, GameCanvas.panel2.H);
										if (flag22)
										{
											GameCanvas.isFocusPanel2 = true;
										}
									}
									bool flag23 = GameCanvas.panel2 != null;
									if (flag23)
									{
										bool flag24 = GameCanvas.isFocusPanel2;
										if (flag24)
										{
											GameCanvas.panel2.updateKey();
										}
										else
										{
											GameCanvas.panel.updateKey();
										}
									}
									else
									{
										GameCanvas.panel.updateKey();
									}
									bool flag25 = GameCanvas.panel.chatTField != null && GameCanvas.panel.chatTField.isShow;
									if (flag25)
									{
										GameCanvas.panel.chatTFUpdateKey();
									}
									else
									{
										bool flag26 = GameCanvas.panel2 != null && GameCanvas.panel2.chatTField != null && GameCanvas.panel2.chatTField.isShow;
										if (flag26)
										{
											GameCanvas.panel2.chatTFUpdateKey();
										}
										else
										{
											bool flag27 = (GameCanvas.isPointer(GameCanvas.panel.X, GameCanvas.panel.Y, GameCanvas.panel.W, GameCanvas.panel.H) && GameCanvas.panel2 != null) || GameCanvas.panel2 == null;
											if (flag27)
											{
												GameCanvas.panel.updateKey();
											}
											else
											{
												bool flag28 = GameCanvas.panel2 != null && GameCanvas.panel2.isShow && GameCanvas.isPointer(GameCanvas.panel2.X, GameCanvas.panel2.Y, GameCanvas.panel2.W, GameCanvas.panel2.H);
												if (flag28)
												{
													GameCanvas.panel2.updateKey();
												}
											}
										}
									}
									bool flag29 = GameCanvas.isPointer(GameCanvas.panel.X + GameCanvas.panel.W, GameCanvas.panel.Y, GameCanvas.w - GameCanvas.panel.W * 2, GameCanvas.panel.H) && GameCanvas.isPointerJustRelease && GameCanvas.panel.isDoneCombine;
									if (flag29)
									{
										GameCanvas.panel.hide();
									}
								}
							}
						}
					}
				}
				GameCanvas.debug("E", 0);
				bool flag30 = !GameCanvas.isLoading;
				if (flag30)
				{
					GameCanvas.currentScreen.update();
				}
				GameCanvas.debug("F", 0);
				bool flag31 = !GameCanvas.panel.isShow && ChatPopup.serverChatPopUp == null;
				if (flag31)
				{
					GameCanvas.currentScreen.updateKey();
				}
				Hint.update();
				SoundMn.gI().update();
			}
			GameCanvas.debug("Ix", 0);
			Timer.update();
			GameCanvas.debug("Hx", 0);
			InfoDlg.update();
			GameCanvas.debug("G", 0);
			bool flag32 = this.resetToLoginScr;
			if (flag32)
			{
				this.resetToLoginScr = false;
				this.doResetToLoginScr(GameCanvas.loginScr);
			}
			GameCanvas.debug("Zzz", 0);
			bool flag33 = (GameCanvas.currentScreen != GameCanvas.serverScr || !GameCanvas.serverScr.isPaintNewUi) && Controller.isConnectOK;
			if (flag33)
			{
				bool isMain = Controller.isMain;
				if (isMain)
				{
					ServerListScreen.testConnect = 2;
					Service.gI().setClientType();
					Service.gI().androidPack();
				}
				else
				{
					Service.gI().setClientType2();
					Service.gI().androidPack2();
				}
				Controller.isConnectOK = false;
			}
			bool isDisconnected = Controller.isDisconnected;
			if (isDisconnected)
			{
				bool flag34 = !Controller.isMain;
				if (flag34)
				{
					bool flag35 = GameCanvas.currentScreen == GameCanvas.serverScreen && !Service.reciveFromMainSession;
					if (flag35)
					{
						GameCanvas.serverScreen.cancel();
					}
					bool flag36 = GameCanvas.currentScreen == GameCanvas.loginScr && !Service.reciveFromMainSession;
					if (flag36)
					{
						this.onDisconnected();
					}
				}
				else
				{
					this.onDisconnected();
				}
				Controller.isDisconnected = false;
			}
			bool isConnectionFail = Controller.isConnectionFail;
			if (isConnectionFail)
			{
				bool flag37 = !Controller.isMain;
				if (flag37)
				{
					bool flag38 = GameCanvas.currentScreen == GameCanvas.serverScreen && ServerListScreen.isGetData && !Service.reciveFromMainSession;
					if (flag38)
					{
						ServerListScreen.testConnect = 0;
						GameCanvas.serverScreen.cancel();
						Debug.Log("connect fail 1");
					}
					bool flag39 = GameCanvas.currentScreen == GameCanvas.loginScr && !Service.reciveFromMainSession;
					if (flag39)
					{
						this.onConnectionFail();
						Debug.Log("connect fail 2");
					}
				}
				else
				{
					bool flag40 = Session_ME.gI().isCompareIPConnect();
					if (flag40)
					{
						this.onConnectionFail();
					}
					Debug.Log("connect fail 3");
				}
				Controller.isConnectionFail = false;
			}
			bool flag41 = Main.isResume;
			if (flag41)
			{
				Main.isResume = false;
				bool flag42 = GameCanvas.currentDialog != null && GameCanvas.currentDialog.left != null && GameCanvas.currentDialog.left.actionListener != null;
				if (flag42)
				{
					GameCanvas.currentDialog.left.performAction();
				}
			}
			bool flag43 = GameCanvas.currentScreen != null && GameCanvas.currentScreen is GameScr;
			if (flag43)
			{
				GameCanvas.xThongBaoTranslate += GameCanvas.dir_ * 2;
				bool flag44 = GameCanvas.xThongBaoTranslate - Panel.imgNew.getWidth() <= 60;
				if (flag44)
				{
					GameCanvas.dir_ = 0;
					this.tickWaitThongBao++;
					bool flag45 = this.tickWaitThongBao > 150;
					if (flag45)
					{
						this.tickWaitThongBao = 0;
						GameCanvas.thongBaoTest = null;
					}
				}
			}
			bool flag46 = GameCanvas.currentScreen != null && GameCanvas.currentScreen.Equals(GameScr.gI());
			if (flag46)
			{
				bool flag47 = GameScr.info1 != null;
				if (flag47)
				{
					GameScr.info1.update();
				}
				bool flag48 = GameScr.info2 != null;
				if (flag48)
				{
					GameScr.info2.update();
				}
			}
			GameCanvas.isPointerSelect = false;
		}
		catch (Exception)
		{
		}
	}

	// Token: 0x060003F6 RID: 1014 RVA: 0x0004D038 File Offset: 0x0004B238
	public void onDisconnected()
	{
		bool isConnectionFail = Controller.isConnectionFail;
		if (isConnectionFail)
		{
			Controller.isConnectionFail = false;
		}
		GameCanvas.isResume = true;
		Session_ME.gI().clearSendingMessage();
		Session_ME2.gI().clearSendingMessage();
		Session_ME.gI().close();
		Session_ME2.gI().close();
		bool isLoadingData = Controller.isLoadingData;
		if (isLoadingData)
		{
			GameCanvas.startOK(mResources.pls_restart_game_error, 8885, null);
			Controller.isDisconnected = false;
		}
		else
		{
			Debug.LogError(">>>>onDisconnected");
			bool flag = GameCanvas.currentScreen != GameCanvas.serverScreen;
			if (flag)
			{
				GameCanvas.serverScreen.switchToMe();
				GameCanvas.startOK(mResources.maychutathoacmatsong + " [4]", 8884, null);
				Main.exit();
			}
			else
			{
				GameCanvas.endDlg();
			}
			global::Char.isLoadingMap = false;
			bool isMain = Controller.isMain;
			if (isMain)
			{
				ServerListScreen.testConnect = 0;
			}
			mSystem.endKey();
		}
	}

	// Token: 0x060003F7 RID: 1015 RVA: 0x0004D120 File Offset: 0x0004B320
	public void onConnectionFail()
	{
		bool flag = GameCanvas.currentScreen.Equals(SplashScr.instance);
		if (flag)
		{
			GameCanvas.startOK(mResources.maychutathoacmatsong + " [1]", 8884, null);
		}
		else
		{
			Session_ME.gI().clearSendingMessage();
			Session_ME2.gI().clearSendingMessage();
			ServerListScreen.isWait = false;
			bool isLoadingData = Controller.isLoadingData;
			if (isLoadingData)
			{
				GameCanvas.startOK(mResources.maychutathoacmatsong + " [2]", 8884, null);
				Controller.isConnectionFail = false;
			}
			else
			{
				GameCanvas.isResume = true;
				LoginScr.isContinueToLogin = false;
				LoginScr.serverName = ServerListScreen.nameServer[ServerListScreen.ipSelect];
				bool flag2 = GameCanvas.currentScreen != GameCanvas.serverScreen;
				if (flag2)
				{
					ServerListScreen.countDieConnect = 0;
				}
				else
				{
					GameCanvas.endDlg();
					ServerListScreen.loadScreen = true;
					GameCanvas.serverScreen.switchToMe();
				}
				global::Char.isLoadingMap = false;
				bool isMain = Controller.isMain;
				if (isMain)
				{
					ServerListScreen.testConnect = 0;
				}
				mSystem.endKey();
			}
		}
	}

	// Token: 0x060003F8 RID: 1016 RVA: 0x0004D21C File Offset: 0x0004B41C
	public static bool isWaiting()
	{
		return InfoDlg.isShow || (GameCanvas.msgdlg != null && GameCanvas.msgdlg.info.Equals(mResources.PLEASEWAIT)) || global::Char.isLoadingMap || LoginScr.isContinueToLogin;
	}

	// Token: 0x060003F9 RID: 1017 RVA: 0x0004D26C File Offset: 0x0004B46C
	public static void connect()
	{
		bool flag = !Session_ME.gI().isConnected();
		if (flag)
		{
			Session_ME.gI().connect(GameMidlet.IP, GameMidlet.PORT);
		}
	}

	// Token: 0x060003FA RID: 1018 RVA: 0x0004D2A4 File Offset: 0x0004B4A4
	public static void connect2()
	{
		bool flag = !Session_ME2.gI().isConnected();
		if (flag)
		{
			Res.outz("IP2= " + GameMidlet.IP2 + " PORT 2= " + GameMidlet.PORT2.ToString());
			Session_ME2.gI().connect(GameMidlet.IP2, GameMidlet.PORT2);
		}
	}

	// Token: 0x060003FB RID: 1019 RVA: 0x00004AAC File Offset: 0x00002CAC
	public static void resetTrans(mGraphics g)
	{
		g.translate(-g.getTranslateX(), -g.getTranslateY());
		g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
	}

	// Token: 0x060003FC RID: 1020 RVA: 0x0004D300 File Offset: 0x0004B500
	public static void resetTransGameScr(mGraphics g)
	{
		g.translate(-g.getTranslateX(), -g.getTranslateY());
		g.translate(0, 0);
		g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
		g.translate(-GameScr.cmx, -GameScr.cmy);
	}

	// Token: 0x060003FD RID: 1021 RVA: 0x0004D354 File Offset: 0x0004B554
	public void initGameCanvas()
	{
		GameCanvas.debug("SP2i1", 0);
		GameCanvas.w = MotherCanvas.instance.getWidthz();
		GameCanvas.h = MotherCanvas.instance.getHeightz();
		GameCanvas.debug("SP2i2", 0);
		GameCanvas.hw = GameCanvas.w / 2;
		GameCanvas.hh = GameCanvas.h / 2;
		GameCanvas.wd3 = GameCanvas.w / 3;
		GameCanvas.hd3 = GameCanvas.h / 3;
		GameCanvas.w2d3 = 2 * GameCanvas.w / 3;
		GameCanvas.h2d3 = 2 * GameCanvas.h / 3;
		GameCanvas.w3d4 = 3 * GameCanvas.w / 4;
		GameCanvas.h3d4 = 3 * GameCanvas.h / 4;
		GameCanvas.wd6 = GameCanvas.w / 6;
		GameCanvas.hd6 = GameCanvas.h / 6;
		GameCanvas.debug("SP2i3", 0);
		mScreen.initPos();
		GameCanvas.debug("SP2i4", 0);
		GameCanvas.debug("SP2i5", 0);
		GameCanvas.inputDlg = new InputDlg();
		GameCanvas.debug("SP2i6", 0);
		GameCanvas.listPoint = new MyVector();
		GameCanvas.debug("SP2i7", 0);
	}

	// Token: 0x060003FE RID: 1022 RVA: 0x00003E4C File Offset: 0x0000204C
	public void start()
	{
	}

	// Token: 0x060003FF RID: 1023 RVA: 0x0004D470 File Offset: 0x0004B670
	public int getWidth()
	{
		return (int)ScaleGUI.WIDTH;
	}

	// Token: 0x06000400 RID: 1024 RVA: 0x0004D488 File Offset: 0x0004B688
	public int getHeight()
	{
		return (int)ScaleGUI.HEIGHT;
	}

	// Token: 0x06000401 RID: 1025 RVA: 0x00003E4C File Offset: 0x0000204C
	public static void debug(string s, int type)
	{
	}

	// Token: 0x06000402 RID: 1026 RVA: 0x0004D4A0 File Offset: 0x0004B6A0
	public void doResetToLoginScr(mScreen screen)
	{
		try
		{
			SoundMn.gI().stopAll();
			LoginScr.isContinueToLogin = false;
			TileMap.lastType = (TileMap.bgType = 0);
			global::Char.clearMyChar();
			GameScr.clearGameScr();
			GameScr.resetAllvector();
			InfoDlg.hide();
			GameScr.info1.hide();
			GameScr.info2.hide();
			GameScr.info2.cmdChat = null;
			Hint.isShow = false;
			ChatPopup.currChatPopup = null;
			Controller.isStopReadMessage = false;
			GameScr.loadCamera(true, -1, -1);
			GameScr.cmx = 100;
			GameCanvas.panel.currentTabIndex = 0;
			GameCanvas.panel.selected = (GameCanvas.isTouch ? (-1) : 0);
			GameCanvas.panel.init();
			GameCanvas.panel2 = null;
			GameScr.isPaint = true;
			ClanMessage.vMessage.removeAllElements();
			GameScr.textTime.removeAllElements();
			GameScr.vClan.removeAllElements();
			GameScr.vFriend.removeAllElements();
			GameScr.vEnemies.removeAllElements();
			TileMap.vCurrItem.removeAllElements();
			BackgroudEffect.vBgEffect.removeAllElements();
			EffecMn.vEff.removeAllElements();
			Effect.newEff.removeAllElements();
			GameCanvas.menu.showMenu = false;
			GameCanvas.panel.vItemCombine.removeAllElements();
			GameCanvas.panel.isShow = false;
			bool flag = GameCanvas.panel.tabIcon != null;
			if (flag)
			{
				GameCanvas.panel.tabIcon.isShow = false;
			}
			bool flag2 = mGraphics.zoomLevel == 1;
			if (flag2)
			{
				SmallImage.clearHastable();
			}
			Session_ME.gI().close();
			Session_ME2.gI().close();
		}
		catch (Exception ex)
		{
			Cout.println("Loi tai doResetToLoginScr " + ex.ToString());
		}
		ServerListScreen.isAutoConect = true;
		ServerListScreen.countDieConnect = 0;
		ServerListScreen.testConnect = -1;
		ServerListScreen.loadScreen = true;
		bool flag3 = ServerListScreen.ipSelect == -1;
		if (flag3)
		{
			GameCanvas.serverScr.switchToMe();
		}
		else
		{
			bool flag4 = GameCanvas.serverScreen == null;
			if (flag4)
			{
				GameCanvas.serverScreen = new ServerListScreen();
			}
			GameCanvas.serverScreen.switchToMe();
		}
	}

	// Token: 0x06000403 RID: 1027 RVA: 0x00003E4C File Offset: 0x0000204C
	public static void showErrorForm(int type, string moreInfo)
	{
	}

	// Token: 0x06000404 RID: 1028 RVA: 0x00003E4C File Offset: 0x0000204C
	public static void paintCloud(mGraphics g)
	{
	}

	// Token: 0x06000405 RID: 1029 RVA: 0x00003E4C File Offset: 0x0000204C
	public static void updateBG()
	{
	}

	// Token: 0x06000406 RID: 1030 RVA: 0x0004D6C8 File Offset: 0x0004B8C8
	public static void fillRect(mGraphics g, int color, int x, int y, int w, int h, int detalY)
	{
		g.setColor(color);
		int cmy = GameScr.cmy;
		bool flag = cmy > GameCanvas.h;
		if (flag)
		{
			cmy = GameCanvas.h;
		}
		g.fillRect(x, y - ((detalY != 0) ? (cmy >> detalY) : 0), w, h + ((detalY != 0) ? (cmy >> detalY) : 0));
	}

	// Token: 0x06000407 RID: 1031 RVA: 0x0004D724 File Offset: 0x0004B924
	public static void paintBackgroundtLayer(mGraphics g, int layer, int deltaY, int color1, int color2)
	{
		try
		{
			int num = layer - 1;
			bool flag = num == GameCanvas.imgBG.Length - 1 && (GameScr.gI().isRongThanXuatHien || GameScr.gI().isFireWorks);
			if (flag)
			{
				g.setColor(GameScr.gI().mautroi);
				g.fillRect(0, 0, GameCanvas.w, GameCanvas.h);
				bool flag2 = GameCanvas.typeBg == 2 || GameCanvas.typeBg == 4 || GameCanvas.typeBg == 7;
				if (flag2)
				{
					GameCanvas.drawSun1(g);
					GameCanvas.drawSun2(g);
				}
				bool flag3 = GameScr.gI().isFireWorks && !GameCanvas.lowGraphic;
				if (flag3)
				{
					FireWorkEff.paint(g);
				}
			}
			else
			{
				bool flag4 = GameCanvas.imgBG == null || GameCanvas.imgBG[num] == null;
				if (!flag4)
				{
					bool flag5 = GameCanvas.moveX[num] != 0;
					if (flag5)
					{
						GameCanvas.moveX[num] += GameCanvas.moveXSpeed[num];
					}
					int cmy = GameScr.cmy;
					bool flag6 = cmy > GameCanvas.h;
					if (flag6)
					{
						cmy = GameCanvas.h;
					}
					bool flag7 = GameCanvas.layerSpeed[num] != 0;
					if (flag7)
					{
						for (int i = -((GameScr.cmx + GameCanvas.moveX[num] >> GameCanvas.layerSpeed[num]) % GameCanvas.bgW[num]); i < GameScr.gW; i += GameCanvas.bgW[num])
						{
							g.drawImage(GameCanvas.imgBG[num], i, GameCanvas.yb[num] - ((deltaY > 0) ? (cmy >> deltaY) : 0), 0);
						}
					}
					else
					{
						for (int j = 0; j < GameScr.gW; j += GameCanvas.bgW[num])
						{
							g.drawImage(GameCanvas.imgBG[num], j, GameCanvas.yb[num] - ((deltaY > 0) ? (cmy >> deltaY) : 0), 0);
						}
					}
					bool flag8 = color1 != -1;
					if (flag8)
					{
						bool flag9 = num == GameCanvas.nBg - 1;
						if (flag9)
						{
							GameCanvas.fillRect(g, color1, 0, -(cmy >> deltaY), GameScr.gW, GameCanvas.yb[num], deltaY);
						}
						else
						{
							GameCanvas.fillRect(g, color1, 0, GameCanvas.yb[num - 1] + GameCanvas.bgH[num - 1], GameScr.gW, GameCanvas.yb[num] - (GameCanvas.yb[num - 1] + GameCanvas.bgH[num - 1]), deltaY);
						}
					}
					bool flag10 = color2 != -1;
					if (flag10)
					{
						bool flag11 = num == 0;
						if (flag11)
						{
							GameCanvas.fillRect(g, color2, 0, GameCanvas.yb[num] + GameCanvas.bgH[num], GameScr.gW, GameScr.gH - (GameCanvas.yb[num] + GameCanvas.bgH[num]), deltaY);
						}
						else
						{
							GameCanvas.fillRect(g, color2, 0, GameCanvas.yb[num] + GameCanvas.bgH[num], GameScr.gW, GameCanvas.yb[num - 1] - (GameCanvas.yb[num] + GameCanvas.bgH[num]) + 80, deltaY);
						}
					}
					bool flag12 = GameCanvas.currentScreen == GameScr.instance;
					if (flag12)
					{
						bool flag13 = layer == 1 && GameCanvas.typeBg == 11;
						if (flag13)
						{
							g.drawImage(GameCanvas.imgSun2, -(GameScr.cmx >> GameCanvas.layerSpeed[0]) + 400, GameCanvas.yb[0] + 30 - (cmy >> 2), StaticObj.BOTTOM_HCENTER);
						}
						bool flag14 = layer == 1 && GameCanvas.typeBg == 13;
						if (flag14)
						{
							g.drawImage(GameCanvas.imgBG[1], -(GameScr.cmx >> GameCanvas.layerSpeed[0]) + TileMap.tmw * 24 / 4, GameCanvas.yb[0] - (cmy >> 3) + 30, 0);
							g.drawRegion(GameCanvas.imgBG[1], 0, 0, GameCanvas.bgW[1], GameCanvas.bgH[1], 2, -(GameScr.cmx >> GameCanvas.layerSpeed[0]) + TileMap.tmw * 24 / 4 + GameCanvas.bgW[1], GameCanvas.yb[0] - (cmy >> 3) + 30, 0);
						}
						bool flag15 = layer == 3 && TileMap.mapID == 1;
						if (flag15)
						{
							for (int k = 0; k < TileMap.pxh / mGraphics.getImageHeight(GameCanvas.imgCaycot); k++)
							{
								g.drawImage(GameCanvas.imgCaycot, -(GameScr.cmx >> GameCanvas.layerSpeed[2]) + 300, k * mGraphics.getImageHeight(GameCanvas.imgCaycot) - (cmy >> 3), 0);
							}
						}
					}
					int num2 = -(GameScr.cmx + GameCanvas.moveX[num] >> GameCanvas.layerSpeed[num]);
					EffecMn.paintBackGroundUnderLayer(g, num2, GameCanvas.yb[num] + GameCanvas.bgH[num] - (cmy >> deltaY), num);
				}
			}
		}
		catch (Exception ex)
		{
			Cout.LogError("Loi ham paint bground: " + ex.ToString());
		}
	}

	// Token: 0x06000408 RID: 1032 RVA: 0x0004DC20 File Offset: 0x0004BE20
	public static void drawSun1(mGraphics g)
	{
		bool flag = GameCanvas.imgSun != null;
		if (flag)
		{
			g.drawImage(GameCanvas.imgSun, GameCanvas.sunX, GameCanvas.sunY, 0);
		}
		bool flag2 = !GameCanvas.isBoltEff;
		if (!flag2)
		{
			bool flag3 = GameCanvas.gameTick % 200 == 0;
			if (flag3)
			{
				GameCanvas.boltActive = true;
			}
			bool flag4 = GameCanvas.boltActive;
			if (flag4)
			{
				GameCanvas.tBolt++;
				bool flag5 = GameCanvas.tBolt == 10;
				if (flag5)
				{
					GameCanvas.tBolt = 0;
					GameCanvas.boltActive = false;
				}
				bool flag6 = GameCanvas.tBolt % 2 == 0;
				if (flag6)
				{
					g.setColor(16777215);
					g.fillRect(0, 0, GameCanvas.w, GameCanvas.h);
				}
			}
		}
	}

	// Token: 0x06000409 RID: 1033 RVA: 0x0004DCE4 File Offset: 0x0004BEE4
	public static void drawSun2(mGraphics g)
	{
		bool flag = GameCanvas.imgSun2 != null;
		if (flag)
		{
			g.drawImage(GameCanvas.imgSun2, GameCanvas.sunX2, GameCanvas.sunY2, 0);
		}
	}

	// Token: 0x0600040A RID: 1034 RVA: 0x0004DD18 File Offset: 0x0004BF18
	public static bool isHDVersion()
	{
		return mGraphics.zoomLevel > 1;
	}

	// Token: 0x0600040B RID: 1035 RVA: 0x0004DD3C File Offset: 0x0004BF3C
	public static void paint_ios_bg(mGraphics g)
	{
		bool flag = mSystem.clientType != 5;
		if (!flag)
		{
			bool flag2 = GameCanvas.imgBgIOS != null;
			if (flag2)
			{
				g.setColor(0);
				g.fillRect(0, 0, GameCanvas.w, GameCanvas.h);
				for (int i = 0; i < 3; i++)
				{
					g.drawImage(GameCanvas.imgBgIOS, GameCanvas.imgBgIOS.getWidth() * i, GameCanvas.h / 2, mGraphics.VCENTER | mGraphics.HCENTER);
				}
			}
			else
			{
				GameCanvas.imgBgIOS = mSystem.loadImage("/bg/bg_ios_" + ((TileMap.bgID % 2 != 0) ? 1 : 2).ToString() + ".png");
			}
		}
	}

	// Token: 0x0600040C RID: 1036 RVA: 0x0004DDF8 File Offset: 0x0004BFF8
	public static void paintBGGameScr(mGraphics g)
	{
		bool flag = !GameCanvas.isLoadBGok;
		if (flag)
		{
			g.setColor(0);
			g.fillRect(0, 0, GameCanvas.w, GameCanvas.h);
		}
		bool isLoadingMap = global::Char.isLoadingMap;
		if (!isLoadingMap)
		{
			int gW = GameScr.gW;
			int gH = GameScr.gH;
			g.translate(-g.getTranslateX(), -g.getTranslateY());
			g.setColor(8421504);
			g.fillRect(0, 0, GameCanvas.w, GameCanvas.h);
		}
	}

	// Token: 0x0600040D RID: 1037 RVA: 0x00003E4C File Offset: 0x0000204C
	public static void resetBg()
	{
	}

	// Token: 0x0600040E RID: 1038 RVA: 0x0004DE80 File Offset: 0x0004C080
	public static void getYBackground(int typeBg)
	{
		try
		{
			int gH = GameScr.gH23;
			switch (typeBg)
			{
			case 0:
				GameCanvas.yb[0] = gH - GameCanvas.bgH[0] + 70;
				GameCanvas.yb[1] = GameCanvas.yb[0] - GameCanvas.bgH[1] + 20;
				GameCanvas.yb[2] = GameCanvas.yb[1] - GameCanvas.bgH[2] + 30;
				GameCanvas.yb[3] = GameCanvas.yb[2] - GameCanvas.bgH[3] + 50;
				goto IL_0688;
			case 1:
				GameCanvas.yb[0] = gH - GameCanvas.bgH[0] + 120;
				GameCanvas.yb[1] = GameCanvas.yb[0] - GameCanvas.bgH[1] + 40;
				GameCanvas.yb[2] = GameCanvas.yb[1] - 90;
				GameCanvas.yb[3] = GameCanvas.yb[2] - 25;
				goto IL_0688;
			case 2:
				GameCanvas.yb[0] = gH - GameCanvas.bgH[0] + 150;
				GameCanvas.yb[1] = GameCanvas.yb[0] - GameCanvas.bgH[1] - 60;
				GameCanvas.yb[2] = GameCanvas.yb[1] - GameCanvas.bgH[2] - 40;
				GameCanvas.yb[3] = GameCanvas.yb[2] - GameCanvas.bgH[3] - 10;
				GameCanvas.yb[4] = GameCanvas.yb[3] - GameCanvas.bgH[4];
				goto IL_0688;
			case 3:
				GameCanvas.yb[0] = gH - GameCanvas.bgH[0] + 10;
				GameCanvas.yb[1] = GameCanvas.yb[0] + 80;
				GameCanvas.yb[2] = GameCanvas.yb[1] - GameCanvas.bgH[2] - 10;
				goto IL_0688;
			case 4:
				GameCanvas.yb[0] = gH - GameCanvas.bgH[0] + 130;
				GameCanvas.yb[1] = GameCanvas.yb[0] - GameCanvas.bgH[1];
				GameCanvas.yb[2] = GameCanvas.yb[1] - GameCanvas.bgH[2] - 20;
				GameCanvas.yb[3] = GameCanvas.yb[1] - GameCanvas.bgH[2] - 80;
				goto IL_0688;
			case 5:
				GameCanvas.yb[0] = gH - GameCanvas.bgH[0] + 40;
				GameCanvas.yb[1] = GameCanvas.yb[0] - GameCanvas.bgH[1] + 10;
				GameCanvas.yb[2] = GameCanvas.yb[1] - GameCanvas.bgH[2] + 15;
				GameCanvas.yb[3] = GameCanvas.yb[2] - GameCanvas.bgH[3] + 50;
				goto IL_0688;
			case 6:
				GameCanvas.yb[0] = gH - GameCanvas.bgH[0] + 100;
				GameCanvas.yb[1] = GameCanvas.yb[0] - GameCanvas.bgH[1] - 30;
				GameCanvas.yb[2] = GameCanvas.yb[1] - GameCanvas.bgH[2] + 10;
				GameCanvas.yb[3] = GameCanvas.yb[2] - GameCanvas.bgH[3] + 15;
				GameCanvas.yb[4] = GameCanvas.yb[3] - GameCanvas.bgH[4] + 15;
				goto IL_0688;
			case 7:
				GameCanvas.yb[0] = gH - GameCanvas.bgH[0] + 20;
				GameCanvas.yb[1] = GameCanvas.yb[0] - GameCanvas.bgH[1] + 15;
				GameCanvas.yb[2] = GameCanvas.yb[1] - GameCanvas.bgH[2] + 20;
				GameCanvas.yb[3] = GameCanvas.yb[1] - GameCanvas.bgH[2] - 10;
				goto IL_0688;
			case 8:
			{
				GameCanvas.yb[0] = gH - 103 + 150;
				bool flag = TileMap.mapID == 103;
				if (flag)
				{
					GameCanvas.yb[0] -= 100;
				}
				GameCanvas.yb[1] = GameCanvas.yb[0] - GameCanvas.bgH[1] - 10;
				GameCanvas.yb[2] = GameCanvas.yb[1] - GameCanvas.bgH[2] + 40;
				GameCanvas.yb[3] = GameCanvas.yb[2] - GameCanvas.bgH[3] + 10;
				goto IL_0688;
			}
			case 9:
				GameCanvas.yb[0] = gH - GameCanvas.bgH[0] + 100;
				GameCanvas.yb[1] = GameCanvas.yb[0] - GameCanvas.bgH[1] + 22;
				GameCanvas.yb[2] = GameCanvas.yb[1] - GameCanvas.bgH[2] + 50;
				GameCanvas.yb[3] = GameCanvas.yb[2] - GameCanvas.bgH[3];
				goto IL_0688;
			case 10:
				GameCanvas.yb[0] = gH - GameCanvas.bgH[0] - 45;
				GameCanvas.yb[1] = GameCanvas.yb[0] - GameCanvas.bgH[1] - 10;
				goto IL_0688;
			case 11:
				GameCanvas.yb[0] = gH - GameCanvas.bgH[0] + 60;
				GameCanvas.yb[1] = GameCanvas.yb[0] - GameCanvas.bgH[1] + 5;
				GameCanvas.yb[2] = GameCanvas.yb[1] - GameCanvas.bgH[2] - 15;
				goto IL_0688;
			case 12:
				GameCanvas.yb[0] = gH + 40;
				GameCanvas.yb[1] = GameCanvas.yb[0] - 40;
				GameCanvas.yb[2] = GameCanvas.yb[1] - 40;
				goto IL_0688;
			case 13:
				GameCanvas.yb[0] = gH - 80;
				GameCanvas.yb[1] = GameCanvas.yb[0];
				goto IL_0688;
			case 15:
				GameCanvas.yb[0] = gH - 20;
				GameCanvas.yb[1] = GameCanvas.yb[0] - 80;
				goto IL_0688;
			case 16:
				GameCanvas.yb[0] = gH - GameCanvas.bgH[0] + 75;
				GameCanvas.yb[1] = GameCanvas.yb[0] - GameCanvas.bgH[1] + 50;
				GameCanvas.yb[2] = GameCanvas.yb[1] - GameCanvas.bgH[2] + 50;
				GameCanvas.yb[3] = GameCanvas.yb[2] - GameCanvas.bgH[3] + 90;
				goto IL_0688;
			case 19:
				GameCanvas.yb[0] = gH - GameCanvas.bgH[0] + 150;
				GameCanvas.yb[1] = GameCanvas.yb[0] - GameCanvas.bgH[1] - 60;
				GameCanvas.yb[2] = GameCanvas.yb[1] - GameCanvas.bgH[2] - 40;
				GameCanvas.yb[3] = GameCanvas.yb[2] - GameCanvas.bgH[3] - 10;
				GameCanvas.yb[4] = GameCanvas.yb[3] - GameCanvas.bgH[4];
				goto IL_0688;
			}
			GameCanvas.yb[0] = gH - GameCanvas.bgH[0] + 75;
			GameCanvas.yb[1] = GameCanvas.yb[0] - GameCanvas.bgH[1] + 50;
			GameCanvas.yb[2] = GameCanvas.yb[1] - GameCanvas.bgH[2] + 50;
			GameCanvas.yb[3] = GameCanvas.yb[2] - GameCanvas.bgH[3] + 90;
			IL_0688:;
		}
		catch (Exception)
		{
			int gH2 = GameScr.gH23;
			for (int i = 0; i < GameCanvas.yb.Length; i++)
			{
				GameCanvas.yb[i] = 1;
			}
		}
	}

	// Token: 0x0600040F RID: 1039 RVA: 0x0004E568 File Offset: 0x0004C768
	public static void loadBG(int typeBG)
	{
		try
		{
			GameCanvas.isLoadBGok = true;
			bool flag = GameCanvas.typeBg == 12;
			if (flag)
			{
				BackgroudEffect.yfog = TileMap.pxh - 100;
			}
			else
			{
				BackgroudEffect.yfog = TileMap.pxh - 160;
			}
			BackgroudEffect.clearImage();
			GameCanvas.randomRaintEff(typeBG);
			bool flag2 = (TileMap.lastBgID == typeBG && TileMap.lastType == TileMap.bgType) || typeBG == -1;
			if (!flag2)
			{
				GameCanvas.transY = 12;
				TileMap.lastBgID = (int)((sbyte)typeBG);
				TileMap.lastType = (int)((sbyte)TileMap.bgType);
				GameCanvas.layerSpeed = new int[] { 1, 2, 3, 7, 8 };
				GameCanvas.moveX = new int[5];
				GameCanvas.moveXSpeed = new int[5];
				GameCanvas.typeBg = typeBG;
				GameCanvas.isBoltEff = false;
				GameScr.firstY = GameScr.cmy;
				GameCanvas.imgBG = null;
				GameCanvas.imgCloud = null;
				GameCanvas.imgSun = null;
				GameCanvas.imgCaycot = null;
				GameScr.firstY = -1;
				switch (GameCanvas.typeBg)
				{
				case 0:
				{
					GameCanvas.imgCaycot = GameCanvas.loadImageRMS("/bg/caycot.png");
					GameCanvas.layerSpeed = new int[] { 1, 3, 5, 7 };
					GameCanvas.nBg = 4;
					bool flag3 = TileMap.bgType == 2;
					if (flag3)
					{
						GameCanvas.transY = 8;
					}
					goto IL_033E;
				}
				case 1:
					GameCanvas.transY = 7;
					GameCanvas.nBg = 4;
					goto IL_033E;
				case 2:
				{
					int[] array = new int[5];
					array[2] = 1;
					GameCanvas.moveX = array;
					int[] array2 = new int[5];
					array2[2] = 2;
					GameCanvas.moveXSpeed = array2;
					GameCanvas.nBg = 5;
					goto IL_033E;
				}
				case 3:
					GameCanvas.nBg = 3;
					goto IL_033E;
				case 4:
				{
					BackgroudEffect.addEffect(3);
					int[] array3 = new int[5];
					array3[1] = 1;
					GameCanvas.moveX = array3;
					int[] array4 = new int[5];
					array4[1] = 1;
					GameCanvas.moveXSpeed = array4;
					GameCanvas.nBg = 4;
					goto IL_033E;
				}
				case 5:
					GameCanvas.nBg = 4;
					goto IL_033E;
				case 6:
				{
					int[] array5 = new int[5];
					array5[0] = 1;
					GameCanvas.moveX = array5;
					int[] array6 = new int[5];
					array6[0] = 2;
					GameCanvas.moveXSpeed = array6;
					GameCanvas.nBg = 5;
					goto IL_033E;
				}
				case 7:
					GameCanvas.nBg = 4;
					goto IL_033E;
				case 8:
					GameCanvas.transY = 8;
					GameCanvas.nBg = 4;
					goto IL_033E;
				case 9:
					BackgroudEffect.addEffect(9);
					GameCanvas.nBg = 4;
					goto IL_033E;
				case 10:
					GameCanvas.nBg = 2;
					goto IL_033E;
				case 11:
					GameCanvas.transY = 7;
					GameCanvas.layerSpeed[2] = 0;
					GameCanvas.nBg = 3;
					goto IL_033E;
				case 12:
				{
					int[] array7 = new int[5];
					array7[0] = 1;
					array7[1] = 1;
					GameCanvas.moveX = array7;
					int[] array8 = new int[5];
					array8[0] = 2;
					array8[1] = 1;
					GameCanvas.moveXSpeed = array8;
					GameCanvas.nBg = 3;
					goto IL_033E;
				}
				case 13:
					GameCanvas.nBg = 2;
					goto IL_033E;
				case 15:
					Res.outz("HELL");
					GameCanvas.nBg = 2;
					goto IL_033E;
				case 16:
					GameCanvas.layerSpeed = new int[] { 1, 3, 5, 7 };
					GameCanvas.nBg = 4;
					goto IL_033E;
				case 19:
				{
					int[] array9 = new int[5];
					array9[1] = 2;
					array9[2] = 1;
					GameCanvas.moveX = array9;
					int[] array10 = new int[5];
					array10[1] = 2;
					array10[2] = 1;
					GameCanvas.moveXSpeed = array10;
					GameCanvas.nBg = 5;
					goto IL_033E;
				}
				}
				GameCanvas.layerSpeed = new int[] { 1, 3, 5, 7 };
				GameCanvas.nBg = 4;
				IL_033E:
				bool flag4 = typeBG <= 16;
				if (flag4)
				{
					GameCanvas.skyColor = StaticObj.SKYCOLOR[GameCanvas.typeBg];
				}
				else
				{
					try
					{
						string text = "/bg/b" + GameCanvas.typeBg.ToString() + 3.ToString() + ".png";
						bool flag5 = TileMap.bgType != 0;
						if (flag5)
						{
							text = string.Concat(new string[]
							{
								"/bg/b",
								GameCanvas.typeBg.ToString(),
								3.ToString(),
								"-",
								TileMap.bgType.ToString(),
								".png"
							});
						}
						int[] array11 = new int[1];
						Image image = GameCanvas.loadImageRMS(text);
						image.getRGB(ref array11, 0, 1, mGraphics.getRealImageWidth(image) / 2, 0, 1, 1);
						GameCanvas.skyColor = array11[0];
					}
					catch (Exception)
					{
						GameCanvas.skyColor = StaticObj.SKYCOLOR[StaticObj.SKYCOLOR.Length - 1];
					}
				}
				GameCanvas.colorTop = new int[StaticObj.SKYCOLOR.Length];
				GameCanvas.colorBotton = new int[StaticObj.SKYCOLOR.Length];
				for (int i = 0; i < StaticObj.SKYCOLOR.Length; i++)
				{
					GameCanvas.colorTop[i] = StaticObj.SKYCOLOR[i];
					GameCanvas.colorBotton[i] = StaticObj.SKYCOLOR[i];
				}
				bool flag6 = GameCanvas.lowGraphic;
				if (flag6)
				{
					GameCanvas.tam = GameCanvas.loadImageRMS("/bg/b63.png");
				}
				else
				{
					GameCanvas.imgBG = new Image[GameCanvas.nBg];
					GameCanvas.bgW = new int[GameCanvas.nBg];
					GameCanvas.bgH = new int[GameCanvas.nBg];
					GameCanvas.colorBotton = new int[GameCanvas.nBg];
					GameCanvas.colorTop = new int[GameCanvas.nBg];
					bool flag7 = TileMap.bgType == 100;
					if (flag7)
					{
						GameCanvas.imgBG[0] = GameCanvas.loadImageRMS("/bg/b100.png");
						GameCanvas.imgBG[1] = GameCanvas.loadImageRMS("/bg/b100.png");
						GameCanvas.imgBG[2] = GameCanvas.loadImageRMS("/bg/b82-1.png");
						GameCanvas.imgBG[3] = GameCanvas.loadImageRMS("/bg/b93.png");
						for (int j = 0; j < GameCanvas.nBg; j++)
						{
							bool flag8 = GameCanvas.imgBG[j] != null;
							if (flag8)
							{
								int[] array12 = new int[1];
								GameCanvas.imgBG[j].getRGB(ref array12, 0, 1, mGraphics.getRealImageWidth(GameCanvas.imgBG[j]) / 2, 0, 1, 1);
								GameCanvas.colorTop[j] = array12[0];
								array12 = new int[1];
								GameCanvas.imgBG[j].getRGB(ref array12, 0, 1, mGraphics.getRealImageWidth(GameCanvas.imgBG[j]) / 2, mGraphics.getRealImageHeight(GameCanvas.imgBG[j]) - 1, 1, 1);
								GameCanvas.colorBotton[j] = array12[0];
								GameCanvas.bgW[j] = mGraphics.getImageWidth(GameCanvas.imgBG[j]);
								GameCanvas.bgH[j] = mGraphics.getImageHeight(GameCanvas.imgBG[j]);
							}
							else
							{
								bool flag9 = GameCanvas.nBg > 1;
								if (flag9)
								{
									GameCanvas.imgBG[j] = GameCanvas.loadImageRMS("/bg/b" + GameCanvas.typeBg.ToString() + "0.png");
									GameCanvas.bgW[j] = mGraphics.getImageWidth(GameCanvas.imgBG[j]);
									GameCanvas.bgH[j] = mGraphics.getImageHeight(GameCanvas.imgBG[j]);
								}
							}
						}
					}
					else
					{
						for (int k = 0; k < GameCanvas.nBg; k++)
						{
							string text2 = "/bg/b" + GameCanvas.typeBg.ToString() + k.ToString() + ".png";
							bool flag10 = TileMap.bgType != 0;
							if (flag10)
							{
								text2 = string.Concat(new string[]
								{
									"/bg/b",
									GameCanvas.typeBg.ToString(),
									k.ToString(),
									"-",
									TileMap.bgType.ToString(),
									".png"
								});
							}
							GameCanvas.imgBG[k] = GameCanvas.loadImageRMS(text2);
							bool flag11 = GameCanvas.imgBG[k] != null;
							if (flag11)
							{
								int[] array13 = new int[1];
								GameCanvas.imgBG[k].getRGB(ref array13, 0, 1, mGraphics.getRealImageWidth(GameCanvas.imgBG[k]) / 2, 0, 1, 1);
								GameCanvas.colorTop[k] = array13[0];
								array13 = new int[1];
								GameCanvas.imgBG[k].getRGB(ref array13, 0, 1, mGraphics.getRealImageWidth(GameCanvas.imgBG[k]) / 2, mGraphics.getRealImageHeight(GameCanvas.imgBG[k]) - 1, 1, 1);
								GameCanvas.colorBotton[k] = array13[0];
								GameCanvas.bgW[k] = mGraphics.getImageWidth(GameCanvas.imgBG[k]);
								GameCanvas.bgH[k] = mGraphics.getImageHeight(GameCanvas.imgBG[k]);
							}
							else
							{
								bool flag12 = GameCanvas.nBg > 1;
								if (flag12)
								{
									GameCanvas.imgBG[k] = GameCanvas.loadImageRMS("/bg/b" + GameCanvas.typeBg.ToString() + "0.png");
									GameCanvas.bgW[k] = mGraphics.getImageWidth(GameCanvas.imgBG[k]);
									GameCanvas.bgH[k] = mGraphics.getImageHeight(GameCanvas.imgBG[k]);
								}
							}
						}
					}
					GameCanvas.getYBackground(GameCanvas.typeBg);
					GameCanvas.cloudX = new int[]
					{
						GameScr.gW / 2 - 40,
						GameScr.gW / 2 + 40,
						GameScr.gW / 2 - 100,
						GameScr.gW / 2 - 80,
						GameScr.gW / 2 - 120
					};
					GameCanvas.cloudY = new int[] { 130, 100, 150, 140, 80 };
					GameCanvas.imgSunSpec = null;
					bool flag13 = GameCanvas.typeBg != 0;
					if (flag13)
					{
						bool flag14 = GameCanvas.typeBg == 2;
						if (flag14)
						{
							GameCanvas.imgSun = GameCanvas.loadImageRMS("/bg/sun0.png");
							GameCanvas.sunX = GameScr.gW / 2 + 50;
							GameCanvas.sunY = GameCanvas.yb[4] - 40;
							TileMap.imgWaterflow = GameCanvas.loadImageRMS("/tWater/wts");
						}
						else
						{
							bool flag15 = GameCanvas.typeBg == 19;
							if (flag15)
							{
								TileMap.imgWaterflow = GameCanvas.loadImageRMS("/tWater/water_flow_32");
							}
							else
							{
								bool flag16 = GameCanvas.typeBg == 4;
								if (flag16)
								{
									GameCanvas.imgSun = GameCanvas.loadImageRMS("/bg/sun2.png");
									GameCanvas.sunX = GameScr.gW / 2 + 30;
									GameCanvas.sunY = GameCanvas.yb[3];
								}
								else
								{
									bool flag17 = GameCanvas.typeBg == 7;
									if (flag17)
									{
										GameCanvas.imgSun = GameCanvas.loadImageRMS("/bg/sun3" + ((TileMap.bgType != 0) ? ("-" + TileMap.bgType.ToString()) : string.Empty) + ".png");
										GameCanvas.imgSun2 = GameCanvas.loadImageRMS("/bg/sun4" + ((TileMap.bgType != 0) ? ("-" + TileMap.bgType.ToString()) : string.Empty) + ".png");
										GameCanvas.sunX = GameScr.gW - GameScr.gW / 3;
										GameCanvas.sunY = GameCanvas.yb[3] - 80;
										GameCanvas.sunX2 = GameCanvas.sunX - 100;
										GameCanvas.sunY2 = GameCanvas.yb[3] - 30;
									}
									else
									{
										bool flag18 = GameCanvas.typeBg == 6;
										if (flag18)
										{
											GameCanvas.imgSun = GameCanvas.loadImageRMS("/bg/sun5" + ((TileMap.bgType != 0) ? ("-" + TileMap.bgType.ToString()) : string.Empty) + ".png");
											GameCanvas.imgSun2 = GameCanvas.loadImageRMS("/bg/sun6" + ((TileMap.bgType != 0) ? ("-" + TileMap.bgType.ToString()) : string.Empty) + ".png");
											GameCanvas.sunX = GameScr.gW - GameScr.gW / 3;
											GameCanvas.sunY = GameCanvas.yb[4];
											GameCanvas.sunX2 = GameCanvas.sunX - 100;
											GameCanvas.sunY2 = GameCanvas.yb[4] + 20;
										}
										else
										{
											bool flag19 = typeBG == 5;
											if (flag19)
											{
												GameCanvas.imgSun = GameCanvas.loadImageRMS("/bg/sun8" + ((TileMap.bgType != 0) ? ("-" + TileMap.bgType.ToString()) : string.Empty) + ".png");
												GameCanvas.imgSun2 = GameCanvas.loadImageRMS("/bg/sun7" + ((TileMap.bgType != 0) ? ("-" + TileMap.bgType.ToString()) : string.Empty) + ".png");
												GameCanvas.sunX = GameScr.gW / 2 - 50;
												GameCanvas.sunY = GameCanvas.yb[3] + 20;
												GameCanvas.sunX2 = GameScr.gW / 2 + 20;
												GameCanvas.sunY2 = GameCanvas.yb[3] - 30;
											}
											else
											{
												bool flag20 = GameCanvas.typeBg == 8 && TileMap.mapID < 90;
												if (flag20)
												{
													GameCanvas.imgSun = GameCanvas.loadImageRMS("/bg/sun9" + ((TileMap.bgType != 0) ? ("-" + TileMap.bgType.ToString()) : string.Empty) + ".png");
													GameCanvas.imgSun2 = GameCanvas.loadImageRMS("/bg/sun10" + ((TileMap.bgType != 0) ? ("-" + TileMap.bgType.ToString()) : string.Empty) + ".png");
													GameCanvas.sunX = GameScr.gW / 2 - 30;
													GameCanvas.sunY = GameCanvas.yb[3] + 60;
													GameCanvas.sunX2 = GameScr.gW / 2 + 20;
													GameCanvas.sunY2 = GameCanvas.yb[3] + 10;
												}
												else
												{
													switch (typeBG)
													{
													case 9:
														GameCanvas.imgSun = GameCanvas.loadImageRMS("/bg/sun11" + ((TileMap.bgType != 0) ? ("-" + TileMap.bgType.ToString()) : string.Empty) + ".png");
														GameCanvas.imgSun2 = GameCanvas.loadImageRMS("/bg/sun12" + ((TileMap.bgType != 0) ? ("-" + TileMap.bgType.ToString()) : string.Empty) + ".png");
														GameCanvas.sunX = GameScr.gW - GameScr.gW / 3;
														GameCanvas.sunY = GameCanvas.yb[4] + 20;
														GameCanvas.sunX2 = GameCanvas.sunX - 80;
														GameCanvas.sunY2 = GameCanvas.yb[4] + 40;
														goto IL_1119;
													case 10:
														GameCanvas.imgSun = GameCanvas.loadImageRMS("/bg/sun13" + ((TileMap.bgType != 0) ? ("-" + TileMap.bgType.ToString()) : string.Empty) + ".png");
														GameCanvas.imgSun2 = GameCanvas.loadImageRMS("/bg/sun14" + ((TileMap.bgType != 0) ? ("-" + TileMap.bgType.ToString()) : string.Empty) + ".png");
														GameCanvas.sunX = GameScr.gW - GameScr.gW / 3;
														GameCanvas.sunY = GameCanvas.yb[1] - 30;
														GameCanvas.sunX2 = GameCanvas.sunX - 80;
														GameCanvas.sunY2 = GameCanvas.yb[1];
														goto IL_1119;
													case 11:
														GameCanvas.imgSun = GameCanvas.loadImageRMS("/bg/sun15" + ((TileMap.bgType != 0) ? ("-" + TileMap.bgType.ToString()) : string.Empty) + ".png");
														GameCanvas.imgSun2 = GameCanvas.loadImageRMS("/bg/b113" + ((TileMap.bgType != 0) ? ("-" + TileMap.bgType.ToString()) : string.Empty) + ".png");
														GameCanvas.sunX = GameScr.gW / 2 - 30;
														GameCanvas.sunY = GameCanvas.yb[2] - 30;
														goto IL_1119;
													case 12:
														GameCanvas.cloudY = new int[] { 200, 170, 220, 150, 250 };
														goto IL_1119;
													case 16:
													{
														GameCanvas.cloudX = new int[] { 90, 170, 250, 320, 400, 450, 500 };
														GameCanvas.cloudY = new int[]
														{
															GameCanvas.yb[2] + 5,
															GameCanvas.yb[2] - 20,
															GameCanvas.yb[2] - 50,
															GameCanvas.yb[2] - 30,
															GameCanvas.yb[2] - 50,
															GameCanvas.yb[2],
															GameCanvas.yb[2] - 40
														};
														GameCanvas.imgSunSpec = new Image[7];
														for (int l = 0; l < GameCanvas.imgSunSpec.Length; l++)
														{
															int num = 161;
															bool flag21 = l == 0 || l == 2 || l == 3 || l == 2 || l == 6;
															if (flag21)
															{
																num = 160;
															}
															GameCanvas.imgSunSpec[l] = GameCanvas.loadImageRMS("/bg/sun" + num.ToString() + ".png");
														}
														goto IL_1119;
													}
													case 19:
													{
														int[] array14 = new int[5];
														array14[1] = 2;
														array14[2] = 1;
														GameCanvas.moveX = array14;
														int[] array15 = new int[5];
														array15[1] = 2;
														array15[2] = 1;
														GameCanvas.moveXSpeed = array15;
														GameCanvas.nBg = 5;
														goto IL_1119;
													}
													}
													GameCanvas.imgCloud = null;
													GameCanvas.imgSun = null;
													GameCanvas.imgSun2 = null;
													GameCanvas.imgSun = GameCanvas.loadImageRMS("/bg/sun" + typeBG.ToString() + ((TileMap.bgType != 0) ? ("-" + TileMap.bgType.ToString()) : string.Empty) + ".png");
													bool flag22 = GameCanvas.loadImageRMS("/tWater/water_flow_" + typeBG.ToString()) != null;
													if (flag22)
													{
														TileMap.imgWaterflow = GameCanvas.loadImageRMS("/tWater/water_flow_" + typeBG.ToString());
													}
													GameCanvas.sunX = GameScr.gW - GameScr.gW / 3;
													GameCanvas.sunY = GameCanvas.yb[2] - 30;
													IL_1119:;
												}
											}
										}
									}
								}
							}
						}
					}
					GameCanvas.paintBG = false;
					bool flag23 = !GameCanvas.paintBG;
					if (flag23)
					{
						GameCanvas.paintBG = true;
					}
				}
			}
		}
		catch (Exception)
		{
			GameCanvas.isLoadBGok = false;
		}
	}

	// Token: 0x06000410 RID: 1040 RVA: 0x0004F6F0 File Offset: 0x0004D8F0
	private static void randomRaintEff(int typeBG)
	{
		for (int i = 0; i < GameCanvas.bgRain.Length; i++)
		{
			bool flag = typeBG == GameCanvas.bgRain[i] && Res.random(0, 2) == 0;
			if (flag)
			{
				BackgroudEffect.addEffect(0);
				break;
			}
		}
	}

	// Token: 0x06000411 RID: 1041 RVA: 0x0004F73C File Offset: 0x0004D93C
	public void keyPressedz(int keyCode)
	{
		GameCanvas.lastTimePress = mSystem.currentTimeMillis();
		bool flag = (keyCode >= 48 && keyCode <= 57) || (keyCode >= 65 && keyCode <= 122) || keyCode == 10 || keyCode == 8 || keyCode == 13 || keyCode == 32 || keyCode == 31;
		if (flag)
		{
			GameCanvas.keyAsciiPress = keyCode;
		}
		this.mapKeyPress(keyCode);
	}

	// Token: 0x06000412 RID: 1042 RVA: 0x0004F798 File Offset: 0x0004D998
	public void mapKeyPress(int keyCode)
	{
		bool flag = GameCanvas.currentDialog != null;
		if (flag)
		{
			GameCanvas.currentDialog.keyPress(keyCode);
			GameCanvas.keyAsciiPress = 0;
		}
		else
		{
			GameCanvas.currentScreen.keyPress(keyCode);
			if (keyCode <= -22)
			{
				if (keyCode <= -38)
				{
					if (keyCode == -39)
					{
						goto IL_0179;
					}
					if (keyCode != -38)
					{
						return;
					}
				}
				else
				{
					if (keyCode == -26)
					{
						GameCanvas.keyHold[16] = true;
						GameCanvas.keyPressed[16] = true;
						return;
					}
					if (keyCode != -22)
					{
						return;
					}
					goto IL_03FF;
				}
			}
			else
			{
				if (keyCode <= -1)
				{
					if (keyCode != -21)
					{
						switch (keyCode)
						{
						case -8:
							GameCanvas.keyHold[14] = true;
							GameCanvas.keyPressed[14] = true;
							return;
						case -7:
							goto IL_03FF;
						case -6:
							break;
						case -5:
							goto IL_0275;
						case -4:
						{
							bool flag2 = (GameCanvas.currentScreen is GameScr || GameCanvas.currentScreen is CrackBallScr) && global::Char.myCharz().isAttack;
							if (flag2)
							{
								GameCanvas.clearKeyHold();
								GameCanvas.clearKeyPressed();
							}
							else
							{
								GameCanvas.keyHold[24] = true;
								GameCanvas.keyPressed[24] = true;
							}
							return;
						}
						case -3:
						{
							bool flag3 = (GameCanvas.currentScreen is GameScr || GameCanvas.currentScreen is CrackBallScr) && global::Char.myCharz().isAttack;
							if (flag3)
							{
								GameCanvas.clearKeyHold();
								GameCanvas.clearKeyPressed();
							}
							else
							{
								GameCanvas.keyHold[23] = true;
								GameCanvas.keyPressed[23] = true;
							}
							return;
						}
						case -2:
							goto IL_0179;
						case -1:
							goto IL_0127;
						default:
							return;
						}
					}
					GameCanvas.keyHold[12] = true;
					GameCanvas.keyPressed[12] = true;
					return;
				}
				if (keyCode != 10)
				{
					switch (keyCode)
					{
					case 35:
						GameCanvas.keyHold[11] = true;
						GameCanvas.keyPressed[11] = true;
						return;
					case 36:
					case 37:
					case 38:
					case 39:
					case 40:
					case 41:
					case 43:
					case 44:
					case 45:
					case 46:
					case 47:
						return;
					case 42:
						GameCanvas.keyHold[10] = true;
						GameCanvas.keyPressed[10] = true;
						return;
					case 48:
						GameCanvas.keyHold[0] = true;
						GameCanvas.keyPressed[0] = true;
						return;
					case 49:
					{
						bool flag4 = GameCanvas.currentScreen == CrackBallScr.instance || (GameCanvas.currentScreen == GameScr.instance && GameCanvas.isMoveNumberPad && !ChatTextField.gI().isShow);
						if (flag4)
						{
							GameCanvas.keyHold[1] = true;
							GameCanvas.keyPressed[1] = true;
						}
						return;
					}
					case 50:
					{
						bool flag5 = GameCanvas.currentScreen == CrackBallScr.instance || (GameCanvas.currentScreen == GameScr.instance && GameCanvas.isMoveNumberPad && !ChatTextField.gI().isShow);
						if (flag5)
						{
							GameCanvas.keyHold[2] = true;
							GameCanvas.keyPressed[2] = true;
						}
						return;
					}
					case 51:
					{
						bool flag6 = GameCanvas.currentScreen == CrackBallScr.instance || (GameCanvas.currentScreen == GameScr.instance && GameCanvas.isMoveNumberPad && !ChatTextField.gI().isShow);
						if (flag6)
						{
							GameCanvas.keyHold[3] = true;
							GameCanvas.keyPressed[3] = true;
						}
						return;
					}
					case 52:
					{
						bool flag7 = GameCanvas.currentScreen == CrackBallScr.instance || (GameCanvas.currentScreen == GameScr.instance && GameCanvas.isMoveNumberPad && !ChatTextField.gI().isShow);
						if (flag7)
						{
							GameCanvas.keyHold[4] = true;
							GameCanvas.keyPressed[4] = true;
						}
						return;
					}
					case 53:
					{
						bool flag8 = GameCanvas.currentScreen == CrackBallScr.instance || (GameCanvas.currentScreen == GameScr.instance && GameCanvas.isMoveNumberPad && !ChatTextField.gI().isShow);
						if (flag8)
						{
							GameCanvas.keyHold[5] = true;
							GameCanvas.keyPressed[5] = true;
						}
						return;
					}
					case 54:
					{
						bool flag9 = GameCanvas.currentScreen == CrackBallScr.instance || (GameCanvas.currentScreen == GameScr.instance && GameCanvas.isMoveNumberPad && !ChatTextField.gI().isShow);
						if (flag9)
						{
							GameCanvas.keyHold[6] = true;
							GameCanvas.keyPressed[6] = true;
						}
						return;
					}
					case 55:
						GameCanvas.keyHold[7] = true;
						GameCanvas.keyPressed[7] = true;
						return;
					case 56:
					{
						bool flag10 = GameCanvas.currentScreen == CrackBallScr.instance || (GameCanvas.currentScreen == GameScr.instance && GameCanvas.isMoveNumberPad && !ChatTextField.gI().isShow);
						if (flag10)
						{
							GameCanvas.keyHold[8] = true;
							GameCanvas.keyPressed[8] = true;
						}
						return;
					}
					case 57:
						GameCanvas.keyHold[9] = true;
						GameCanvas.keyPressed[9] = true;
						return;
					default:
						if (keyCode != 113)
						{
							return;
						}
						GameCanvas.keyHold[17] = true;
						GameCanvas.keyPressed[17] = true;
						return;
					}
				}
				IL_0275:
				bool flag11 = (GameCanvas.currentScreen is GameScr || GameCanvas.currentScreen is CrackBallScr) && global::Char.myCharz().isAttack;
				if (flag11)
				{
					GameCanvas.clearKeyHold();
					GameCanvas.clearKeyPressed();
					return;
				}
				GameCanvas.keyHold[25] = true;
				GameCanvas.keyPressed[25] = true;
				GameCanvas.keyHold[15] = true;
				GameCanvas.keyPressed[15] = true;
				return;
			}
			IL_0127:
			bool flag12 = (GameCanvas.currentScreen is GameScr || GameCanvas.currentScreen is CrackBallScr) && global::Char.myCharz().isAttack;
			if (flag12)
			{
				GameCanvas.clearKeyHold();
				GameCanvas.clearKeyPressed();
			}
			else
			{
				GameCanvas.keyHold[21] = true;
				GameCanvas.keyPressed[21] = true;
			}
			return;
			IL_0179:
			bool flag13 = (GameCanvas.currentScreen is GameScr || GameCanvas.currentScreen is CrackBallScr) && global::Char.myCharz().isAttack;
			if (flag13)
			{
				GameCanvas.clearKeyHold();
				GameCanvas.clearKeyPressed();
			}
			else
			{
				GameCanvas.keyHold[22] = true;
				GameCanvas.keyPressed[22] = true;
			}
			return;
			IL_03FF:
			GameCanvas.keyHold[13] = true;
			GameCanvas.keyPressed[13] = true;
		}
	}

	// Token: 0x06000413 RID: 1043 RVA: 0x00004AD7 File Offset: 0x00002CD7
	public void keyReleasedz(int keyCode)
	{
		GameCanvas.keyAsciiPress = 0;
		this.mapKeyRelease(keyCode);
	}

	// Token: 0x06000414 RID: 1044 RVA: 0x0004FD80 File Offset: 0x0004DF80
	public void mapKeyRelease(int keyCode)
	{
		if (keyCode > -22)
		{
			if (keyCode <= -1)
			{
				if (keyCode != -21)
				{
					switch (keyCode)
					{
					case -8:
						GameCanvas.keyHold[14] = false;
						return;
					case -7:
						goto IL_0278;
					case -6:
						break;
					case -5:
						goto IL_012F;
					case -4:
						GameCanvas.keyHold[24] = false;
						return;
					case -3:
						GameCanvas.keyHold[23] = false;
						return;
					case -2:
						goto IL_0105;
					case -1:
						goto IL_00F7;
					default:
						return;
					}
				}
				GameCanvas.keyHold[12] = false;
				GameCanvas.keyReleased[12] = true;
				return;
			}
			if (keyCode != 10)
			{
				switch (keyCode)
				{
				case 35:
					GameCanvas.keyHold[11] = false;
					GameCanvas.keyReleased[11] = true;
					return;
				case 36:
				case 37:
				case 38:
				case 39:
				case 40:
				case 41:
				case 43:
				case 44:
				case 45:
				case 46:
				case 47:
					return;
				case 42:
					GameCanvas.keyHold[10] = false;
					GameCanvas.keyReleased[10] = true;
					return;
				case 48:
					GameCanvas.keyHold[0] = false;
					GameCanvas.keyReleased[0] = true;
					return;
				case 49:
				{
					bool flag = GameCanvas.currentScreen == CrackBallScr.instance || (GameCanvas.currentScreen == GameScr.instance && GameCanvas.isMoveNumberPad && !ChatTextField.gI().isShow);
					if (flag)
					{
						GameCanvas.keyHold[1] = false;
						GameCanvas.keyReleased[1] = true;
					}
					return;
				}
				case 50:
				{
					bool flag2 = GameCanvas.currentScreen == CrackBallScr.instance || (GameCanvas.currentScreen == GameScr.instance && GameCanvas.isMoveNumberPad && !ChatTextField.gI().isShow);
					if (flag2)
					{
						GameCanvas.keyHold[2] = false;
						GameCanvas.keyReleased[2] = true;
					}
					return;
				}
				case 51:
				{
					bool flag3 = GameCanvas.currentScreen == CrackBallScr.instance || (GameCanvas.currentScreen == GameScr.instance && GameCanvas.isMoveNumberPad && !ChatTextField.gI().isShow);
					if (flag3)
					{
						GameCanvas.keyHold[3] = false;
						GameCanvas.keyReleased[3] = true;
					}
					return;
				}
				case 52:
				{
					bool flag4 = GameCanvas.currentScreen == CrackBallScr.instance || (GameCanvas.currentScreen == GameScr.instance && GameCanvas.isMoveNumberPad && !ChatTextField.gI().isShow);
					if (flag4)
					{
						GameCanvas.keyHold[4] = false;
						GameCanvas.keyReleased[4] = true;
					}
					return;
				}
				case 53:
				{
					bool flag5 = GameCanvas.currentScreen == CrackBallScr.instance || (GameCanvas.currentScreen == GameScr.instance && GameCanvas.isMoveNumberPad && !ChatTextField.gI().isShow);
					if (flag5)
					{
						GameCanvas.keyHold[5] = false;
						GameCanvas.keyReleased[5] = true;
					}
					return;
				}
				case 54:
				{
					bool flag6 = GameCanvas.currentScreen == CrackBallScr.instance || (GameCanvas.currentScreen == GameScr.instance && GameCanvas.isMoveNumberPad && !ChatTextField.gI().isShow);
					if (flag6)
					{
						GameCanvas.keyHold[6] = false;
						GameCanvas.keyReleased[6] = true;
					}
					return;
				}
				case 55:
					GameCanvas.keyHold[7] = false;
					GameCanvas.keyReleased[7] = true;
					return;
				case 56:
				{
					bool flag7 = GameCanvas.currentScreen == CrackBallScr.instance || (GameCanvas.currentScreen == GameScr.instance && GameCanvas.isMoveNumberPad && !ChatTextField.gI().isShow);
					if (flag7)
					{
						GameCanvas.keyHold[8] = false;
						GameCanvas.keyReleased[8] = true;
					}
					return;
				}
				case 57:
					GameCanvas.keyHold[9] = false;
					GameCanvas.keyReleased[9] = true;
					return;
				default:
					if (keyCode != 113)
					{
						return;
					}
					GameCanvas.keyHold[17] = false;
					GameCanvas.keyReleased[17] = true;
					return;
				}
			}
			IL_012F:
			GameCanvas.keyHold[25] = false;
			GameCanvas.keyReleased[25] = true;
			GameCanvas.keyHold[15] = true;
			GameCanvas.keyPressed[15] = true;
			return;
		}
		if (keyCode <= -38)
		{
			if (keyCode == -39)
			{
				goto IL_0105;
			}
			if (keyCode != -38)
			{
				return;
			}
		}
		else
		{
			if (keyCode == -26)
			{
				GameCanvas.keyHold[16] = false;
				return;
			}
			if (keyCode != -22)
			{
				return;
			}
			goto IL_0278;
		}
		IL_00F7:
		GameCanvas.keyHold[21] = false;
		return;
		IL_0105:
		GameCanvas.keyHold[22] = false;
		return;
		IL_0278:
		GameCanvas.keyHold[13] = false;
		GameCanvas.keyReleased[13] = true;
	}

	// Token: 0x06000415 RID: 1045 RVA: 0x00004AE8 File Offset: 0x00002CE8
	public void pointerMouse(int x, int y)
	{
		GameCanvas.pxMouse = x;
		GameCanvas.pyMouse = y;
	}

	// Token: 0x06000416 RID: 1046 RVA: 0x000501CC File Offset: 0x0004E3CC
	public void scrollMouse(int a)
	{
		GameCanvas.pXYScrollMouse = a;
		bool flag = GameCanvas.panel != null && GameCanvas.panel.isShow;
		if (flag)
		{
			GameCanvas.panel.updateScroolMouse(a);
		}
	}

	// Token: 0x06000417 RID: 1047 RVA: 0x00050208 File Offset: 0x0004E408
	public void pointerDragged(int x, int y)
	{
		GameCanvas.isPointerSelect = false;
		bool flag = Res.abs(x - GameCanvas.pxLast) >= 10 || Res.abs(y - GameCanvas.pyLast) >= 10;
		if (flag)
		{
			GameCanvas.isPointerClick = false;
			GameCanvas.isPointerDown = true;
			GameCanvas.isPointerMove = true;
		}
		GameCanvas.px = x;
		GameCanvas.py = y;
		GameCanvas.curPos++;
		bool flag2 = GameCanvas.curPos > 3;
		if (flag2)
		{
			GameCanvas.curPos = 0;
		}
		GameCanvas.arrPos[GameCanvas.curPos] = new Position(x, y);
	}

	// Token: 0x06000418 RID: 1048 RVA: 0x00050298 File Offset: 0x0004E498
	public static bool isHoldPress()
	{
		return mSystem.currentTimeMillis() - GameCanvas.lastTimePress >= 800L;
	}

	// Token: 0x06000419 RID: 1049 RVA: 0x000502CC File Offset: 0x0004E4CC
	public void pointerPressed(int x, int y)
	{
		GameCanvas.isPointerSelect = false;
		GameCanvas.isPointerJustRelease = false;
		GameCanvas.isPointerJustDown = true;
		GameCanvas.isPointerDown = true;
		GameCanvas.isPointerClick = false;
		GameCanvas.isPointerMove = false;
		GameCanvas.lastTimePress = mSystem.currentTimeMillis();
		GameCanvas.pxFirst = x;
		GameCanvas.pyFirst = y;
		GameCanvas.pxLast = x;
		GameCanvas.pyLast = y;
		GameCanvas.px = x;
		GameCanvas.py = y;
	}

	// Token: 0x0600041A RID: 1050 RVA: 0x0005032C File Offset: 0x0004E52C
	public void pointerReleased(int x, int y)
	{
		bool flag = !GameCanvas.isPointerMove;
		if (flag)
		{
			GameCanvas.isPointerSelect = true;
		}
		GameCanvas.isPointerDown = false;
		GameCanvas.isPointerMove = false;
		GameCanvas.isPointerJustRelease = true;
		GameCanvas.isPointerClick = true;
		mScreen.keyTouch = -1;
		GameCanvas.px = x;
		GameCanvas.py = y;
	}

	// Token: 0x0600041B RID: 1051 RVA: 0x00050378 File Offset: 0x0004E578
	public static bool isPointerHoldIn(int x, int y, int w, int h)
	{
		bool flag = !GameCanvas.isPointerDown && !GameCanvas.isPointerJustRelease;
		bool flag2;
		if (flag)
		{
			flag2 = false;
		}
		else
		{
			bool flag3 = GameCanvas.px >= x && GameCanvas.px <= x + w && GameCanvas.py >= y && GameCanvas.py <= y + h;
			flag2 = flag3;
		}
		return flag2;
	}

	// Token: 0x0600041C RID: 1052 RVA: 0x000503DC File Offset: 0x0004E5DC
	public static bool isPointSelect(int x, int y, int w, int h)
	{
		bool flag = !GameCanvas.isPointerSelect;
		bool flag2;
		if (flag)
		{
			flag2 = false;
		}
		else
		{
			bool flag3 = GameCanvas.px >= x && GameCanvas.px <= x + w && GameCanvas.py >= y && GameCanvas.py <= y + h;
			flag2 = flag3;
		}
		return flag2;
	}

	// Token: 0x0600041D RID: 1053 RVA: 0x00050434 File Offset: 0x0004E634
	public static bool isMouseFocus(int x, int y, int w, int h)
	{
		return GameCanvas.pxMouse >= x && GameCanvas.pxMouse <= x + w && GameCanvas.pyMouse >= y && GameCanvas.pyMouse <= y + h;
	}

	// Token: 0x0600041E RID: 1054 RVA: 0x0005047C File Offset: 0x0004E67C
	public static void clearKeyPressed()
	{
		for (int i = 0; i < GameCanvas.keyPressed.Length; i++)
		{
			GameCanvas.keyPressed[i] = false;
		}
		GameCanvas.isPointerJustRelease = false;
	}

	// Token: 0x0600041F RID: 1055 RVA: 0x000504B0 File Offset: 0x0004E6B0
	public static void clearKeyHold()
	{
		for (int i = 0; i < GameCanvas.keyHold.Length; i++)
		{
			GameCanvas.keyHold[i] = false;
		}
	}

	// Token: 0x06000420 RID: 1056 RVA: 0x000504E0 File Offset: 0x0004E6E0
	public static void checkBackButton()
	{
		bool flag = ChatPopup.serverChatPopUp == null && ChatPopup.currChatPopup == null;
		if (flag)
		{
			GameCanvas.startYesNoDlg(mResources.DOYOUWANTEXIT, new Command(mResources.YES, GameCanvas.instance, 8885, null), new Command(mResources.NO, GameCanvas.instance, 8882, null));
		}
	}

	// Token: 0x06000421 RID: 1057 RVA: 0x0005053C File Offset: 0x0004E73C
	public void paintChangeMap(mGraphics g)
	{
		string empty = string.Empty;
		GameCanvas.resetTrans(g);
		g.setColor(0);
		g.fillRect(0, 0, GameCanvas.w, GameCanvas.h);
		g.drawImage(LoginScr.imgTitle, GameCanvas.w / 2, GameCanvas.h / 2 - 24, StaticObj.BOTTOM_HCENTER);
		GameCanvas.paintShukiren(GameCanvas.hw, GameCanvas.h / 2 + 24, g);
		mFont.tahoma_7b_white.drawString(g, mResources.PLEASEWAIT + ((LoginScr.timeLogin <= 0) ? empty : (" " + LoginScr.timeLogin.ToString() + "s")), GameCanvas.w / 2, GameCanvas.h / 2, 2);
	}

	// Token: 0x06000422 RID: 1058 RVA: 0x000505F4 File Offset: 0x0004E7F4
	public void paint(mGraphics gx)
	{
		try
		{
			GameCanvas.debugPaint.removeAllElements();
			GameCanvas.debug("PA", 1);
			bool flag = GameCanvas.currentScreen != null;
			if (flag)
			{
				GameCanvas.currentScreen.paint(this.g);
			}
			GameCanvas.debug("PB", 1);
			this.g.translate(-this.g.getTranslateX(), -this.g.getTranslateY());
			this.g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
			bool isShow = GameCanvas.panel.isShow;
			if (isShow)
			{
				GameCanvas.panel.paint(this.g);
				bool flag2 = GameCanvas.panel2 != null && GameCanvas.panel2.isShow;
				if (flag2)
				{
					GameCanvas.panel2.paint(this.g);
				}
				bool flag3 = GameCanvas.panel.chatTField != null && GameCanvas.panel.chatTField.isShow;
				if (flag3)
				{
					GameCanvas.panel.chatTField.paint(this.g);
				}
				bool flag4 = GameCanvas.panel2 != null && GameCanvas.panel2.chatTField != null && GameCanvas.panel2.chatTField.isShow;
				if (flag4)
				{
					GameCanvas.panel2.chatTField.paint(this.g);
				}
			}
			Res.paintOnScreenDebug(this.g);
			InfoDlg.paint(this.g);
			bool flag5 = GameCanvas.currentDialog != null;
			if (flag5)
			{
				GameCanvas.debug("PC", 1);
				GameCanvas.currentDialog.paint(this.g);
			}
			else
			{
				bool showMenu = GameCanvas.menu.showMenu;
				if (showMenu)
				{
					GameCanvas.debug("PD", 1);
					GameCanvas.resetTrans(this.g);
					GameCanvas.menu.paintMenu(this.g);
				}
			}
			GameScr.info1.paint(this.g);
			GameScr.info2.paint(this.g);
			bool flag6 = GameScr.gI().popUpYesNo != null;
			if (flag6)
			{
				GameScr.gI().popUpYesNo.paint(this.g);
			}
			bool flag7 = ChatPopup.currChatPopup != null;
			if (flag7)
			{
				ChatPopup.currChatPopup.paint(this.g);
			}
			Hint.paint(this.g);
			bool flag8 = ChatPopup.serverChatPopUp != null;
			if (flag8)
			{
				ChatPopup.serverChatPopUp.paint(this.g);
			}
			for (int i = 0; i < Effect2.vEffect2.size(); i++)
			{
				Effect2 effect = (Effect2)Effect2.vEffect2.elementAt(i);
				bool flag9 = effect is ChatPopup && !effect.Equals(ChatPopup.currChatPopup) && !effect.Equals(ChatPopup.serverChatPopUp);
				if (flag9)
				{
					effect.paint(this.g);
				}
			}
			bool flag10 = GameCanvas.currentDialog != null;
			if (flag10)
			{
				GameCanvas.currentDialog.paint(this.g);
			}
			bool flag11 = GameCanvas.isWait();
			if (flag11)
			{
				this.paintChangeMap(this.g);
				bool flag12 = GameCanvas.timeLoading > 0 && LoginScr.timeLogin <= 0 && mSystem.currentTimeMillis() - GameCanvas.TIMEOUT >= 1000L;
				if (flag12)
				{
					GameCanvas.timeLoading--;
					bool flag13 = GameCanvas.timeLoading == 0;
					if (flag13)
					{
						GameCanvas.timeLoading = 15;
					}
					GameCanvas.TIMEOUT = mSystem.currentTimeMillis();
				}
			}
			GameCanvas.debug("PE", 1);
			GameCanvas.resetTrans(this.g);
			EffecMn.paintLayer4(this.g);
			bool flag14 = GameCanvas.open3Hour && !GameCanvas.isLoading;
			if (flag14)
			{
				bool flag15 = GameCanvas.currentScreen == GameCanvas.loginScr || GameCanvas.currentScreen == GameCanvas.serverScreen || GameCanvas.currentScreen == GameCanvas.serverScr;
				if (flag15)
				{
					this.g.drawImage(GameCanvas.img12, 5, 5, 0);
				}
				bool flag16 = GameCanvas.currentScreen == CreateCharScr.instance;
				if (flag16)
				{
					this.g.drawImage(GameCanvas.img12, 5, 20, 0);
				}
			}
			GameCanvas.resetTrans(this.g);
			int num = GameCanvas.h / 4;
			bool flag17 = GameCanvas.currentScreen != null && GameCanvas.currentScreen is GameScr && GameCanvas.thongBaoTest != null;
			if (flag17)
			{
				this.g.setClip(60, num, GameCanvas.w - 120, mFont.tahoma_7_white.getHeight() + 2);
				mFont.tahoma_7_grey.drawString(this.g, GameCanvas.thongBaoTest, GameCanvas.xThongBaoTranslate, num + 1, 0);
				mFont.tahoma_7_yellow.drawString(this.g, GameCanvas.thongBaoTest, GameCanvas.xThongBaoTranslate, num, 0);
				this.g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
			}
		}
		catch (Exception)
		{
		}
	}

	// Token: 0x06000423 RID: 1059 RVA: 0x00050AF8 File Offset: 0x0004ECF8
	public static void endDlg()
	{
		bool flag = GameCanvas.inputDlg != null;
		if (flag)
		{
			GameCanvas.inputDlg.tfInput.setMaxTextLenght(500);
		}
		GameCanvas.currentDialog = null;
		InfoDlg.hide();
	}

	// Token: 0x06000424 RID: 1060 RVA: 0x00050B38 File Offset: 0x0004ED38
	public static void startOKDlg(string info)
	{
		bool flag = info == "Không thể đổi khu vực trong map này";
		if (!flag)
		{
			GameCanvas.closeKeyBoard();
			GameCanvas.msgdlg.setInfo(info, null, new Command(mResources.OK, GameCanvas.instance, 8882, null), null);
			GameCanvas.currentDialog = GameCanvas.msgdlg;
		}
	}

	// Token: 0x06000425 RID: 1061 RVA: 0x00050B8C File Offset: 0x0004ED8C
	public static void startWaitDlg(string info)
	{
		GameCanvas.closeKeyBoard();
		GameCanvas.msgdlg.setInfo(info, null, new Command(mResources.CANCEL, GameCanvas.instance, 8882, null), null);
		GameCanvas.currentDialog = GameCanvas.msgdlg;
		GameCanvas.msgdlg.isWait = true;
	}

	// Token: 0x06000426 RID: 1062 RVA: 0x00050B8C File Offset: 0x0004ED8C
	public static void startOKDlg(string info, bool isError)
	{
		GameCanvas.closeKeyBoard();
		GameCanvas.msgdlg.setInfo(info, null, new Command(mResources.CANCEL, GameCanvas.instance, 8882, null), null);
		GameCanvas.currentDialog = GameCanvas.msgdlg;
		GameCanvas.msgdlg.isWait = true;
	}

	// Token: 0x06000427 RID: 1063 RVA: 0x00004AF7 File Offset: 0x00002CF7
	public static void startWaitDlg()
	{
		GameCanvas.closeKeyBoard();
		global::Char.isLoadingMap = true;
	}

	// Token: 0x06000428 RID: 1064 RVA: 0x00004B06 File Offset: 0x00002D06
	public void openWeb(string strLeft, string strRight, string url, string str)
	{
		GameCanvas.msgdlg.setInfo(str, new Command(strLeft, this, 8881, url), null, new Command(strRight, this, 8882, null));
		GameCanvas.currentDialog = GameCanvas.msgdlg;
	}

	// Token: 0x06000429 RID: 1065 RVA: 0x00004B3B File Offset: 0x00002D3B
	public static void startOK(string info, int actionID, object p)
	{
		GameCanvas.closeKeyBoard();
		GameCanvas.msgdlg.setInfo(info, null, new Command(mResources.OK, GameCanvas.instance, actionID, p), null);
		GameCanvas.msgdlg.show();
	}

	// Token: 0x0600042A RID: 1066 RVA: 0x00050BD8 File Offset: 0x0004EDD8
	public static void startYesNoDlg(string info, int iYes, object pYes, int iNo, object pNo)
	{
		GameCanvas.closeKeyBoard();
		GameCanvas.msgdlg.setInfo(info, new Command(mResources.YES, GameCanvas.instance, iYes, pYes), new Command(string.Empty, GameCanvas.instance, iYes, pYes), new Command(mResources.NO, GameCanvas.instance, iNo, pNo));
		GameCanvas.msgdlg.show();
	}

	// Token: 0x0600042B RID: 1067 RVA: 0x00004B6E File Offset: 0x00002D6E
	public static void startYesNoDlg(string info, Command cmdYes, Command cmdNo)
	{
		GameCanvas.closeKeyBoard();
		GameCanvas.msgdlg.setInfo(info, cmdYes, null, cmdNo);
		GameCanvas.msgdlg.show();
	}

	// Token: 0x0600042C RID: 1068 RVA: 0x00004B91 File Offset: 0x00002D91
	public static void startserverThongBao(string msgSv)
	{
		GameCanvas.thongBaoTest = msgSv;
		GameCanvas.xThongBaoTranslate = GameCanvas.w - 60;
		GameCanvas.dir_ = -1;
	}

	// Token: 0x0600042D RID: 1069 RVA: 0x00050C38 File Offset: 0x0004EE38
	public static string getMoneys(int m)
	{
		string text = string.Empty;
		int num = m / 1000 + 1;
		for (int i = 0; i < num; i++)
		{
			bool flag = m >= 1000;
			if (!flag)
			{
				text = m.ToString() + text;
				break;
			}
			int num2 = m % 1000;
			text = ((num2 != 0) ? ((num2 >= 10) ? ((num2 >= 100) ? ("." + num2.ToString() + text) : (".0" + num2.ToString() + text)) : (".00" + num2.ToString() + text)) : (".000" + text));
			m /= 1000;
		}
		return text;
	}

	// Token: 0x0600042E RID: 1070 RVA: 0x00050D04 File Offset: 0x0004EF04
	public static int getX(int start, int w)
	{
		return (GameCanvas.px - start) / w;
	}

	// Token: 0x0600042F RID: 1071 RVA: 0x00050D20 File Offset: 0x0004EF20
	public static int getY(int start, int w)
	{
		return (GameCanvas.py - start) / w;
	}

	// Token: 0x06000430 RID: 1072 RVA: 0x00003E4C File Offset: 0x0000204C
	protected void sizeChanged(int w, int h)
	{
	}

	// Token: 0x06000431 RID: 1073 RVA: 0x00050D3C File Offset: 0x0004EF3C
	public static bool isGetResourceFromServer()
	{
		return true;
	}

	// Token: 0x06000432 RID: 1074 RVA: 0x00050D50 File Offset: 0x0004EF50
	public static Image loadImageRMS(string path)
	{
		path = Main.res + "/x" + mGraphics.zoomLevel.ToString() + path;
		path = GameCanvas.cutPng(path);
		Image image = null;
		try
		{
			image = Image.createImage(path);
		}
		catch (Exception ex)
		{
			try
			{
				string[] array = Res.split(path, "/", 0);
				string text = "x" + mGraphics.zoomLevel.ToString() + array[array.Length - 1];
				sbyte[] array2 = Rms.loadRMS(text);
				bool flag = array2 != null;
				if (flag)
				{
					image = Image.createImage(array2, 0, array2.Length);
				}
			}
			catch (Exception)
			{
				Cout.LogError("Loi ham khong tim thay a: " + ex.ToString());
			}
		}
		return image;
	}

	// Token: 0x06000433 RID: 1075 RVA: 0x00050E24 File Offset: 0x0004F024
	public static Image loadImage(string path)
	{
		path = Main.res + "/x" + mGraphics.zoomLevel.ToString() + path;
		path = GameCanvas.cutPng(path);
		Image image = null;
		try
		{
			image = Image.createImage(path);
		}
		catch (Exception)
		{
		}
		return image;
	}

	// Token: 0x06000434 RID: 1076 RVA: 0x00050E80 File Offset: 0x0004F080
	public static string cutPng(string str)
	{
		string text = str;
		bool flag = str.Contains(".png");
		if (flag)
		{
			text = str.Replace(".png", string.Empty);
		}
		return text;
	}

	// Token: 0x06000435 RID: 1077 RVA: 0x00050EB8 File Offset: 0x0004F0B8
	public static int random(int a, int b)
	{
		return a + GameCanvas.r.nextInt(b - a);
	}

	// Token: 0x06000436 RID: 1078 RVA: 0x00050EDC File Offset: 0x0004F0DC
	public bool startDust(int dir, int x, int y)
	{
		bool flag = GameCanvas.lowGraphic;
		bool flag2;
		if (flag)
		{
			flag2 = false;
		}
		else
		{
			int num = ((dir != 1) ? 1 : 0);
			bool flag3 = this.dustState[num] != -1;
			if (flag3)
			{
				flag2 = false;
			}
			else
			{
				this.dustState[num] = 0;
				this.dustX[num] = x;
				this.dustY[num] = y;
				flag2 = true;
			}
		}
		return flag2;
	}

	// Token: 0x06000437 RID: 1079 RVA: 0x00050F38 File Offset: 0x0004F138
	public void loadWaterSplash()
	{
		bool flag = !GameCanvas.lowGraphic;
		if (flag)
		{
			GameCanvas.imgWS = new Image[3];
			for (int i = 0; i < 3; i++)
			{
				GameCanvas.imgWS[i] = GameCanvas.loadImage("/e/w" + i.ToString() + ".png");
			}
			GameCanvas.wsX = new int[2];
			GameCanvas.wsY = new int[2];
			GameCanvas.wsState = new int[2];
			GameCanvas.wsF = new int[2];
			GameCanvas.wsState[0] = (GameCanvas.wsState[1] = -1);
		}
	}

	// Token: 0x06000438 RID: 1080 RVA: 0x00050FD4 File Offset: 0x0004F1D4
	public bool startWaterSplash(int x, int y)
	{
		bool flag = GameCanvas.lowGraphic;
		bool flag2;
		if (flag)
		{
			flag2 = false;
		}
		else
		{
			int num = ((GameCanvas.wsState[0] != -1) ? 1 : 0);
			bool flag3 = GameCanvas.wsState[num] != -1;
			if (flag3)
			{
				flag2 = false;
			}
			else
			{
				GameCanvas.wsState[num] = 0;
				GameCanvas.wsX[num] = x;
				GameCanvas.wsY[num] = y;
				flag2 = true;
			}
		}
		return flag2;
	}

	// Token: 0x06000439 RID: 1081 RVA: 0x00051034 File Offset: 0x0004F234
	public void updateWaterSplash()
	{
		bool flag = GameCanvas.lowGraphic;
		if (!flag)
		{
			for (int i = 0; i < 2; i++)
			{
				bool flag2 = GameCanvas.wsState[i] == -1;
				if (!flag2)
				{
					GameCanvas.wsY[i]--;
					bool flag3 = GameCanvas.gameTick % 2 == 0;
					if (flag3)
					{
						GameCanvas.wsState[i]++;
						bool flag4 = GameCanvas.wsState[i] > 2;
						if (flag4)
						{
							GameCanvas.wsState[i] = -1;
						}
						else
						{
							GameCanvas.wsF[i] = GameCanvas.wsState[i];
						}
					}
				}
			}
		}
	}

	// Token: 0x0600043A RID: 1082 RVA: 0x000510D4 File Offset: 0x0004F2D4
	public void updateDust()
	{
		bool flag = GameCanvas.lowGraphic;
		if (!flag)
		{
			for (int i = 0; i < 2; i++)
			{
				bool flag2 = this.dustState[i] != -1;
				if (flag2)
				{
					this.dustState[i]++;
					bool flag3 = this.dustState[i] >= 5;
					if (flag3)
					{
						this.dustState[i] = -1;
					}
					bool flag4 = i == 0;
					if (flag4)
					{
						this.dustX[i]--;
					}
					else
					{
						this.dustX[i]++;
					}
					this.dustY[i]--;
				}
			}
		}
	}

	// Token: 0x0600043B RID: 1083 RVA: 0x00051190 File Offset: 0x0004F390
	public static bool isPaint(int x, int y)
	{
		bool flag = x < GameScr.cmx;
		bool flag2;
		if (flag)
		{
			flag2 = false;
		}
		else
		{
			bool flag3 = x > GameScr.cmx + GameScr.gW;
			if (flag3)
			{
				flag2 = false;
			}
			else
			{
				bool flag4 = y < GameScr.cmy;
				if (flag4)
				{
					flag2 = false;
				}
				else
				{
					bool flag5 = y > GameScr.cmy + GameScr.gH + 30;
					flag2 = !flag5;
				}
			}
		}
		return flag2;
	}

	// Token: 0x0600043C RID: 1084 RVA: 0x000511F8 File Offset: 0x0004F3F8
	public void paintDust(mGraphics g)
	{
		bool flag = GameCanvas.lowGraphic;
		if (!flag)
		{
			for (int i = 0; i < 2; i++)
			{
				bool flag2 = this.dustState[i] != -1 && GameCanvas.isPaint(this.dustX[i], this.dustY[i]);
				if (flag2)
				{
					g.drawImage(GameCanvas.imgDust[i][this.dustState[i]], this.dustX[i], this.dustY[i], 3);
				}
			}
		}
	}

	// Token: 0x0600043D RID: 1085 RVA: 0x00051278 File Offset: 0x0004F478
	public void loadDust()
	{
		bool flag = GameCanvas.lowGraphic;
		if (!flag)
		{
			bool flag2 = GameCanvas.imgDust == null;
			if (flag2)
			{
				GameCanvas.imgDust = new Image[2][];
				for (int i = 0; i < GameCanvas.imgDust.Length; i++)
				{
					GameCanvas.imgDust[i] = new Image[5];
				}
				for (int j = 0; j < 2; j++)
				{
					for (int k = 0; k < 5; k++)
					{
						GameCanvas.imgDust[j][k] = GameCanvas.loadImage("/e/d" + j.ToString() + k.ToString() + ".png");
					}
				}
			}
			this.dustX = new int[2];
			this.dustY = new int[2];
			this.dustState = new int[2];
			this.dustState[0] = (this.dustState[1] = -1);
		}
	}

	// Token: 0x0600043E RID: 1086 RVA: 0x00051370 File Offset: 0x0004F570
	public static void paintShukiren(int x, int y, mGraphics g)
	{
		g.drawRegion(GameCanvas.imgShuriken, 0, Main.f * 16, 16, 16, 0, x, y, mGraphics.HCENTER | mGraphics.VCENTER);
	}

	// Token: 0x0600043F RID: 1087 RVA: 0x00004BAD File Offset: 0x00002DAD
	public void resetToLoginScrz()
	{
		this.resetToLoginScr = true;
	}

	// Token: 0x06000440 RID: 1088 RVA: 0x00050378 File Offset: 0x0004E578
	public static bool isPointer(int x, int y, int w, int h)
	{
		bool flag = !GameCanvas.isPointerDown && !GameCanvas.isPointerJustRelease;
		bool flag2;
		if (flag)
		{
			flag2 = false;
		}
		else
		{
			bool flag3 = GameCanvas.px >= x && GameCanvas.px <= x + w && GameCanvas.py >= y && GameCanvas.py <= y + h;
			flag2 = flag3;
		}
		return flag2;
	}

	// Token: 0x06000441 RID: 1089 RVA: 0x000513A8 File Offset: 0x0004F5A8
	public void perform(int idAction, object p)
	{
		if (idAction <= 88839)
		{
			if (idAction <= 8889)
			{
				if (idAction == 999)
				{
					mSystem.closeBanner();
					GameCanvas.endDlg();
					return;
				}
				switch (idAction)
				{
				case 8881:
				{
					string text = (string)p;
					try
					{
						GameMidlet.instance.platformRequest(text);
					}
					catch (Exception)
					{
					}
					GameCanvas.currentDialog = null;
					return;
				}
				case 8882:
					InfoDlg.hide();
					GameCanvas.currentDialog = null;
					ServerListScreen.isAutoConect = false;
					ServerListScreen.countDieConnect = 0;
					return;
				case 8883:
					return;
				case 8884:
				{
					GameCanvas.endDlg();
					bool flag = GameCanvas.serverScr == null;
					if (flag)
					{
						GameCanvas.serverScr = new ServerScr();
					}
					GameCanvas.serverScr.switchToMe();
					return;
				}
				case 8885:
					GameMidlet.instance.exit();
					return;
				case 8886:
				{
					GameCanvas.endDlg();
					string text2 = (string)p;
					Service.gI().addFriend(text2);
					return;
				}
				case 8887:
				{
					GameCanvas.endDlg();
					int num = (int)p;
					Service.gI().addPartyAccept(num);
					return;
				}
				case 8888:
				{
					int num2 = (int)p;
					Service.gI().addPartyCancel(num2);
					GameCanvas.endDlg();
					return;
				}
				case 8889:
				{
					string text3 = (string)p;
					GameCanvas.endDlg();
					Service.gI().acceptPleaseParty(text3);
					return;
				}
				default:
					return;
				}
			}
			else
			{
				if (idAction == 9000)
				{
					GameCanvas.endDlg();
					SplashScr.imgLogo = null;
					SmallImage.loadBigRMS();
					mSystem.gcc();
					ServerListScreen.bigOk = true;
					ServerListScreen.loadScreen = true;
					GameScr.gI().loadGameScr();
					bool flag2 = GameCanvas.currentScreen != GameCanvas.loginScr;
					if (flag2)
					{
						GameCanvas.serverScreen.switchToMe2();
					}
					return;
				}
				if (idAction == 9999)
				{
					GameCanvas.endDlg();
					GameCanvas.connect();
					Service.gI().setClientType();
					bool flag3 = GameCanvas.loginScr == null;
					if (flag3)
					{
						GameCanvas.loginScr = new LoginScr();
					}
					GameCanvas.loginScr.doLogin();
					return;
				}
				switch (idAction)
				{
				case 88810:
				{
					int num3 = (int)p;
					GameCanvas.endDlg();
					Service.gI().acceptInviteTrade(num3);
					return;
				}
				case 88811:
					GameCanvas.endDlg();
					Service.gI().cancelInviteTrade();
					return;
				case 88812:
				case 88813:
				case 88815:
				case 88816:
				case 88830:
				case 88831:
				case 88832:
				case 88833:
				case 88834:
				case 88835:
				case 88838:
					return;
				case 88814:
				{
					Item[] array = (Item[])p;
					GameCanvas.endDlg();
					Service.gI().crystalCollectLock(array);
					return;
				}
				case 88817:
					ChatPopup.addChatPopup(string.Empty, 1, global::Char.myCharz().npcFocus);
					Service.gI().menu(global::Char.myCharz().npcFocus.template.npcTemplateId, GameCanvas.menu.menuSelectedItem, 0);
					return;
				case 88818:
				{
					short num4 = (short)p;
					Service.gI().textBoxId(num4, GameCanvas.inputDlg.tfInput.getText());
					GameCanvas.endDlg();
					return;
				}
				case 88819:
				{
					short num5 = (short)p;
					Service.gI().menuId(num5);
					return;
				}
				case 88820:
				{
					string[] array2 = (string[])p;
					bool flag4 = global::Char.myCharz().npcFocus == null;
					if (flag4)
					{
						return;
					}
					int menuSelectedItem = GameCanvas.menu.menuSelectedItem;
					bool flag5 = array2.Length > 1;
					if (flag5)
					{
						MyVector myVector = new MyVector();
						for (int i = 0; i < array2.Length - 1; i++)
						{
							myVector.addElement(new Command(array2[i + 1], GameCanvas.instance, 88821, menuSelectedItem));
						}
						GameCanvas.menu.startAt(myVector, 3);
					}
					else
					{
						ChatPopup.addChatPopup(string.Empty, 1, global::Char.myCharz().npcFocus);
						Service.gI().menu(global::Char.myCharz().npcFocus.template.npcTemplateId, menuSelectedItem, 0);
					}
					return;
				}
				case 88821:
				{
					int num6 = (int)p;
					ChatPopup.addChatPopup(string.Empty, 1, global::Char.myCharz().npcFocus);
					Service.gI().menu(global::Char.myCharz().npcFocus.template.npcTemplateId, num6, GameCanvas.menu.menuSelectedItem);
					return;
				}
				case 88822:
					ChatPopup.addChatPopup(string.Empty, 1, global::Char.myCharz().npcFocus);
					Service.gI().menu(global::Char.myCharz().npcFocus.template.npcTemplateId, GameCanvas.menu.menuSelectedItem, 0);
					return;
				case 88823:
					GameCanvas.startOKDlg(mResources.SENTMSG);
					return;
				case 88824:
					GameCanvas.startOKDlg(mResources.NOSENDMSG);
					return;
				case 88825:
					GameCanvas.startOKDlg(mResources.sendMsgSuccess, false);
					return;
				case 88826:
					GameCanvas.startOKDlg(mResources.cannotSendMsg, false);
					return;
				case 88827:
					GameCanvas.startOKDlg(mResources.sendGuessMsgSuccess);
					return;
				case 88828:
					GameCanvas.startOKDlg(mResources.sendMsgFail);
					return;
				case 88829:
				{
					string text4 = GameCanvas.inputDlg.tfInput.getText();
					bool flag6 = !text4.Equals(string.Empty);
					if (flag6)
					{
						Service.gI().changeName(text4, (int)p);
						InfoDlg.showWait();
					}
					return;
				}
				case 88836:
					GameCanvas.inputDlg.tfInput.setMaxTextLenght(6);
					GameCanvas.inputDlg.show(mResources.INPUT_PRIVATE_PASS, new Command(mResources.ACCEPT, GameCanvas.instance, 888361, null), TField.INPUT_TYPE_NUMERIC);
					return;
				case 88837:
					break;
				case 88839:
					goto IL_0775;
				default:
					return;
				}
			}
		}
		else if (idAction <= 100016)
		{
			switch (idAction)
			{
			case 100001:
				Service.gI().getFlag(0, -1);
				InfoDlg.showWait();
				return;
			case 100002:
			{
				bool flag7 = GameCanvas.loginScr == null;
				if (flag7)
				{
					GameCanvas.loginScr = new LoginScr();
				}
				GameCanvas.loginScr.backToRegister();
				return;
			}
			case 100003:
			case 100004:
				return;
			case 100005:
			{
				bool flag8 = global::Char.myCharz().statusMe == 14;
				if (flag8)
				{
					GameCanvas.startOKDlg(mResources.can_not_do_when_die);
				}
				else
				{
					Service.gI().openUIZone();
				}
				return;
			}
			case 100006:
				mSystem.onDisconnected();
				return;
			default:
				if (idAction != 100016)
				{
					return;
				}
				ServerListScreen.SetIpSelect(17, false);
				GameCanvas.instance.doResetToLoginScr(GameCanvas.serverScreen);
				ServerListScreen.waitToLogin = true;
				GameCanvas.endDlg();
				return;
			}
		}
		else
		{
			switch (idAction)
			{
			case 101023:
				Main.numberQuit = 0;
				return;
			case 101024:
				Res.outz("output 101024");
				GameCanvas.endDlg();
				return;
			case 101025:
			{
				GameCanvas.endDlg();
				bool loadScreen = ServerListScreen.loadScreen;
				if (loadScreen)
				{
					GameCanvas.serverScreen.switchToMe();
				}
				else
				{
					GameCanvas.serverScreen.show2();
				}
				return;
			}
			case 101026:
				mSystem.onDisconnected();
				return;
			default:
				if (idAction != 888361)
				{
					switch (idAction)
					{
					case 888391:
						goto IL_07EE;
					case 888392:
						Service.gI().menu(4, GameCanvas.menu.menuSelectedItem, 0);
						return;
					case 888393:
					{
						bool flag9 = GameCanvas.loginScr == null;
						if (flag9)
						{
							GameCanvas.loginScr = new LoginScr();
						}
						GameCanvas.loginScr.doLogin();
						Main.closeKeyBoard();
						return;
					}
					case 888394:
						GameCanvas.endDlg();
						return;
					case 888395:
						GameCanvas.endDlg();
						return;
					case 888396:
						GameCanvas.endDlg();
						return;
					case 888397:
					{
						string text5 = (string)p;
						return;
					}
					default:
						return;
					}
				}
				else
				{
					string text6 = GameCanvas.inputDlg.tfInput.getText();
					GameCanvas.endDlg();
					bool flag10 = text6.Length < 6 || text6.Equals(string.Empty);
					if (flag10)
					{
						GameCanvas.startOKDlg(mResources.ALERT_PRIVATE_PASS_1);
						return;
					}
					try
					{
						Service.gI().activeAccProtect(int.Parse(text6));
						return;
					}
					catch (Exception ex)
					{
						GameCanvas.startOKDlg(mResources.ALERT_PRIVATE_PASS_2);
						Cout.println("Loi tai 888361 Gamescavas " + ex.ToString());
						return;
					}
				}
				break;
			}
		}
		string text7 = GameCanvas.inputDlg.tfInput.getText();
		GameCanvas.endDlg();
		try
		{
			Service.gI().openLockAccProtect(int.Parse(text7.Trim()));
			return;
		}
		catch (Exception ex2)
		{
			Cout.println("Loi tai 88837 " + ex2.ToString());
			return;
		}
		IL_0775:
		string text8 = GameCanvas.inputDlg.tfInput.getText();
		GameCanvas.endDlg();
		bool flag11 = text8.Length < 6 || text8.Equals(string.Empty);
		if (flag11)
		{
			GameCanvas.startOKDlg(mResources.ALERT_PRIVATE_PASS_1);
			return;
		}
		try
		{
			GameCanvas.startYesNoDlg(mResources.cancelAccountProtection, 888391, text8, 8882, null);
			return;
		}
		catch (Exception)
		{
			GameCanvas.startOKDlg(mResources.ALERT_PRIVATE_PASS_2);
			return;
		}
		IL_07EE:
		string text9 = (string)p;
		GameCanvas.endDlg();
		Service.gI().clearAccProtect(int.Parse(text9));
	}

	// Token: 0x06000442 RID: 1090 RVA: 0x00004BB7 File Offset: 0x00002DB7
	public static void clearAllPointerEvent()
	{
		GameCanvas.isPointerClick = false;
		GameCanvas.isPointerDown = false;
		GameCanvas.isPointerJustDown = false;
		GameCanvas.isPointerJustRelease = false;
		GameCanvas.isPointerSelect = false;
		GameScr.gI().lastSingleClick = 0L;
		GameScr.gI().isPointerDowning = false;
	}

	// Token: 0x06000443 RID: 1091 RVA: 0x00051D60 File Offset: 0x0004FF60
	public static bool isWait()
	{
		return global::Char.isLoadingMap || LoginScr.isContinueToLogin || ServerListScreen.waitToLogin || ServerListScreen.isWait || SelectCharScr.isWait;
	}

	// Token: 0x04000694 RID: 1684
	public static long timeNow = 0L;

	// Token: 0x04000695 RID: 1685
	public static bool open3Hour;

	// Token: 0x04000696 RID: 1686
	public static bool lowGraphic = true;

	// Token: 0x04000697 RID: 1687
	public static bool serverchat = false;

	// Token: 0x04000698 RID: 1688
	public static bool isMoveNumberPad = true;

	// Token: 0x04000699 RID: 1689
	public static bool isLoading;

	// Token: 0x0400069A RID: 1690
	public static bool isTouch = false;

	// Token: 0x0400069B RID: 1691
	public static bool isTouchControl;

	// Token: 0x0400069C RID: 1692
	public static bool isTouchControlSmallScreen;

	// Token: 0x0400069D RID: 1693
	public static bool isTouchControlLargeScreen;

	// Token: 0x0400069E RID: 1694
	public static bool isConnectFail;

	// Token: 0x0400069F RID: 1695
	public static GameCanvas instance;

	// Token: 0x040006A0 RID: 1696
	public static bool bRun;

	// Token: 0x040006A1 RID: 1697
	public static bool[] keyPressed = new bool[30];

	// Token: 0x040006A2 RID: 1698
	public static bool[] keyReleased = new bool[30];

	// Token: 0x040006A3 RID: 1699
	public static bool[] keyHold = new bool[30];

	// Token: 0x040006A4 RID: 1700
	public static bool isPointerDown;

	// Token: 0x040006A5 RID: 1701
	public static bool isPointerClick;

	// Token: 0x040006A6 RID: 1702
	public static bool isPointerJustRelease;

	// Token: 0x040006A7 RID: 1703
	public static bool isPointerSelect;

	// Token: 0x040006A8 RID: 1704
	public static bool isPointerMove;

	// Token: 0x040006A9 RID: 1705
	public static int px;

	// Token: 0x040006AA RID: 1706
	public static int py;

	// Token: 0x040006AB RID: 1707
	public static int pxFirst;

	// Token: 0x040006AC RID: 1708
	public static int pyFirst;

	// Token: 0x040006AD RID: 1709
	public static int pxLast;

	// Token: 0x040006AE RID: 1710
	public static int pyLast;

	// Token: 0x040006AF RID: 1711
	public static int pxMouse;

	// Token: 0x040006B0 RID: 1712
	public static int pyMouse;

	// Token: 0x040006B1 RID: 1713
	public static Position[] arrPos = new Position[4];

	// Token: 0x040006B2 RID: 1714
	public static int gameTick;

	// Token: 0x040006B3 RID: 1715
	public static int taskTick;

	// Token: 0x040006B4 RID: 1716
	public static bool isEff1;

	// Token: 0x040006B5 RID: 1717
	public static bool isEff2;

	// Token: 0x040006B6 RID: 1718
	public static long timeTickEff1;

	// Token: 0x040006B7 RID: 1719
	public static long timeTickEff2;

	// Token: 0x040006B8 RID: 1720
	public static int w;

	// Token: 0x040006B9 RID: 1721
	public static int h;

	// Token: 0x040006BA RID: 1722
	public static int hw;

	// Token: 0x040006BB RID: 1723
	public static int hh;

	// Token: 0x040006BC RID: 1724
	public static int wd3;

	// Token: 0x040006BD RID: 1725
	public static int hd3;

	// Token: 0x040006BE RID: 1726
	public static int w2d3;

	// Token: 0x040006BF RID: 1727
	public static int h2d3;

	// Token: 0x040006C0 RID: 1728
	public static int w3d4;

	// Token: 0x040006C1 RID: 1729
	public static int h3d4;

	// Token: 0x040006C2 RID: 1730
	public static int wd6;

	// Token: 0x040006C3 RID: 1731
	public static int hd6;

	// Token: 0x040006C4 RID: 1732
	public static mScreen currentScreen;

	// Token: 0x040006C5 RID: 1733
	public static Menu menu = new Menu();

	// Token: 0x040006C6 RID: 1734
	public static Panel panel;

	// Token: 0x040006C7 RID: 1735
	public static Panel panel2;

	// Token: 0x040006C8 RID: 1736
	public static ChooseCharScr chooseCharScr;

	// Token: 0x040006C9 RID: 1737
	public static LoginScr loginScr;

	// Token: 0x040006CA RID: 1738
	public static RegisterScreen registerScr;

	// Token: 0x040006CB RID: 1739
	public static Dialog currentDialog;

	// Token: 0x040006CC RID: 1740
	public static MsgDlg msgdlg;

	// Token: 0x040006CD RID: 1741
	public static InputDlg inputDlg;

	// Token: 0x040006CE RID: 1742
	public static MyVector currentPopup = new MyVector();

	// Token: 0x040006CF RID: 1743
	public static int requestLoseCount;

	// Token: 0x040006D0 RID: 1744
	public static MyVector listPoint;

	// Token: 0x040006D1 RID: 1745
	public static Paint paintz;

	// Token: 0x040006D2 RID: 1746
	public static bool isGetResFromServer;

	// Token: 0x040006D3 RID: 1747
	public static Image[] imgBG;

	// Token: 0x040006D4 RID: 1748
	public static int skyColor;

	// Token: 0x040006D5 RID: 1749
	public static int curPos = 0;

	// Token: 0x040006D6 RID: 1750
	public static int[] bgW;

	// Token: 0x040006D7 RID: 1751
	public static int[] bgH;

	// Token: 0x040006D8 RID: 1752
	public static int planet = 0;

	// Token: 0x040006D9 RID: 1753
	private mGraphics g = new mGraphics();

	// Token: 0x040006DA RID: 1754
	public static Image img12;

	// Token: 0x040006DB RID: 1755
	public static Image[] imgBlue = new Image[7];

	// Token: 0x040006DC RID: 1756
	public static Image[] imgViolet = new Image[7];

	// Token: 0x040006DD RID: 1757
	public static MyHashTable danhHieu = new MyHashTable();

	// Token: 0x040006DE RID: 1758
	public static MyVector messageServer = new MyVector(string.Empty);

	// Token: 0x040006DF RID: 1759
	public static bool isPlaySound = false;

	// Token: 0x040006E0 RID: 1760
	private static int clearOldData;

	// Token: 0x040006E1 RID: 1761
	public static int timeOpenKeyBoard;

	// Token: 0x040006E2 RID: 1762
	public static bool isFocusPanel2;

	// Token: 0x040006E3 RID: 1763
	public static int fps = 0;

	// Token: 0x040006E4 RID: 1764
	public static int max;

	// Token: 0x040006E5 RID: 1765
	public static int up;

	// Token: 0x040006E6 RID: 1766
	public static int upmax;

	// Token: 0x040006E7 RID: 1767
	private long timefps = mSystem.currentTimeMillis() + 1000L;

	// Token: 0x040006E8 RID: 1768
	private long timeup = mSystem.currentTimeMillis() + 1000L;

	// Token: 0x040006E9 RID: 1769
	public static int isRequestMapID = -1;

	// Token: 0x040006EA RID: 1770
	public static long waitingTimeChangeMap;

	// Token: 0x040006EB RID: 1771
	private static int dir_ = -1;

	// Token: 0x040006EC RID: 1772
	private int tickWaitThongBao;

	// Token: 0x040006ED RID: 1773
	public bool isPaintCarret;

	// Token: 0x040006EE RID: 1774
	public static MyVector debugUpdate;

	// Token: 0x040006EF RID: 1775
	public static MyVector debugPaint;

	// Token: 0x040006F0 RID: 1776
	public static MyVector debugSession;

	// Token: 0x040006F1 RID: 1777
	private static bool isShowErrorForm = false;

	// Token: 0x040006F2 RID: 1778
	public static bool paintBG;

	// Token: 0x040006F3 RID: 1779
	public static int gsskyHeight;

	// Token: 0x040006F4 RID: 1780
	public static int gsgreenField1Y;

	// Token: 0x040006F5 RID: 1781
	public static int gsgreenField2Y;

	// Token: 0x040006F6 RID: 1782
	public static int gshouseY;

	// Token: 0x040006F7 RID: 1783
	public static int gsmountainY;

	// Token: 0x040006F8 RID: 1784
	public static int bgLayer0y;

	// Token: 0x040006F9 RID: 1785
	public static int bgLayer1y;

	// Token: 0x040006FA RID: 1786
	public static Image imgCloud;

	// Token: 0x040006FB RID: 1787
	public static Image imgSun;

	// Token: 0x040006FC RID: 1788
	public static Image imgSun2;

	// Token: 0x040006FD RID: 1789
	public static Image imgClear;

	// Token: 0x040006FE RID: 1790
	public static Image[] imgBorder = new Image[3];

	// Token: 0x040006FF RID: 1791
	public static Image[] imgSunSpec = new Image[3];

	// Token: 0x04000700 RID: 1792
	public static int borderConnerW;

	// Token: 0x04000701 RID: 1793
	public static int borderConnerH;

	// Token: 0x04000702 RID: 1794
	public static int borderCenterW;

	// Token: 0x04000703 RID: 1795
	public static int borderCenterH;

	// Token: 0x04000704 RID: 1796
	public static int[] cloudX;

	// Token: 0x04000705 RID: 1797
	public static int[] cloudY;

	// Token: 0x04000706 RID: 1798
	public static int sunX;

	// Token: 0x04000707 RID: 1799
	public static int sunY;

	// Token: 0x04000708 RID: 1800
	public static int sunX2;

	// Token: 0x04000709 RID: 1801
	public static int sunY2;

	// Token: 0x0400070A RID: 1802
	public static int[] layerSpeed;

	// Token: 0x0400070B RID: 1803
	public static int[] moveX;

	// Token: 0x0400070C RID: 1804
	public static int[] moveXSpeed;

	// Token: 0x0400070D RID: 1805
	public static bool isBoltEff;

	// Token: 0x0400070E RID: 1806
	public static bool boltActive;

	// Token: 0x0400070F RID: 1807
	public static int tBolt;

	// Token: 0x04000710 RID: 1808
	public static Image imgBgIOS;

	// Token: 0x04000711 RID: 1809
	public static int typeBg = -1;

	// Token: 0x04000712 RID: 1810
	public static int transY;

	// Token: 0x04000713 RID: 1811
	public static int[] yb = new int[5];

	// Token: 0x04000714 RID: 1812
	public static int[] colorTop;

	// Token: 0x04000715 RID: 1813
	public static int[] colorBotton;

	// Token: 0x04000716 RID: 1814
	public static int yb1;

	// Token: 0x04000717 RID: 1815
	public static int yb2;

	// Token: 0x04000718 RID: 1816
	public static int yb3;

	// Token: 0x04000719 RID: 1817
	public static int nBg = 0;

	// Token: 0x0400071A RID: 1818
	public static int lastBg = -1;

	// Token: 0x0400071B RID: 1819
	public static int[] bgRain = new int[] { 1, 4, 11 };

	// Token: 0x0400071C RID: 1820
	public static int[] bgRainFont = new int[] { -1 };

	// Token: 0x0400071D RID: 1821
	public static Image imgCaycot;

	// Token: 0x0400071E RID: 1822
	public static Image tam;

	// Token: 0x0400071F RID: 1823
	public static int typeBackGround = -1;

	// Token: 0x04000720 RID: 1824
	public static int saveIDBg = -10;

	// Token: 0x04000721 RID: 1825
	public static bool isLoadBGok;

	// Token: 0x04000722 RID: 1826
	private static long lastTimePress = 0L;

	// Token: 0x04000723 RID: 1827
	public static int keyAsciiPress;

	// Token: 0x04000724 RID: 1828
	public static int pXYScrollMouse;

	// Token: 0x04000725 RID: 1829
	private static Image imgSignal;

	// Token: 0x04000726 RID: 1830
	public static MyVector flyTexts = new MyVector();

	// Token: 0x04000727 RID: 1831
	public int longTime;

	// Token: 0x04000728 RID: 1832
	public static long timeBreakLoading;

	// Token: 0x04000729 RID: 1833
	private static string thongBaoTest;

	// Token: 0x0400072A RID: 1834
	public static int xThongBaoTranslate = GameCanvas.w - 60;

	// Token: 0x0400072B RID: 1835
	public static bool isPointerJustDown = false;

	// Token: 0x0400072C RID: 1836
	private int count = 1;

	// Token: 0x0400072D RID: 1837
	public static bool csWait;

	// Token: 0x0400072E RID: 1838
	public static MyRandom r = new MyRandom();

	// Token: 0x0400072F RID: 1839
	public static bool isBlackScreen;

	// Token: 0x04000730 RID: 1840
	public static int[] bgSpeed;

	// Token: 0x04000731 RID: 1841
	public static int cmdBarX;

	// Token: 0x04000732 RID: 1842
	public static int cmdBarY;

	// Token: 0x04000733 RID: 1843
	public static int cmdBarW;

	// Token: 0x04000734 RID: 1844
	public static int cmdBarH;

	// Token: 0x04000735 RID: 1845
	public static int cmdBarLeftW;

	// Token: 0x04000736 RID: 1846
	public static int cmdBarRightW;

	// Token: 0x04000737 RID: 1847
	public static int cmdBarCenterW;

	// Token: 0x04000738 RID: 1848
	public static int hpBarX;

	// Token: 0x04000739 RID: 1849
	public static int hpBarY;

	// Token: 0x0400073A RID: 1850
	public static int hpBarW;

	// Token: 0x0400073B RID: 1851
	public static int expBarW;

	// Token: 0x0400073C RID: 1852
	public static int lvPosX;

	// Token: 0x0400073D RID: 1853
	public static int moneyPosX;

	// Token: 0x0400073E RID: 1854
	public static int hpBarH;

	// Token: 0x0400073F RID: 1855
	public static int girlHPBarY;

	// Token: 0x04000740 RID: 1856
	public int timeOut;

	// Token: 0x04000741 RID: 1857
	public int[] dustX;

	// Token: 0x04000742 RID: 1858
	public int[] dustY;

	// Token: 0x04000743 RID: 1859
	public int[] dustState;

	// Token: 0x04000744 RID: 1860
	public static int[] wsX;

	// Token: 0x04000745 RID: 1861
	public static int[] wsY;

	// Token: 0x04000746 RID: 1862
	public static int[] wsState;

	// Token: 0x04000747 RID: 1863
	public static int[] wsF;

	// Token: 0x04000748 RID: 1864
	public static Image[] imgWS;

	// Token: 0x04000749 RID: 1865
	public static Image imgShuriken;

	// Token: 0x0400074A RID: 1866
	public static Image[][] imgDust;

	// Token: 0x0400074B RID: 1867
	public static bool isResume;

	// Token: 0x0400074C RID: 1868
	public static ServerListScreen serverScreen;

	// Token: 0x0400074D RID: 1869
	public static ServerScr serverScr;

	// Token: 0x0400074E RID: 1870
	public static SelectCharScr _SelectCharScr;

	// Token: 0x0400074F RID: 1871
	public bool resetToLoginScr;

	// Token: 0x04000750 RID: 1872
	public static long TIMEOUT;

	// Token: 0x04000751 RID: 1873
	public static int timeLoading = 15;
}
using System;
using UnityEngine;

// Token: 0x0200005C RID: 92
public class GameMidlet
{
	// Token: 0x06000445 RID: 1093 RVA: 0x00004BEF File Offset: 0x00002DEF
	public GameMidlet()
	{
		this.initGame();
	}

	// Token: 0x06000446 RID: 1094 RVA: 0x00051EFC File Offset: 0x000500FC
	public void initGame()
	{
		GameMidlet.instance = this;
		MotherCanvas.instance = new MotherCanvas();
		Session_ME.gI().setHandler(Controller.gI());
		Session_ME2.gI().setHandler(Controller.gI());
		Session_ME2.isMainSession = false;
		GameMidlet.instance = this;
		GameMidlet.gameCanvas = new GameCanvas();
		GameMidlet.gameCanvas.start();
		SplashScr.loadImg();
		SplashScr.loadSplashScr();
		GameCanvas.currentScreen = new SplashScr();
	}

	// Token: 0x06000447 RID: 1095 RVA: 0x00051F74 File Offset: 0x00050174
	public void exit()
	{
		bool flag = Main.typeClient == 6;
		if (flag)
		{
			mSystem.exitWP();
		}
		else
		{
			GameCanvas.bRun = false;
			mSystem.gcc();
			this.notifyDestroyed();
		}
	}

	// Token: 0x06000448 RID: 1096 RVA: 0x00004C00 File Offset: 0x00002E00
	public static void sendSMS(string data, string to, Command successAction, Command failAction)
	{
		Cout.println("SEND SMS");
	}

	// Token: 0x06000449 RID: 1097 RVA: 0x00004C0E File Offset: 0x00002E0E
	public static void flatForm(string url)
	{
		Cout.LogWarning("PLATFORM REQUEST: " + url);
		Application.OpenURL(url);
	}

	// Token: 0x0600044A RID: 1098 RVA: 0x00004C29 File Offset: 0x00002E29
	public void notifyDestroyed()
	{
		Main.exit();
	}

	// Token: 0x0600044B RID: 1099 RVA: 0x00004C32 File Offset: 0x00002E32
	public void platformRequest(string url)
	{
		Cout.LogWarning("PLATFORM REQUEST: " + url);
		Application.OpenURL(url);
	}

	// Token: 0x04000752 RID: 1874
	public static string IP = "112.213.94.23";

	// Token: 0x04000753 RID: 1875
	public static int PORT = 14445;

	// Token: 0x04000754 RID: 1876
	public static string IP2;

	// Token: 0x04000755 RID: 1877
	public static int PORT2;

	// Token: 0x04000756 RID: 1878
	public static sbyte PROVIDER;

	// Token: 0x04000757 RID: 1879
	public static int LANGUAGE;

	// Token: 0x04000758 RID: 1880
	public static string VERSION = "2.4.3";

	// Token: 0x04000759 RID: 1881
	public static int intVERSION = 243;

	// Token: 0x0400075A RID: 1882
	public static GameCanvas gameCanvas;

	// Token: 0x0400075B RID: 1883
	public static GameMidlet instance;

	// Token: 0x0400075C RID: 1884
	public static bool isConnect2;

	// Token: 0x0400075D RID: 1885
	public static bool isBackWindowsPhone;
}
using System;

// Token: 0x0200005D RID: 93
public class GamePad
{
	// Token: 0x0600044D RID: 1101 RVA: 0x00051FAC File Offset: 0x000501AC
	public GamePad()
	{
		this.R = 28;
		bool flag = GameCanvas.w < 300;
		if (flag)
		{
			this.isSmallGamePad = true;
			this.isMediumGamePad = false;
			this.isLargeGamePad = false;
		}
		bool flag2 = GameCanvas.w >= 300 && GameCanvas.w <= 380;
		if (flag2)
		{
			this.isSmallGamePad = false;
			this.isMediumGamePad = true;
			this.isLargeGamePad = false;
		}
		bool flag3 = GameCanvas.w > 380;
		if (flag3)
		{
			this.isSmallGamePad = false;
			this.isMediumGamePad = false;
			this.isLargeGamePad = true;
		}
		bool flag4 = !this.isLargeGamePad;
		if (flag4)
		{
			this.xZone = 0;
			this.wZone = GameCanvas.hw;
			this.yZone = GameCanvas.hh >> 1;
			this.hZone = GameCanvas.h - 80;
		}
		else
		{
			this.xZone = 0;
			this.wZone = GameCanvas.hw / 4 * 3 - 20;
			this.yZone = GameCanvas.hh >> 1;
			this.hZone = GameCanvas.h;
			bool flag5 = mSystem.clientType == 2;
			if (flag5)
			{
				this.xZone = 0;
				this.yZone = (GameCanvas.h >> 1) + 40;
				this.wZone = GameCanvas.hw / 4 * 3 - 40;
				this.hZone = GameCanvas.h;
			}
		}
	}

	// Token: 0x0600044E RID: 1102 RVA: 0x00052104 File Offset: 0x00050304
	public void update()
	{
		try
		{
			bool flag = GameScr.isAnalog == 0;
			if (!flag)
			{
				bool flag2 = GameCanvas.isPointerDown && !GameCanvas.isPointerJustRelease;
				if (flag2)
				{
					this.xTemp = GameCanvas.pxFirst;
					this.yTemp = GameCanvas.pyFirst;
					bool flag3 = this.xTemp < this.xZone || this.xTemp > this.wZone || this.yTemp < this.yZone || this.yTemp > this.hZone;
					if (!flag3)
					{
						bool flag4 = !this.isGamePad;
						if (flag4)
						{
							this.xC = (this.xM = this.xTemp);
							this.yC = (this.yM = this.yTemp);
						}
						this.isGamePad = true;
						this.deltaX = GameCanvas.px - this.xC;
						this.deltaY = GameCanvas.py - this.yC;
						this.delta = global::Math.pow(this.deltaX, 2) + global::Math.pow(this.deltaY, 2);
						this.d = Res.sqrt(this.delta);
						bool flag5 = global::Math.abs(this.deltaX) <= 4 && global::Math.abs(this.deltaY) <= 4;
						if (!flag5)
						{
							this.angle = Res.angle(this.deltaX, this.deltaY);
							bool flag6 = !GameCanvas.isPointerHoldIn(this.xC - this.R, this.yC - this.R, 2 * this.R, 2 * this.R);
							if (flag6)
							{
								bool flag7 = this.d != 0;
								if (flag7)
								{
									this.yM = this.deltaY * this.R / this.d;
									this.xM = this.deltaX * this.R / this.d;
									this.xM += this.xC;
									this.yM += this.yC;
									bool flag8 = !Res.inRect(this.xC - this.R, this.yC - this.R, 2 * this.R, 2 * this.R, this.xM, this.yM);
									if (flag8)
									{
										this.xM = this.xMLast;
										this.yM = this.yMLast;
									}
									else
									{
										this.xMLast = this.xM;
										this.yMLast = this.yM;
									}
								}
								else
								{
									this.xM = this.xMLast;
									this.yM = this.yMLast;
								}
							}
							else
							{
								this.xM = GameCanvas.px;
								this.yM = GameCanvas.py;
							}
							this.resetHold();
							bool flag9 = this.checkPointerMove(2);
							if (flag9)
							{
								bool flag10 = (this.angle <= 360 && this.angle >= 340) || (this.angle >= 0 && this.angle <= 20);
								if (flag10)
								{
									GameCanvas.keyHold[(!Main.isPC) ? 6 : 24] = true;
									GameCanvas.keyPressed[(!Main.isPC) ? 6 : 24] = true;
								}
								else
								{
									bool flag11 = this.angle > 40 && this.angle < 70;
									if (flag11)
									{
										GameCanvas.keyHold[(!Main.isPC) ? 6 : 24] = true;
										GameCanvas.keyPressed[(!Main.isPC) ? 6 : 24] = true;
									}
									else
									{
										bool flag12 = this.angle >= 70 && this.angle <= 110;
										if (flag12)
										{
											GameCanvas.keyHold[(!Main.isPC) ? 8 : 22] = true;
											GameCanvas.keyPressed[(!Main.isPC) ? 8 : 22] = true;
										}
										else
										{
											bool flag13 = this.angle > 110 && this.angle < 120;
											if (flag13)
											{
												GameCanvas.keyHold[(!Main.isPC) ? 4 : 23] = true;
												GameCanvas.keyPressed[(!Main.isPC) ? 4 : 23] = true;
											}
											else
											{
												bool flag14 = this.angle >= 120 && this.angle <= 200;
												if (flag14)
												{
													GameCanvas.keyHold[(!Main.isPC) ? 4 : 23] = true;
													GameCanvas.keyPressed[(!Main.isPC) ? 4 : 23] = true;
												}
												else
												{
													bool flag15 = this.angle > 200 && this.angle < 250;
													if (flag15)
													{
														GameCanvas.keyHold[(!Main.isPC) ? 2 : 21] = true;
														GameCanvas.keyPressed[(!Main.isPC) ? 2 : 21] = true;
														GameCanvas.keyHold[(!Main.isPC) ? 4 : 23] = true;
														GameCanvas.keyPressed[(!Main.isPC) ? 4 : 23] = true;
													}
													else
													{
														bool flag16 = this.angle >= 250 && this.angle <= 290;
														if (flag16)
														{
															GameCanvas.keyHold[(!Main.isPC) ? 2 : 21] = true;
															GameCanvas.keyPressed[(!Main.isPC) ? 2 : 21] = true;
														}
														else
														{
															bool flag17 = this.angle > 290 && this.angle < 340;
															if (flag17)
															{
																GameCanvas.keyHold[(!Main.isPC) ? 2 : 21] = true;
																GameCanvas.keyPressed[(!Main.isPC) ? 2 : 21] = true;
																GameCanvas.keyHold[(!Main.isPC) ? 6 : 24] = true;
																GameCanvas.keyPressed[(!Main.isPC) ? 6 : 24] = true;
															}
														}
													}
												}
											}
										}
									}
								}
							}
							else
							{
								this.resetHold();
							}
						}
					}
				}
				else
				{
					this.xM = (this.xC = 45);
					bool flag18 = !this.isLargeGamePad;
					if (flag18)
					{
						this.yM = (this.yC = GameCanvas.h - 90);
					}
					else
					{
						this.yM = (this.yC = GameCanvas.h - 45);
					}
					this.isGamePad = false;
					this.resetHold();
				}
			}
		}
		catch (Exception)
		{
		}
	}

	// Token: 0x0600044F RID: 1103 RVA: 0x00052758 File Offset: 0x00050958
	private bool checkPointerMove(int distance)
	{
		bool flag = GameScr.isAnalog == 0;
		bool flag2;
		if (flag)
		{
			flag2 = false;
		}
		else
		{
			bool flag3 = global::Char.myCharz().statusMe == 3;
			if (flag3)
			{
				flag2 = true;
			}
			else
			{
				try
				{
					for (int i = 2; i > 0; i--)
					{
						int num = GameCanvas.arrPos[i].x - GameCanvas.arrPos[i - 1].x;
						int num2 = GameCanvas.arrPos[i].y - GameCanvas.arrPos[i - 1].y;
						bool flag4 = Res.abs(num) > distance && Res.abs(num2) > distance;
						if (flag4)
						{
							return false;
						}
					}
				}
				catch (Exception)
				{
				}
				flag2 = true;
			}
		}
		return flag2;
	}

	// Token: 0x06000450 RID: 1104 RVA: 0x00004C77 File Offset: 0x00002E77
	private void resetHold()
	{
		GameCanvas.clearKeyHold();
	}

	// Token: 0x06000451 RID: 1105 RVA: 0x00052820 File Offset: 0x00050A20
	public void paint(mGraphics g)
	{
		bool flag = GameScr.isAnalog != 0;
		if (flag)
		{
			this.xZone = 0;
			this.yZone = (GameCanvas.h >> 1) + 40;
			this.wZone = GameCanvas.hw / 4 * 3 - 40;
			this.hZone = GameCanvas.h;
			g.drawImage(GameScr.imgAnalog1, this.xC, this.yC, mGraphics.HCENTER | mGraphics.VCENTER);
			g.drawImage(GameScr.imgAnalog2, this.xM, this.yM, mGraphics.HCENTER | mGraphics.VCENTER);
		}
	}

	// Token: 0x06000452 RID: 1106 RVA: 0x000528B8 File Offset: 0x00050AB8
	public bool disableCheckDrag()
	{
		bool flag = GameScr.isAnalog == 0;
		return !flag && this.isGamePad;
	}

	// Token: 0x06000453 RID: 1107 RVA: 0x000528E4 File Offset: 0x00050AE4
	public bool disableClickMove()
	{
		bool flag2;
		try
		{
			bool flag = GameScr.isAnalog == 0;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				flag2 = (GameCanvas.px >= this.xZone && GameCanvas.px <= this.xZone + this.wZone && GameCanvas.py >= this.yZone && GameCanvas.py <= this.yZone + this.hZone) || (GameCanvas.px >= this.xZone && GameCanvas.px <= GameCanvas.w && GameCanvas.py >= this.yZone && GameCanvas.py <= this.yZone + this.hZone) || GameCanvas.px >= GameCanvas.w - 50;
			}
		}
		catch (Exception)
		{
			flag2 = false;
		}
		return flag2;
	}

	// Token: 0x0400075E RID: 1886
	private int xC;

	// Token: 0x0400075F RID: 1887
	private int yC;

	// Token: 0x04000760 RID: 1888
	private int xM;

	// Token: 0x04000761 RID: 1889
	private int yM;

	// Token: 0x04000762 RID: 1890
	private int xMLast;

	// Token: 0x04000763 RID: 1891
	private int yMLast;

	// Token: 0x04000764 RID: 1892
	private int R;

	// Token: 0x04000765 RID: 1893
	private int r;

	// Token: 0x04000766 RID: 1894
	private int d;

	// Token: 0x04000767 RID: 1895
	private int xTemp;

	// Token: 0x04000768 RID: 1896
	private int yTemp;

	// Token: 0x04000769 RID: 1897
	private int deltaX;

	// Token: 0x0400076A RID: 1898
	private int deltaY;

	// Token: 0x0400076B RID: 1899
	private int delta;

	// Token: 0x0400076C RID: 1900
	private int angle;

	// Token: 0x0400076D RID: 1901
	public int xZone;

	// Token: 0x0400076E RID: 1902
	public int yZone;

	// Token: 0x0400076F RID: 1903
	public int wZone;

	// Token: 0x04000770 RID: 1904
	public int hZone;

	// Token: 0x04000771 RID: 1905
	private bool isGamePad;

	// Token: 0x04000772 RID: 1906
	public bool isSmallGamePad;

	// Token: 0x04000773 RID: 1907
	public bool isMediumGamePad;

	// Token: 0x04000774 RID: 1908
	public bool isLargeGamePad;
}
using System;
using System.Threading;
using AssemblyCSharp.Mod.Xmap;
using Assets.src.g;

// Token: 0x0200005E RID: 94
public class GameScr : mScreen, IChatable
{
	// Token: 0x06000454 RID: 1108 RVA: 0x000529B4 File Offset: 0x00050BB4
	public GameScr()
	{
		bool flag = GameCanvas.w == 128 || GameCanvas.h <= 208;
		if (flag)
		{
			GameScr.indexSize = 20;
		}
		this.cmdback = new Command(string.Empty, 11021);
		this.cmdMenu = new Command("menu", 11000);
		this.cmdFocus = new Command(string.Empty, 11001);
		this.cmdMenu.img = GameScr.imgMenu;
		this.cmdMenu.w = mGraphics.getImageWidth(this.cmdMenu.img) + 20;
		this.cmdMenu.isPlaySoundButton = false;
		this.cmdFocus.img = GameScr.imgFocus;
		bool isTouch = GameCanvas.isTouch;
		if (isTouch)
		{
			this.cmdMenu.x = 0;
			this.cmdMenu.y = 50;
			this.cmdFocus = null;
		}
		else
		{
			this.cmdMenu.x = 0;
			this.cmdMenu.y = GameScr.gH - 30;
			this.cmdFocus.x = GameScr.gW - 32;
			this.cmdFocus.y = GameScr.gH - 32;
		}
		this.right = this.cmdFocus;
		GameScr.isPaintRada = 1;
		bool isTouch2 = GameCanvas.isTouch;
		if (isTouch2)
		{
			GameScr.isHaveSelectSkill = true;
		}
		this.cmdDoiCo = new Command("Đổi cờ", GameCanvas.gI(), 100001, null);
		this.cmdLogOut = new Command("Logout", GameCanvas.gI(), 100002, null);
		this.cmdChatTheGioi = new Command("chat world", GameCanvas.gI(), 100003, null);
		this.cmdshowInfo = new Command("InfoLog", GameCanvas.gI(), 100004, null);
		this.cmdDoiCo.setType();
		this.cmdLogOut.setType();
		this.cmdChatTheGioi.setType();
		this.cmdshowInfo.setType();
		this.cmdChatTheGioi.x = GameCanvas.w - this.cmdChatTheGioi.w;
		this.cmdshowInfo.x = GameCanvas.w - this.cmdshowInfo.w;
		this.cmdLogOut.x = GameCanvas.w - this.cmdLogOut.w;
		this.cmdDoiCo.x = GameCanvas.w - this.cmdDoiCo.w;
		this.cmdChatTheGioi.y = this.cmdChatTheGioi.h + mFont.tahoma_7_white.getHeight();
		this.cmdshowInfo.y = this.cmdChatTheGioi.h * 2 + mFont.tahoma_7_white.getHeight();
		this.cmdLogOut.y = this.cmdChatTheGioi.h * 3 + mFont.tahoma_7_white.getHeight();
		this.cmdDoiCo.y = this.cmdChatTheGioi.h * 4 + mFont.tahoma_7_white.getHeight();
	}

	// Token: 0x06000455 RID: 1109 RVA: 0x00052D2C File Offset: 0x00050F2C
	public static void loadBg()
	{
		GameScr.fra_PVE_Bar_0 = new FrameImage(mSystem.loadImage("/mainImage/i_pve_bar_0.png"), 6, 15);
		GameScr.fra_PVE_Bar_1 = new FrameImage(mSystem.loadImage("/mainImage/i_pve_bar_1.png"), 38, 21);
		GameScr.imgVS = mSystem.loadImage("/mainImage/i_vs.png");
		GameScr.imgHP_NEW = mSystem.loadImage("/mainImage/i_hp.png");
		GameScr.imgKhung = mSystem.loadImage("/mainImage/i_khung.png");
		GameScr.imgMenu = GameCanvas.loadImage("/mainImage/myTexture2dmenu.png");
		GameScr.imgFocus = GameCanvas.loadImage("/mainImage/myTexture2dfocus.png");
		GameScr.imgHP_tm_do = GameCanvas.loadImage("/mainImage/tm-do.png");
		GameScr.imgHP_tm_vang = GameCanvas.loadImage("/mainImage/tm-vang.png");
		GameScr.imgHP_tm_xam = GameCanvas.loadImage("/mainImage/tm-xam.png");
		GameScr.imgHP_tm_xanh = GameCanvas.loadImage("/mainImage/tm-xanh.png");
		GameScr.imgNR1 = GameCanvas.loadImage("/mainImage/myTexture2dPea_0.png");
		GameScr.imgNR2 = GameCanvas.loadImage("/mainImage/myTexture2dPea_1.png");
		GameScr.imgNR3 = GameCanvas.loadImage("/mainImage/myTexture2dPea_2.png");
		GameScr.imgNR4 = GameCanvas.loadImage("/mainImage/myTexture2dPea_3.png");
		GameScr.flyTextX = new int[5];
		GameScr.flyTextY = new int[5];
		GameScr.flyTextDx = new int[5];
		GameScr.flyTextDy = new int[5];
		GameScr.flyTextState = new int[5];
		GameScr.flyTextString = new string[5];
		GameScr.flyTextYTo = new int[5];
		GameScr.flyTime = new int[5];
		GameScr.flyTextColor = new int[8];
		for (int i = 0; i < 5; i++)
		{
			GameScr.flyTextState[i] = -1;
		}
		sbyte[] array = Rms.loadRMS("NRdataVersion");
		sbyte[] array2 = Rms.loadRMS("NRmapVersion");
		sbyte[] array3 = Rms.loadRMS("NRskillVersion");
		sbyte[] array4 = Rms.loadRMS("NRitemVersion");
		bool flag = array != null;
		if (flag)
		{
			GameScr.vcData = array[0];
		}
		bool flag2 = array2 != null;
		if (flag2)
		{
			GameScr.vcMap = array2[0];
		}
		bool flag3 = array3 != null;
		if (flag3)
		{
			GameScr.vcSkill = array3[0];
		}
		bool flag4 = array4 != null;
		if (flag4)
		{
			GameScr.vcItem = array4[0];
		}
		GameScr.imgNut = GameCanvas.loadImage("/mainImage/myTexture2dnut.png");
		GameScr.imgNutF = GameCanvas.loadImage("/mainImage/myTexture2dnutF.png");
		MobCapcha.init();
		GameScr.isAnalog = ((Rms.loadRMSInt("analog") == 1) ? 1 : 0);
		GameScr.gamePad = new GamePad();
		GameScr.arrow = GameCanvas.loadImage("/mainImage/myTexture2darrow3.png");
		GameScr.imgTrans = GameCanvas.loadImage("/bg/trans.png");
		GameScr.imgRoomStat = GameCanvas.loadImage("/mainImage/myTexture2dstat.png");
		GameScr.frBarPow0 = GameCanvas.loadImage("/mainImage/myTexture2dlineColor20.png");
		GameScr.frBarPow1 = GameCanvas.loadImage("/mainImage/myTexture2dlineColor21.png");
		GameScr.frBarPow2 = GameCanvas.loadImage("/mainImage/myTexture2dlineColor22.png");
		GameScr.frBarPow20 = GameCanvas.loadImage("/mainImage/myTexture2dlineColor00.png");
		GameScr.frBarPow21 = GameCanvas.loadImage("/mainImage/myTexture2dlineColor01.png");
		GameScr.frBarPow22 = GameCanvas.loadImage("/mainImage/myTexture2dlineColor02.png");
	}

	// Token: 0x06000456 RID: 1110 RVA: 0x00004C80 File Offset: 0x00002E80
	public void initSelectChar()
	{
		this.readPart();
		SmallImage.init();
	}

	// Token: 0x06000457 RID: 1111 RVA: 0x00052FF8 File Offset: 0x000511F8
	public static void paintOngMauPercent(Image img0, Image img1, Image img2, float x, float y, int size, float pixelPercent, mGraphics g)
	{
		int clipX = g.getClipX();
		int clipY = g.getClipY();
		int clipWidth = g.getClipWidth();
		int clipHeight = g.getClipHeight();
		g.setClip((int)x, (int)y, (int)pixelPercent, 13);
		int num = size / 15 - 2;
		for (int i = 0; i < num; i++)
		{
			g.drawImage(img1, x + (float)((i + 1) * 15), y, 0);
		}
		g.drawImage(img0, x, y, 0);
		g.drawImage(img1, x + (float)size - 30f, y, 0);
		g.drawImage(img2, x + (float)size - 15f, y, 0);
		g.setClip(clipX, clipY, clipWidth, clipHeight);
	}

	// Token: 0x06000458 RID: 1112 RVA: 0x000530B8 File Offset: 0x000512B8
	public void initTraining()
	{
		bool isCreateChar = CreateCharScr.isCreateChar;
		if (isCreateChar)
		{
			CreateCharScr.isCreateChar = false;
			this.right = null;
		}
	}

	// Token: 0x06000459 RID: 1113 RVA: 0x000530E0 File Offset: 0x000512E0
	public bool isMapDocNhan()
	{
		return TileMap.mapID >= 53 && TileMap.mapID <= 62;
	}

	// Token: 0x0600045A RID: 1114 RVA: 0x00053114 File Offset: 0x00051314
	public bool isMapFize()
	{
		return TileMap.mapID >= 63;
	}

	// Token: 0x0600045B RID: 1115 RVA: 0x0005313C File Offset: 0x0005133C
	public override void switchToMe()
	{
		GameScr.vChatVip.removeAllElements();
		ServerListScreen.isWait = false;
		bool flag = BackgroudEffect.isHaveRain();
		if (flag)
		{
			SoundMn.gI().rain();
		}
		LoginScr.isContinueToLogin = false;
		global::Char.isLoadingMap = false;
		bool flag2 = !GameScr.isPaintOther;
		if (flag2)
		{
			Service.gI().finishLoadMap();
		}
		bool flag3 = TileMap.isTrainingMap();
		if (flag3)
		{
			this.initTraining();
		}
		GameScr.info1.isUpdate = true;
		GameScr.info2.isUpdate = true;
		this.resetButton();
		GameScr.isLoadAllData = true;
		GameScr.isPaintOther = false;
		base.switchToMe();
	}

	// Token: 0x0600045C RID: 1116 RVA: 0x000531D8 File Offset: 0x000513D8
	public static int getMaxExp(int level)
	{
		int num = 0;
		for (int i = 0; i <= level; i++)
		{
			num += (int)GameScr.exps[i];
		}
		return num;
	}

	// Token: 0x0600045D RID: 1117 RVA: 0x00053210 File Offset: 0x00051410
	public static void resetAllvector()
	{
		GameScr.vCharInMap.removeAllElements();
		Teleport.vTeleport.removeAllElements();
		GameScr.vItemMap.removeAllElements();
		Effect2.vEffect2.removeAllElements();
		Effect2.vAnimateEffect.removeAllElements();
		Effect2.vEffect2Outside.removeAllElements();
		Effect2.vEffectFeet.removeAllElements();
		Effect2.vEffect3.removeAllElements();
		GameScr.vMobAttack.removeAllElements();
		GameScr.vMob.removeAllElements();
		GameScr.vNpc.removeAllElements();
		global::Char.myCharz().vMovePoints.removeAllElements();
	}

	// Token: 0x0600045E RID: 1118 RVA: 0x00003E4C File Offset: 0x0000204C
	public void loadSkillShortcut()
	{
	}

	// Token: 0x0600045F RID: 1119 RVA: 0x000532A8 File Offset: 0x000514A8
	public void onOSkill(sbyte[] oSkillID)
	{
		Cout.println("GET onScreenSkill!");
		GameScr.onScreenSkill = new Skill[10];
		bool flag = oSkillID == null;
		if (flag)
		{
			this.loadDefaultonScreenSkill();
		}
		else
		{
			for (int i = 0; i < oSkillID.Length; i++)
			{
				for (int j = 0; j < global::Char.myCharz().vSkillFight.size(); j++)
				{
					Skill skill = (Skill)global::Char.myCharz().vSkillFight.elementAt(j);
					bool flag2 = skill.template.id == oSkillID[i];
					if (flag2)
					{
						GameScr.onScreenSkill[i] = skill;
						break;
					}
				}
			}
		}
	}

	// Token: 0x06000460 RID: 1120 RVA: 0x00053350 File Offset: 0x00051550
	public void onKSkill(sbyte[] kSkillID)
	{
		Cout.println("GET KEYSKILL!");
		GameScr.keySkill = new Skill[10];
		bool flag = kSkillID == null;
		if (flag)
		{
			this.loadDefaultKeySkill();
		}
		else
		{
			for (int i = 0; i < kSkillID.Length; i++)
			{
				for (int j = 0; j < global::Char.myCharz().vSkillFight.size(); j++)
				{
					Skill skill = (Skill)global::Char.myCharz().vSkillFight.elementAt(j);
					bool flag2 = skill.template.id == kSkillID[i];
					if (flag2)
					{
						GameScr.keySkill[i] = skill;
						break;
					}
				}
			}
		}
	}

	// Token: 0x06000461 RID: 1121 RVA: 0x000533F8 File Offset: 0x000515F8
	public void onCSkill(sbyte[] cSkillID)
	{
		Cout.println("GET CURRENTSKILL!");
		bool flag = cSkillID == null || cSkillID.Length == 0;
		if (flag)
		{
			bool flag2 = global::Char.myCharz().vSkillFight.size() > 0;
			if (flag2)
			{
				global::Char.myCharz().myskill = (Skill)global::Char.myCharz().vSkillFight.elementAt(0);
			}
		}
		else
		{
			for (int i = 0; i < global::Char.myCharz().vSkillFight.size(); i++)
			{
				Skill skill = (Skill)global::Char.myCharz().vSkillFight.elementAt(i);
				bool flag3 = skill.template.id == cSkillID[0];
				if (flag3)
				{
					global::Char.myCharz().myskill = skill;
					break;
				}
			}
		}
		bool flag4 = global::Char.myCharz().myskill != null;
		if (flag4)
		{
			Service.gI().selectSkill((int)global::Char.myCharz().myskill.template.id);
			this.saveRMSCurrentSkill(global::Char.myCharz().myskill.template.id);
		}
	}

	// Token: 0x06000462 RID: 1122 RVA: 0x00053508 File Offset: 0x00051708
	private void loadDefaultonScreenSkill()
	{
		Cout.println("LOAD DEFAULT ONmScreen SKILL");
		int num = 0;
		while (num < GameScr.onScreenSkill.Length && num < global::Char.myCharz().vSkillFight.size())
		{
			Skill skill = (Skill)global::Char.myCharz().vSkillFight.elementAt(num);
			GameScr.onScreenSkill[num] = skill;
			num++;
		}
		this.saveonScreenSkillToRMS();
	}

	// Token: 0x06000463 RID: 1123 RVA: 0x00053574 File Offset: 0x00051774
	private void loadDefaultKeySkill()
	{
		Cout.println("LOAD DEFAULT KEY SKILL");
		int num = 0;
		while (num < GameScr.keySkill.Length && num < global::Char.myCharz().vSkillFight.size())
		{
			Skill skill = (Skill)global::Char.myCharz().vSkillFight.elementAt(num);
			GameScr.keySkill[num] = skill;
			num++;
		}
		this.saveKeySkillToRMS();
	}

	// Token: 0x06000464 RID: 1124 RVA: 0x000535E0 File Offset: 0x000517E0
	public void doSetOnScreenSkill(SkillTemplate skillTemplate)
	{
		Skill skill = global::Char.myCharz().getSkill(skillTemplate);
		MyVector myVector = new MyVector();
		for (int i = 0; i < 10; i++)
		{
			object obj = new object[]
			{
				skill,
				i.ToString() + string.Empty
			};
			Command command = new Command(mResources.into_place + (i + 1).ToString(), 11120, obj);
			Skill skill2 = GameScr.onScreenSkill[i];
			bool flag = skill2 != null;
			if (flag)
			{
				command.isDisplay = true;
			}
			myVector.addElement(command);
		}
		GameCanvas.menu.startAt(myVector, 0);
	}

	// Token: 0x06000465 RID: 1125 RVA: 0x0005368C File Offset: 0x0005188C
	public void doSetKeySkill(SkillTemplate skillTemplate)
	{
		Cout.println("DO SET KEY SKILL");
		Skill skill = global::Char.myCharz().getSkill(skillTemplate);
		string[] array = ((!TField.isQwerty) ? mResources.key_skill : mResources.key_skill_qwerty);
		MyVector myVector = new MyVector();
		for (int i = 0; i < 10; i++)
		{
			MyVector myVector2 = myVector;
			object obj = new object[]
			{
				skill,
				i.ToString() + string.Empty
			};
			myVector2.addElement(new Command(array[i], 11121, obj));
		}
		GameCanvas.menu.startAt(myVector, 0);
	}

	// Token: 0x06000466 RID: 1126 RVA: 0x00053724 File Offset: 0x00051924
	public void saveonScreenSkillToRMS()
	{
		sbyte[] array = new sbyte[GameScr.onScreenSkill.Length];
		for (int i = 0; i < GameScr.onScreenSkill.Length; i++)
		{
			bool flag = GameScr.onScreenSkill[i] == null;
			if (flag)
			{
				array[i] = -1;
			}
			else
			{
				array[i] = GameScr.onScreenSkill[i].template.id;
			}
		}
		Service.gI().changeOnKeyScr(array);
	}

	// Token: 0x06000467 RID: 1127 RVA: 0x00053790 File Offset: 0x00051990
	public void saveKeySkillToRMS()
	{
		sbyte[] array = new sbyte[GameScr.keySkill.Length];
		for (int i = 0; i < GameScr.keySkill.Length; i++)
		{
			bool flag = GameScr.keySkill[i] == null;
			if (flag)
			{
				array[i] = -1;
			}
			else
			{
				array[i] = GameScr.keySkill[i].template.id;
			}
		}
		Service.gI().changeOnKeyScr(array);
	}

	// Token: 0x06000468 RID: 1128 RVA: 0x00003E4C File Offset: 0x0000204C
	public void saveRMSCurrentSkill(sbyte id)
	{
	}

	// Token: 0x06000469 RID: 1129 RVA: 0x000537FC File Offset: 0x000519FC
	public void addSkillShortcut(Skill skill)
	{
		Cout.println("ADD SKILL SHORTCUT TO SKILL " + skill.template.id.ToString());
		for (int i = 0; i < GameScr.onScreenSkill.Length; i++)
		{
			bool flag = GameScr.onScreenSkill[i] == null;
			if (flag)
			{
				GameScr.onScreenSkill[i] = skill;
				break;
			}
		}
		for (int j = 0; j < GameScr.keySkill.Length; j++)
		{
			bool flag2 = GameScr.keySkill[j] == null;
			if (flag2)
			{
				GameScr.keySkill[j] = skill;
				break;
			}
		}
		bool flag3 = global::Char.myCharz().myskill == null;
		if (flag3)
		{
			global::Char.myCharz().myskill = skill;
		}
		this.saveKeySkillToRMS();
		this.saveonScreenSkillToRMS();
	}

	// Token: 0x0600046A RID: 1130 RVA: 0x0000EC9C File Offset: 0x0000CE9C
	public bool isBagFull()
	{
		for (int i = global::Char.myCharz().arrItemBag.Length - 1; i >= 0; i--)
		{
			bool flag = global::Char.myCharz().arrItemBag[i] == null;
			if (flag)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600046B RID: 1131 RVA: 0x00004C90 File Offset: 0x00002E90
	public void createConfirm(string[] menu, Npc npc)
	{
		this.resetButton();
		this.isLockKey = true;
		this.left = new Command(menu[0], 130011, npc);
		this.right = new Command(menu[1], 130012, npc);
	}

	// Token: 0x0600046C RID: 1132 RVA: 0x000538C0 File Offset: 0x00051AC0
	public void createMenu(string[] menu, Npc npc)
	{
		MyVector myVector = new MyVector();
		for (int i = 0; i < menu.Length; i++)
		{
			myVector.addElement(new Command(menu[i], 11057, npc));
		}
		GameCanvas.menu.startAt(myVector, 2);
	}

	// Token: 0x0600046D RID: 1133 RVA: 0x0005390C File Offset: 0x00051B0C
	public void readPart()
	{
		DataInputStream dataInputStream = null;
		try
		{
			dataInputStream = new DataInputStream(Rms.loadRMS("NR_part"));
			int num = (int)dataInputStream.readShort();
			GameScr.parts = new Part[num];
			for (int i = 0; i < num; i++)
			{
				int num2 = (int)dataInputStream.readByte();
				GameScr.parts[i] = new Part(num2);
				for (int j = 0; j < GameScr.parts[i].pi.Length; j++)
				{
					GameScr.parts[i].pi[j] = new PartImage();
					GameScr.parts[i].pi[j].id = dataInputStream.readShort();
					GameScr.parts[i].pi[j].dx = dataInputStream.readByte();
					GameScr.parts[i].pi[j].dy = dataInputStream.readByte();
				}
			}
		}
		catch (Exception ex)
		{
			Cout.LogError("LOI TAI readPart " + ex.ToString());
		}
		finally
		{
			try
			{
				dataInputStream.close();
			}
			catch (Exception ex2)
			{
				Res.outz2("LOI TAI readPart 2" + ex2.StackTrace);
			}
		}
	}

	// Token: 0x0600046E RID: 1134 RVA: 0x00053A6C File Offset: 0x00051C6C
	public void readEfect()
	{
		DataInputStream dataInputStream = null;
		try
		{
			dataInputStream = new DataInputStream(Rms.loadRMS("NR_effect"));
			int num = (int)dataInputStream.readShort();
			GameScr.efs = new EffectCharPaint[num];
			for (int i = 0; i < num; i++)
			{
				GameScr.efs[i] = new EffectCharPaint();
				GameScr.efs[i].idEf = (int)dataInputStream.readShort();
				GameScr.efs[i].arrEfInfo = new EffectInfoPaint[(int)dataInputStream.readByte()];
				for (int j = 0; j < GameScr.efs[i].arrEfInfo.Length; j++)
				{
					GameScr.efs[i].arrEfInfo[j] = new EffectInfoPaint();
					GameScr.efs[i].arrEfInfo[j].idImg = (int)dataInputStream.readShort();
					GameScr.efs[i].arrEfInfo[j].dx = (int)dataInputStream.readByte();
					GameScr.efs[i].arrEfInfo[j].dy = (int)dataInputStream.readByte();
				}
			}
		}
		catch (Exception)
		{
		}
		finally
		{
			try
			{
				dataInputStream.close();
			}
			catch (Exception ex)
			{
				Cout.LogError("Loi ham Eff: " + ex.ToString());
			}
		}
	}

	// Token: 0x0600046F RID: 1135 RVA: 0x00053BCC File Offset: 0x00051DCC
	public void readArrow()
	{
		DataInputStream dataInputStream = null;
		try
		{
			dataInputStream = new DataInputStream(Rms.loadRMS("NR_arrow"));
			int num = (int)dataInputStream.readShort();
			GameScr.arrs = new Arrowpaint[num];
			for (int i = 0; i < num; i++)
			{
				GameScr.arrs[i] = new Arrowpaint();
				GameScr.arrs[i].id = (int)dataInputStream.readShort();
				GameScr.arrs[i].imgId[0] = (int)dataInputStream.readShort();
				GameScr.arrs[i].imgId[1] = (int)dataInputStream.readShort();
				GameScr.arrs[i].imgId[2] = (int)dataInputStream.readShort();
			}
		}
		catch (Exception)
		{
		}
		finally
		{
			try
			{
				dataInputStream.close();
			}
			catch (Exception ex)
			{
				Cout.LogError("Loi ham readArrow: " + ex.ToString());
			}
		}
	}

	// Token: 0x06000470 RID: 1136 RVA: 0x00053CC8 File Offset: 0x00051EC8
	public void readDart()
	{
		DataInputStream dataInputStream = null;
		try
		{
			dataInputStream = new DataInputStream(Rms.loadRMS("NR_dart"));
			int num = (int)dataInputStream.readShort();
			GameScr.darts = new DartInfo[num];
			for (int i = 0; i < num; i++)
			{
				GameScr.darts[i] = new DartInfo();
				GameScr.darts[i].id = dataInputStream.readShort();
				GameScr.darts[i].nUpdate = dataInputStream.readShort();
				GameScr.darts[i].va = (int)(dataInputStream.readShort() * 256);
				GameScr.darts[i].xdPercent = dataInputStream.readShort();
				int num2 = (int)dataInputStream.readShort();
				GameScr.darts[i].tail = new short[num2];
				for (int j = 0; j < num2; j++)
				{
					GameScr.darts[i].tail[j] = dataInputStream.readShort();
				}
				num2 = (int)dataInputStream.readShort();
				GameScr.darts[i].tailBorder = new short[num2];
				for (int k = 0; k < num2; k++)
				{
					GameScr.darts[i].tailBorder[k] = dataInputStream.readShort();
				}
				num2 = (int)dataInputStream.readShort();
				GameScr.darts[i].xd1 = new short[num2];
				for (int l = 0; l < num2; l++)
				{
					GameScr.darts[i].xd1[l] = dataInputStream.readShort();
				}
				num2 = (int)dataInputStream.readShort();
				GameScr.darts[i].xd2 = new short[num2];
				for (int m = 0; m < num2; m++)
				{
					GameScr.darts[i].xd2[m] = dataInputStream.readShort();
				}
				num2 = (int)dataInputStream.readShort();
				GameScr.darts[i].head = new short[num2][];
				for (int n = 0; n < num2; n++)
				{
					short num3 = dataInputStream.readShort();
					GameScr.darts[i].head[n] = new short[(int)num3];
					for (int num4 = 0; num4 < (int)num3; num4++)
					{
						GameScr.darts[i].head[n][num4] = dataInputStream.readShort();
					}
				}
				num2 = (int)dataInputStream.readShort();
				GameScr.darts[i].headBorder = new short[num2][];
				for (int num5 = 0; num5 < num2; num5++)
				{
					short num6 = dataInputStream.readShort();
					GameScr.darts[i].headBorder[num5] = new short[(int)num6];
					for (int num7 = 0; num7 < (int)num6; num7++)
					{
						GameScr.darts[i].headBorder[num5][num7] = dataInputStream.readShort();
					}
				}
			}
		}
		catch (Exception ex)
		{
			Cout.LogError("Loi ham ReadDart: " + ex.ToString());
		}
		finally
		{
			try
			{
				dataInputStream.close();
			}
			catch (Exception ex2)
			{
				Cout.LogError("Loi ham reaaDart: " + ex2.ToString());
			}
		}
	}

	// Token: 0x06000471 RID: 1137 RVA: 0x00054020 File Offset: 0x00052220
	public void readSkill()
	{
		DataInputStream dataInputStream = null;
		try
		{
			dataInputStream = new DataInputStream(Rms.loadRMS("NR_skill"));
			int num = (int)dataInputStream.readShort();
			int num2 = Skills.skills.size();
			GameScr.sks = new SkillPaint[num2];
			for (int i = 0; i < num; i++)
			{
				short num3 = dataInputStream.readShort();
				bool flag = num3 == 1111;
				if (flag)
				{
					num3 = (short)(num - 1);
				}
				GameScr.sks[(int)num3] = new SkillPaint();
				GameScr.sks[(int)num3].id = (int)num3;
				GameScr.sks[(int)num3].effectHappenOnMob = (int)dataInputStream.readShort();
				bool flag2 = GameScr.sks[(int)num3].effectHappenOnMob <= 0;
				if (flag2)
				{
					GameScr.sks[(int)num3].effectHappenOnMob = 80;
				}
				GameScr.sks[(int)num3].numEff = (int)dataInputStream.readByte();
				GameScr.sks[(int)num3].skillStand = new SkillInfoPaint[(int)dataInputStream.readByte()];
				for (int j = 0; j < GameScr.sks[(int)num3].skillStand.Length; j++)
				{
					GameScr.sks[(int)num3].skillStand[j] = new SkillInfoPaint();
					GameScr.sks[(int)num3].skillStand[j].status = (int)dataInputStream.readByte();
					GameScr.sks[(int)num3].skillStand[j].effS0Id = (int)dataInputStream.readShort();
					GameScr.sks[(int)num3].skillStand[j].e0dx = (int)dataInputStream.readShort();
					GameScr.sks[(int)num3].skillStand[j].e0dy = (int)dataInputStream.readShort();
					GameScr.sks[(int)num3].skillStand[j].effS1Id = (int)dataInputStream.readShort();
					GameScr.sks[(int)num3].skillStand[j].e1dx = (int)dataInputStream.readShort();
					GameScr.sks[(int)num3].skillStand[j].e1dy = (int)dataInputStream.readShort();
					GameScr.sks[(int)num3].skillStand[j].effS2Id = (int)dataInputStream.readShort();
					GameScr.sks[(int)num3].skillStand[j].e2dx = (int)dataInputStream.readShort();
					GameScr.sks[(int)num3].skillStand[j].e2dy = (int)dataInputStream.readShort();
					GameScr.sks[(int)num3].skillStand[j].arrowId = (int)dataInputStream.readShort();
					GameScr.sks[(int)num3].skillStand[j].adx = (int)dataInputStream.readShort();
					GameScr.sks[(int)num3].skillStand[j].ady = (int)dataInputStream.readShort();
				}
				GameScr.sks[(int)num3].skillfly = new SkillInfoPaint[(int)dataInputStream.readByte()];
				for (int k = 0; k < GameScr.sks[(int)num3].skillfly.Length; k++)
				{
					GameScr.sks[(int)num3].skillfly[k] = new SkillInfoPaint();
					GameScr.sks[(int)num3].skillfly[k].status = (int)dataInputStream.readByte();
					GameScr.sks[(int)num3].skillfly[k].effS0Id = (int)dataInputStream.readShort();
					GameScr.sks[(int)num3].skillfly[k].e0dx = (int)dataInputStream.readShort();
					GameScr.sks[(int)num3].skillfly[k].e0dy = (int)dataInputStream.readShort();
					GameScr.sks[(int)num3].skillfly[k].effS1Id = (int)dataInputStream.readShort();
					GameScr.sks[(int)num3].skillfly[k].e1dx = (int)dataInputStream.readShort();
					GameScr.sks[(int)num3].skillfly[k].e1dy = (int)dataInputStream.readShort();
					GameScr.sks[(int)num3].skillfly[k].effS2Id = (int)dataInputStream.readShort();
					GameScr.sks[(int)num3].skillfly[k].e2dx = (int)dataInputStream.readShort();
					GameScr.sks[(int)num3].skillfly[k].e2dy = (int)dataInputStream.readShort();
					GameScr.sks[(int)num3].skillfly[k].arrowId = (int)dataInputStream.readShort();
					GameScr.sks[(int)num3].skillfly[k].adx = (int)dataInputStream.readShort();
					GameScr.sks[(int)num3].skillfly[k].ady = (int)dataInputStream.readShort();
				}
			}
		}
		catch (Exception ex)
		{
			Cout.LogError("Loi ham readSkill: " + ex.ToString());
		}
		finally
		{
			try
			{
				dataInputStream.close();
			}
			catch (Exception ex2)
			{
				Cout.LogError("Loi ham readskill: " + ex2.ToString());
			}
		}
	}

	// Token: 0x06000472 RID: 1138 RVA: 0x0005450C File Offset: 0x0005270C
	public void readOk()
	{
		try
		{
			Res.outz("<readOk><vsData<" + GameScr.vsData.ToString() + "==" + GameScr.vcData.ToString());
			Res.outz("<readOk><vsMap<" + GameScr.vsMap.ToString() + "==" + GameScr.vcMap.ToString());
			Res.outz("<readOk><vsSkill<" + GameScr.vsSkill.ToString() + "==" + GameScr.vcSkill.ToString());
			Res.outz("<readOk><vsItem<" + GameScr.vsItem.ToString() + "==" + GameScr.vcItem.ToString());
			bool flag = GameScr.vsData == GameScr.vcData && GameScr.vsMap == GameScr.vcMap && GameScr.vsSkill == GameScr.vcSkill && GameScr.vsItem == GameScr.vcItem;
			if (flag)
			{
				Res.outz(string.Concat(new string[]
				{
					GameScr.vsData.ToString(),
					",",
					GameScr.vsMap.ToString(),
					",",
					GameScr.vsSkill.ToString(),
					",",
					GameScr.vsItem.ToString()
				}));
				GameScr.gI().readDart();
				GameScr.gI().readEfect();
				GameScr.gI().readArrow();
				GameScr.gI().readSkill();
				Service.gI().clientOk();
			}
		}
		catch (Exception ex)
		{
			Cout.LogError("Loi ham readOk: " + ex.ToString());
		}
	}

	// Token: 0x06000473 RID: 1139 RVA: 0x000546CC File Offset: 0x000528CC
	public static GameScr gI()
	{
		bool flag = GameScr.instance == null;
		if (flag)
		{
			GameScr.instance = new GameScr();
		}
		return GameScr.instance;
	}

	// Token: 0x06000474 RID: 1140 RVA: 0x00004CC9 File Offset: 0x00002EC9
	public static void clearGameScr()
	{
		GameScr.instance = null;
	}

	// Token: 0x06000475 RID: 1141 RVA: 0x00004CD2 File Offset: 0x00002ED2
	public void loadGameScr()
	{
		GameScr.loadSplash();
		Res.init();
		this.loadInforBar();
	}

	// Token: 0x06000476 RID: 1142 RVA: 0x000546FC File Offset: 0x000528FC
	public void doMenuInforMe()
	{
		GameScr.scrMain.clear();
		GameScr.scrInfo.clear();
		GameScr.isViewNext = false;
		this.cmdBag = new Command(mResources.MENUME[0], 1100011);
		this.cmdSkill = new Command(mResources.MENUME[1], 1100012);
		this.cmdTiemnang = new Command(mResources.MENUME[2], 1100013);
		this.cmdInfo = new Command(mResources.MENUME[3], 1100014);
		this.cmdtrangbi = new Command(mResources.MENUME[4], 1100015);
		MyVector myVector = new MyVector();
		myVector.addElement(this.cmdBag);
		myVector.addElement(this.cmdSkill);
		myVector.addElement(this.cmdTiemnang);
		myVector.addElement(this.cmdInfo);
		myVector.addElement(this.cmdtrangbi);
		GameCanvas.menu.startAt(myVector, 3);
	}

	// Token: 0x06000477 RID: 1143 RVA: 0x000547F0 File Offset: 0x000529F0
	public void doMenusynthesis()
	{
		MyVector myVector = new MyVector();
		myVector.addElement(new Command(mResources.SYNTHESIS[0], 110002));
		myVector.addElement(new Command(mResources.SYNTHESIS[1], 1100032));
		myVector.addElement(new Command(mResources.SYNTHESIS[2], 1100033));
		GameCanvas.menu.startAt(myVector, 3);
	}

	// Token: 0x06000478 RID: 1144 RVA: 0x0005485C File Offset: 0x00052A5C
	public static void loadCamera(bool fullmScreen, int cx, int cy)
	{
		GameScr.gW = GameCanvas.w;
		GameScr.cmdBarH = 39;
		GameScr.gH = GameCanvas.h;
		GameScr.cmdBarW = GameScr.gW;
		GameScr.cmdBarX = 0;
		GameScr.cmdBarY = GameCanvas.h - Paint.hTab - GameScr.cmdBarH;
		GameScr.girlHPBarY = 0;
		GameScr.csPadMaxH = GameCanvas.h / 6;
		bool flag = GameScr.csPadMaxH < 48;
		if (flag)
		{
			GameScr.csPadMaxH = 48;
		}
		GameScr.gW2 = GameScr.gW >> 1;
		GameScr.gH2 = GameScr.gH >> 1;
		GameScr.gW3 = GameScr.gW / 3;
		GameScr.gH3 = GameScr.gH / 3;
		GameScr.gW23 = GameScr.gH - 120;
		GameScr.gH23 = GameScr.gH * 2 / 3;
		GameScr.gW34 = 3 * GameScr.gW / 4;
		GameScr.gH34 = 3 * GameScr.gH / 4;
		GameScr.gW6 = GameScr.gW / 6;
		GameScr.gH6 = GameScr.gH / 6;
		GameScr.gssw = GameScr.gW / (int)TileMap.size + 2;
		GameScr.gssh = GameScr.gH / (int)TileMap.size + 2;
		bool flag2 = GameScr.gW % 24 != 0;
		if (flag2)
		{
			GameScr.gssw++;
		}
		GameScr.cmxLim = (TileMap.tmw - 1) * (int)TileMap.size - GameScr.gW;
		GameScr.cmyLim = (TileMap.tmh - 1) * (int)TileMap.size - GameScr.gH;
		bool flag3 = cx == -1 && cy == -1;
		if (flag3)
		{
			GameScr.cmx = (GameScr.cmtoX = global::Char.myCharz().cx - GameScr.gW2 + GameScr.gW6 * global::Char.myCharz().cdir);
			GameScr.cmy = (GameScr.cmtoY = global::Char.myCharz().cy - GameScr.gH23);
		}
		else
		{
			GameScr.cmx = (GameScr.cmtoX = cx - GameScr.gW23 + GameScr.gW6 * global::Char.myCharz().cdir);
			GameScr.cmy = (GameScr.cmtoY = cy - GameScr.gH23);
		}
		GameScr.firstY = GameScr.cmy;
		bool flag4 = GameScr.cmx < 24;
		if (flag4)
		{
			GameScr.cmx = (GameScr.cmtoX = 24);
		}
		bool flag5 = GameScr.cmx > GameScr.cmxLim;
		if (flag5)
		{
			GameScr.cmx = (GameScr.cmtoX = GameScr.cmxLim);
		}
		bool flag6 = GameScr.cmy < 0;
		if (flag6)
		{
			GameScr.cmy = (GameScr.cmtoY = 0);
		}
		bool flag7 = GameScr.cmy > GameScr.cmyLim;
		if (flag7)
		{
			GameScr.cmy = (GameScr.cmtoY = GameScr.cmyLim);
		}
		GameScr.gssx = GameScr.cmx / (int)TileMap.size - 1;
		bool flag8 = GameScr.gssx < 0;
		if (flag8)
		{
			GameScr.gssx = 0;
		}
		GameScr.gssy = GameScr.cmy / (int)TileMap.size;
		GameScr.gssxe = GameScr.gssx + GameScr.gssw;
		GameScr.gssye = GameScr.gssy + GameScr.gssh;
		bool flag9 = GameScr.gssy < 0;
		if (flag9)
		{
			GameScr.gssy = 0;
		}
		bool flag10 = GameScr.gssye > TileMap.tmh - 1;
		if (flag10)
		{
			GameScr.gssye = TileMap.tmh - 1;
		}
		TileMap.countx = (GameScr.gssxe - GameScr.gssx) * 4;
		bool flag11 = TileMap.countx > TileMap.tmw;
		if (flag11)
		{
			TileMap.countx = TileMap.tmw;
		}
		TileMap.county = (GameScr.gssye - GameScr.gssy) * 4;
		bool flag12 = TileMap.county > TileMap.tmh;
		if (flag12)
		{
			TileMap.county = TileMap.tmh;
		}
		TileMap.gssx = (global::Char.myCharz().cx - 2 * GameScr.gW) / (int)TileMap.size;
		bool flag13 = TileMap.gssx < 0;
		if (flag13)
		{
			TileMap.gssx = 0;
		}
		TileMap.gssxe = TileMap.gssx + TileMap.countx;
		bool flag14 = TileMap.gssxe > TileMap.tmw;
		if (flag14)
		{
			TileMap.gssxe = TileMap.tmw;
		}
		TileMap.gssy = (global::Char.myCharz().cy - 2 * GameScr.gH) / (int)TileMap.size;
		bool flag15 = TileMap.gssy < 0;
		if (flag15)
		{
			TileMap.gssy = 0;
		}
		TileMap.gssye = TileMap.gssy + TileMap.county;
		bool flag16 = TileMap.gssye > TileMap.tmh;
		if (flag16)
		{
			TileMap.gssye = TileMap.tmh;
		}
		ChatTextField.gI().parentScreen = GameScr.instance;
		ChatTextField.gI().tfChat.y = GameCanvas.h - 35 - ChatTextField.gI().tfChat.height;
		ChatTextField.gI().initChatTextField();
		bool isTouch = GameCanvas.isTouch;
		if (isTouch)
		{
			GameScr.yTouchBar = GameScr.gH - 88;
			GameScr.xC = GameScr.gW - 40;
			GameScr.yC = 2;
			bool flag17 = GameCanvas.w <= 240;
			if (flag17)
			{
				GameScr.xC = GameScr.gW - 35;
				GameScr.yC = 5;
			}
			GameScr.xF = GameScr.gW - 55;
			GameScr.yF = GameScr.yTouchBar + 35;
			GameScr.xTG = GameScr.gW - 37;
			GameScr.yTG = GameScr.yTouchBar - 1;
			bool flag18 = GameCanvas.w >= 450;
			if (flag18)
			{
				GameScr.yTG -= 12;
				GameScr.yHP -= 7;
				GameScr.xF -= 10;
				GameScr.yF -= 5;
				GameScr.xTG -= 10;
			}
		}
		GameScr.setSkillBarPosition();
		GameScr.disXC = ((GameCanvas.w <= 200) ? 30 : 40);
		bool flag19 = Rms.loadRMSInt("viewchat") == -1;
		if (flag19)
		{
			GameCanvas.panel.isViewChatServer = true;
		}
		else
		{
			GameCanvas.panel.isViewChatServer = Rms.loadRMSInt("viewchat") == 1;
		}
	}

	// Token: 0x06000479 RID: 1145 RVA: 0x00054E0C File Offset: 0x0005300C
	public static void setSkillBarPosition()
	{
		Skill[] array = ((!GameCanvas.isTouch) ? GameScr.keySkill : GameScr.onScreenSkill);
		GameScr.xS = new int[array.Length];
		GameScr.yS = new int[array.Length];
		bool flag = GameCanvas.isTouchControlSmallScreen && GameScr.isUseTouch;
		if (flag)
		{
			GameScr.xSkill = 23;
			GameScr.ySkill = 52;
			GameScr.padSkill = 5;
			for (int i = 0; i < GameScr.xS.Length; i++)
			{
				GameScr.xS[i] = i * (25 + GameScr.padSkill);
				GameScr.yS[i] = GameScr.ySkill;
				bool flag2 = GameScr.xS.Length > 5 && i >= GameScr.xS.Length / 2;
				if (flag2)
				{
					GameScr.xS[i] = (i - GameScr.xS.Length / 2) * (25 + GameScr.padSkill);
					GameScr.yS[i] = GameScr.ySkill - 32;
				}
			}
			GameScr.xHP = array.Length * (25 + GameScr.padSkill);
			GameScr.yHP = GameScr.ySkill;
		}
		else
		{
			GameScr.wSkill = 30;
			bool flag3 = GameCanvas.w <= 320;
			if (flag3)
			{
				GameScr.ySkill = GameScr.gH - GameScr.wSkill - 6;
				GameScr.xSkill = GameScr.gW2 - array.Length * GameScr.wSkill / 2 - 25;
			}
			else
			{
				GameScr.wSkill = 40;
				GameScr.xSkill = 10;
				GameScr.ySkill = GameCanvas.h - GameScr.wSkill + 7;
			}
			for (int j = 0; j < GameScr.xS.Length; j++)
			{
				GameScr.xS[j] = j * GameScr.wSkill;
				GameScr.yS[j] = GameScr.ySkill;
				bool flag4 = GameScr.xS.Length > 5 && j >= GameScr.xS.Length / 2;
				if (flag4)
				{
					GameScr.xS[j] = (j - GameScr.xS.Length / 2) * GameScr.wSkill;
					GameScr.yS[j] = GameScr.ySkill - 32;
				}
			}
			GameScr.xHP = array.Length * GameScr.wSkill;
			GameScr.yHP = GameScr.ySkill;
		}
		bool flag5 = !GameCanvas.isTouch;
		if (!flag5)
		{
			GameScr.xSkill = 17;
			GameScr.ySkill = GameCanvas.h - 40;
			bool flag6 = GameScr.gamePad.isSmallGamePad && GameScr.isAnalog == 1;
			if (flag6)
			{
				GameScr.xHP = array.Length * GameScr.wSkill;
				GameScr.yHP = GameScr.ySkill;
			}
			else
			{
				GameScr.xHP = GameCanvas.w - 45;
				GameScr.yHP = GameCanvas.h - 45;
			}
			GameScr.setTouchBtn();
			for (int k = 0; k < GameScr.xS.Length; k++)
			{
				GameScr.xS[k] = k * GameScr.wSkill;
				GameScr.yS[k] = GameScr.ySkill;
				bool flag7 = GameScr.xS.Length > 5 && k >= GameScr.xS.Length / 2;
				if (flag7)
				{
					GameScr.xS[k] = (k - GameScr.xS.Length / 2) * GameScr.wSkill;
					GameScr.yS[k] = GameScr.ySkill - 32;
				}
			}
		}
	}

	// Token: 0x0600047A RID: 1146 RVA: 0x0005513C File Offset: 0x0005333C
	private static void updateCamera()
	{
		bool flag = GameScr.isPaintOther;
		if (!flag)
		{
			bool flag2 = GameScr.cmx != GameScr.cmtoX || GameScr.cmy != GameScr.cmtoY;
			if (flag2)
			{
				GameScr.cmvx = GameScr.cmtoX - GameScr.cmx << 2;
				GameScr.cmvy = GameScr.cmtoY - GameScr.cmy << 2;
				GameScr.cmdx += GameScr.cmvx;
				GameScr.cmx += GameScr.cmdx >> 4;
				GameScr.cmdx &= 15;
				GameScr.cmdy += GameScr.cmvy;
				GameScr.cmy += GameScr.cmdy >> 4;
				GameScr.cmdy &= 15;
				bool flag3 = GameScr.cmx < 24;
				if (flag3)
				{
					GameScr.cmx = 24;
				}
				bool flag4 = GameScr.cmx > GameScr.cmxLim;
				if (flag4)
				{
					GameScr.cmx = GameScr.cmxLim;
				}
				bool flag5 = GameScr.cmy < 0;
				if (flag5)
				{
					GameScr.cmy = 0;
				}
				bool flag6 = GameScr.cmy > GameScr.cmyLim;
				if (flag6)
				{
					GameScr.cmy = GameScr.cmyLim;
				}
			}
			GameScr.gssx = GameScr.cmx / (int)TileMap.size - 1;
			bool flag7 = GameScr.gssx < 0;
			if (flag7)
			{
				GameScr.gssx = 0;
			}
			GameScr.gssy = GameScr.cmy / (int)TileMap.size;
			GameScr.gssxe = GameScr.gssx + GameScr.gssw;
			GameScr.gssye = GameScr.gssy + GameScr.gssh;
			bool flag8 = GameScr.gssy < 0;
			if (flag8)
			{
				GameScr.gssy = 0;
			}
			bool flag9 = GameScr.gssye > TileMap.tmh - 1;
			if (flag9)
			{
				GameScr.gssye = TileMap.tmh - 1;
			}
			TileMap.gssx = (global::Char.myCharz().cx - 2 * GameScr.gW) / (int)TileMap.size;
			bool flag10 = TileMap.gssx < 0;
			if (flag10)
			{
				TileMap.gssx = 0;
			}
			TileMap.gssxe = TileMap.gssx + TileMap.countx;
			bool flag11 = TileMap.gssxe > TileMap.tmw;
			if (flag11)
			{
				TileMap.gssxe = TileMap.tmw;
				TileMap.gssx = TileMap.gssxe - TileMap.countx;
			}
			TileMap.gssy = (global::Char.myCharz().cy - 2 * GameScr.gH) / (int)TileMap.size;
			bool flag12 = TileMap.gssy < 0;
			if (flag12)
			{
				TileMap.gssy = 0;
			}
			TileMap.gssye = TileMap.gssy + TileMap.county;
			bool flag13 = TileMap.gssye > TileMap.tmh;
			if (flag13)
			{
				TileMap.gssye = TileMap.tmh;
				TileMap.gssy = TileMap.gssye - TileMap.county;
			}
			GameScr.scrMain.updatecm();
			GameScr.scrInfo.updatecm();
		}
	}

	// Token: 0x0600047B RID: 1147 RVA: 0x000553F0 File Offset: 0x000535F0
	public bool testAct()
	{
		for (sbyte b = 2; b < 9; b += 2)
		{
			bool flag = GameCanvas.keyHold[(int)b];
			if (flag)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600047C RID: 1148 RVA: 0x00055428 File Offset: 0x00053628
	public void clanInvite(string strInvite, int clanID, int code)
	{
		ClanObject clanObject = new ClanObject();
		clanObject.code = code;
		clanObject.clanID = clanID;
		this.startYesNoPopUp(strInvite, new Command(mResources.YES, 12002, clanObject), new Command(mResources.NO, 12003, clanObject));
	}

	// Token: 0x0600047D RID: 1149 RVA: 0x00055474 File Offset: 0x00053674
	public void playerMenu(global::Char c)
	{
		this.auto = 0;
		GameCanvas.clearKeyHold();
		bool flag = global::Char.myCharz().charFocus.charID < 0 || global::Char.myCharz().charID < 0;
		if (!flag)
		{
			MyVector vPlayerMenu = GameCanvas.panel.vPlayerMenu;
			bool flag2 = vPlayerMenu.size() > 0;
			if (!flag2)
			{
				bool flag3 = global::Char.myCharz().taskMaint != null && global::Char.myCharz().taskMaint.taskId > 1;
				if (flag3)
				{
					vPlayerMenu.addElement(new Command(mResources.make_friend, 11112, global::Char.myCharz().charFocus));
					vPlayerMenu.addElement(new Command(mResources.trade, 11113, global::Char.myCharz().charFocus));
				}
				bool flag4 = global::Char.myCharz().clan != null && global::Char.myCharz().role < 2 && global::Char.myCharz().charFocus.clanID == -1;
				if (flag4)
				{
					vPlayerMenu.addElement(new Command(mResources.CHAR_ORDER[4], 110391));
				}
				bool flag5 = global::Char.myCharz().charFocus.statusMe != 14 && global::Char.myCharz().charFocus.statusMe != 5;
				if (flag5)
				{
					bool flag6 = global::Char.myCharz().taskMaint != null && global::Char.myCharz().taskMaint.taskId >= 14;
					if (flag6)
					{
						vPlayerMenu.addElement(new Command(mResources.CHAR_ORDER[0], 2003));
					}
				}
				else
				{
					bool flag7 = global::Char.myCharz().myskill.template.type != 4;
					if (flag7)
					{
					}
				}
				bool flag8 = global::Char.myCharz().clan != null && global::Char.myCharz().clan.ID == global::Char.myCharz().charFocus.clanID && global::Char.myCharz().charFocus.statusMe != 14 && global::Char.myCharz().taskMaint != null && global::Char.myCharz().taskMaint.taskId >= 14;
				if (flag8)
				{
					vPlayerMenu.addElement(new Command(mResources.CHAR_ORDER[1], 2004));
				}
				int num = global::Char.myCharz().nClass.skillTemplates.Length;
				for (int i = 0; i < num; i++)
				{
					SkillTemplate skillTemplate = global::Char.myCharz().nClass.skillTemplates[i];
					Skill skill = global::Char.myCharz().getSkill(skillTemplate);
					bool flag9 = skill != null && skillTemplate.isBuffToPlayer() && skill.point >= 1;
					if (flag9)
					{
						vPlayerMenu.addElement(new Command(skillTemplate.name, 12004, skill));
					}
				}
			}
		}
	}

	// Token: 0x0600047E RID: 1150 RVA: 0x0005573C File Offset: 0x0005393C
	public bool isAttack()
	{
		bool flag = this.checkClickToBotton(global::Char.myCharz().charFocus);
		bool flag2;
		if (flag)
		{
			flag2 = false;
		}
		else
		{
			bool flag3 = this.checkClickToBotton(global::Char.myCharz().mobFocus);
			if (flag3)
			{
				flag2 = false;
			}
			else
			{
				bool flag4 = this.checkClickToBotton(global::Char.myCharz().npcFocus);
				if (flag4)
				{
					flag2 = false;
				}
				else
				{
					bool isShow = ChatTextField.gI().isShow;
					if (isShow)
					{
						flag2 = false;
					}
					else
					{
						bool flag5 = InfoDlg.isLock || global::Char.myCharz().isLockAttack || global::Char.isLockKey;
						if (flag5)
						{
							flag2 = false;
						}
						else
						{
							bool flag6 = global::Char.myCharz().myskill != null && global::Char.myCharz().myskill.template.id == 6 && global::Char.myCharz().itemFocus != null;
							if (flag6)
							{
								this.pickItem();
								flag2 = false;
							}
							else
							{
								bool flag7 = global::Char.myCharz().myskill != null && global::Char.myCharz().myskill.template.type == 2 && global::Char.myCharz().npcFocus == null && global::Char.myCharz().myskill.template.id != 6;
								if (flag7)
								{
									bool flag8 = !this.checkSkillValid();
									flag2 = !flag8;
								}
								else
								{
									bool flag9 = global::Char.myCharz().skillPaint != null || (global::Char.myCharz().mobFocus == null && global::Char.myCharz().npcFocus == null && global::Char.myCharz().charFocus == null && global::Char.myCharz().itemFocus == null);
									if (flag9)
									{
										flag2 = false;
									}
									else
									{
										bool flag10 = global::Char.myCharz().mobFocus != null;
										if (flag10)
										{
											bool flag11 = global::Char.myCharz().mobFocus.isBigBoss() && global::Char.myCharz().mobFocus.status == 4;
											if (flag11)
											{
												global::Char.myCharz().mobFocus = null;
												global::Char.myCharz().currentMovePoint = null;
											}
											GameScr.isAutoPlay = true;
											bool flag12 = !this.isMeCanAttackMob(global::Char.myCharz().mobFocus);
											if (flag12)
											{
												Res.outz("can not attack");
												flag2 = false;
											}
											else
											{
												bool flag13 = this.mobCapcha != null;
												if (flag13)
												{
													flag2 = false;
												}
												else
												{
													bool flag14 = global::Char.myCharz().myskill == null;
													if (flag14)
													{
														flag2 = false;
													}
													else
													{
														bool flag15 = global::Char.myCharz().isSelectingSkillUseAlone();
														if (flag15)
														{
															flag2 = false;
														}
														else
														{
															int num = -1;
															int num2 = Res.abs(global::Char.myCharz().cx - GameScr.cmx) * mGraphics.zoomLevel;
															bool flag16 = global::Char.myCharz().charFocus != null;
															if (flag16)
															{
																num = Res.abs(global::Char.myCharz().cx - global::Char.myCharz().charFocus.cx) * mGraphics.zoomLevel;
															}
															else
															{
																bool flag17 = global::Char.myCharz().mobFocus != null;
																if (flag17)
																{
																	num = Res.abs(global::Char.myCharz().cx - global::Char.myCharz().mobFocus.x) * mGraphics.zoomLevel;
																}
															}
															bool flag18 = (global::Char.myCharz().mobFocus.status == 1 || global::Char.myCharz().mobFocus.status == 0 || global::Char.myCharz().myskill.template.type == 4 || num == -1 || num > num2) && global::Char.myCharz().myskill.template.type == 4;
															if (flag18)
															{
																bool flag19 = global::Char.myCharz().mobFocus.x < global::Char.myCharz().cx;
																if (flag19)
																{
																	global::Char.myCharz().cdir = -1;
																}
																else
																{
																	global::Char.myCharz().cdir = 1;
																}
																this.doSelectSkill(global::Char.myCharz().myskill, true);
															}
															bool flag20 = !this.checkSkillValid();
															if (flag20)
															{
																flag2 = false;
															}
															else
															{
																bool flag21 = global::Char.myCharz().cx < global::Char.myCharz().mobFocus.getX();
																if (flag21)
																{
																	global::Char.myCharz().cdir = 1;
																}
																else
																{
																	global::Char.myCharz().cdir = -1;
																}
																int num3 = global::Math.abs(global::Char.myCharz().cx - global::Char.myCharz().mobFocus.getX());
																int num4 = global::Math.abs(global::Char.myCharz().cy - global::Char.myCharz().mobFocus.getY());
																global::Char.myCharz().cvx = 0;
																bool flag22 = num3 <= global::Char.myCharz().myskill.dx && num4 <= global::Char.myCharz().myskill.dy;
																if (flag22)
																{
																	bool flag23 = global::Char.myCharz().myskill.template.id == 20;
																	if (flag23)
																	{
																		flag2 = true;
																	}
																	else
																	{
																		bool flag24 = num4 > num3 && Res.abs(global::Char.myCharz().cy - global::Char.myCharz().mobFocus.getY()) > 30 && global::Char.myCharz().mobFocus.getTemplate().type == 4;
																		if (flag24)
																		{
																			global::Char.myCharz().currentMovePoint = new MovePoint(global::Char.myCharz().cx + global::Char.myCharz().cdir, global::Char.myCharz().mobFocus.getY());
																			global::Char.myCharz().endMovePointCommand = new Command(null, null, 8002, null);
																			GameCanvas.clearKeyHold();
																			GameCanvas.clearKeyPressed();
																			flag2 = false;
																		}
																		else
																		{
																			int num5 = 20;
																			bool flag25 = false;
																			bool flag26 = global::Char.myCharz().mobFocus is BigBoss || global::Char.myCharz().mobFocus is BigBoss2;
																			if (flag26)
																			{
																				flag25 = true;
																			}
																			bool flag27 = global::Char.myCharz().myskill.dx > 100;
																			if (flag27)
																			{
																				num5 = 60;
																				bool flag28 = num3 < 20;
																				if (flag28)
																				{
																					global::Char.myCharz().createShadow(global::Char.myCharz().cx, global::Char.myCharz().cy, 10);
																				}
																			}
																			bool flag29 = false;
																			bool flag30 = (TileMap.tileTypeAtPixel(global::Char.myCharz().cx, global::Char.myCharz().cy + 3) & 2) == 2;
																			if (flag30)
																			{
																				int num6 = ((global::Char.myCharz().cx > global::Char.myCharz().mobFocus.getX()) ? 1 : (-1));
																				bool flag31 = (TileMap.tileTypeAtPixel(global::Char.myCharz().mobFocus.getX() + num5 * num6, global::Char.myCharz().cy + 3) & 2) != 2;
																				if (flag31)
																				{
																					flag29 = true;
																				}
																			}
																			bool flag32 = num3 <= num5 && !flag29;
																			if (flag32)
																			{
																				bool flag33 = global::Char.myCharz().cx > global::Char.myCharz().mobFocus.getX();
																				if (flag33)
																				{
																					int num7 = global::Char.myCharz().mobFocus.getX() + num5 + (flag25 ? 30 : 0);
																					int i = global::Char.myCharz().mobFocus.getX();
																					bool flag34 = false;
																					while (i < num7)
																					{
																						bool flag35 = TileMap.tileTypeAtPixel(i, global::Char.myCharz().cy + 3) == 8 || TileMap.tileTypeAtPixel(i, global::Char.myCharz().cy + 3) == 4;
																						if (flag35)
																						{
																							flag34 = true;
																							break;
																						}
																						i += 24;
																					}
																					bool flag36 = flag34;
																					if (flag36)
																					{
																						global::Char.myCharz().cx = i - 24;
																					}
																					else
																					{
																						global::Char.myCharz().cx = num7;
																					}
																					global::Char.myCharz().cdir = -1;
																				}
																				else
																				{
																					int num8 = global::Char.myCharz().mobFocus.getX() - num5 - (flag25 ? 30 : 0);
																					int j = global::Char.myCharz().mobFocus.getX();
																					bool flag37 = false;
																					while (j > num8)
																					{
																						bool flag38 = TileMap.tileTypeAtPixel(j, global::Char.myCharz().cy + 3) == 8 || TileMap.tileTypeAtPixel(j, global::Char.myCharz().cy + 3) == 4;
																						if (flag38)
																						{
																							flag37 = true;
																							break;
																						}
																						j -= 24;
																					}
																					bool flag39 = flag37;
																					if (flag39)
																					{
																						global::Char.myCharz().cx = j + 24;
																					}
																					else
																					{
																						global::Char.myCharz().cx = num8;
																					}
																					global::Char.myCharz().cdir = 1;
																				}
																				Service.gI().charMove();
																			}
																			GameCanvas.clearKeyHold();
																			GameCanvas.clearKeyPressed();
																			flag2 = true;
																		}
																	}
																}
																else
																{
																	bool flag40 = false;
																	bool flag41 = global::Char.myCharz().mobFocus is BigBoss || global::Char.myCharz().mobFocus is BigBoss2;
																	if (flag41)
																	{
																		flag40 = true;
																	}
																	int num9 = (global::Char.myCharz().myskill.dx - ((!flag40) ? 20 : 50)) * ((global::Char.myCharz().cx > global::Char.myCharz().mobFocus.getX()) ? 1 : (-1));
																	bool flag42 = num3 <= global::Char.myCharz().myskill.dx;
																	if (flag42)
																	{
																		num9 = 0;
																	}
																	global::Char.myCharz().currentMovePoint = new MovePoint(global::Char.myCharz().mobFocus.getX() + num9, global::Char.myCharz().mobFocus.getY());
																	global::Char.myCharz().endMovePointCommand = new Command(null, null, 8002, null);
																	GameCanvas.clearKeyHold();
																	GameCanvas.clearKeyPressed();
																	flag2 = false;
																}
															}
														}
													}
												}
											}
										}
										else
										{
											bool flag43 = global::Char.myCharz().npcFocus != null;
											if (flag43)
											{
												bool isHide = global::Char.myCharz().npcFocus.isHide;
												if (isHide)
												{
													flag2 = false;
												}
												else
												{
													bool flag44 = global::Char.myCharz().cx < global::Char.myCharz().npcFocus.cx;
													if (flag44)
													{
														global::Char.myCharz().cdir = 1;
													}
													else
													{
														global::Char.myCharz().cdir = -1;
													}
													bool flag45 = global::Char.myCharz().cx < global::Char.myCharz().npcFocus.cx;
													if (flag45)
													{
														global::Char.myCharz().npcFocus.cdir = -1;
													}
													else
													{
														global::Char.myCharz().npcFocus.cdir = 1;
													}
													int num10 = global::Math.abs(global::Char.myCharz().cx - global::Char.myCharz().npcFocus.cx);
													int num11 = global::Math.abs(global::Char.myCharz().cy - global::Char.myCharz().npcFocus.cy);
													bool flag46 = num11 > 40;
													if (flag46)
													{
														global::Char.myCharz().cy = global::Char.myCharz().npcFocus.cy - 40;
													}
													bool flag47 = num10 < 60;
													if (flag47)
													{
														GameCanvas.clearKeyHold();
														GameCanvas.clearKeyPressed();
														bool flag48 = this.tMenuDelay == 0;
														if (flag48)
														{
															bool flag49 = global::Char.myCharz().taskMaint != null && global::Char.myCharz().taskMaint.taskId == 0;
															if (flag49)
															{
																bool flag50 = global::Char.myCharz().taskMaint.index < 4 && global::Char.myCharz().npcFocus.template.npcTemplateId == 4;
																if (flag50)
																{
																	return false;
																}
																bool flag51 = global::Char.myCharz().taskMaint.index < 3 && global::Char.myCharz().npcFocus.template.npcTemplateId == 3;
																if (flag51)
																{
																	return false;
																}
															}
															this.tMenuDelay = 50;
															InfoDlg.showWait();
															Service.gI().charMove();
															Service.gI().openMenu(global::Char.myCharz().npcFocus.template.npcTemplateId);
														}
													}
													else
													{
														int num12 = (20 + Res.r.nextInt(20)) * ((global::Char.myCharz().cx > global::Char.myCharz().npcFocus.cx) ? 1 : (-1));
														global::Char.myCharz().currentMovePoint = new MovePoint(global::Char.myCharz().npcFocus.cx + num12, global::Char.myCharz().cy);
														global::Char.myCharz().endMovePointCommand = new Command(null, null, 8002, null);
														GameCanvas.clearKeyHold();
														GameCanvas.clearKeyPressed();
													}
													flag2 = false;
												}
											}
											else
											{
												bool flag52 = global::Char.myCharz().charFocus != null;
												if (flag52)
												{
													bool flag53 = this.mobCapcha != null;
													if (flag53)
													{
														flag2 = false;
													}
													else
													{
														bool flag54 = global::Char.myCharz().cx < global::Char.myCharz().charFocus.cx;
														if (flag54)
														{
															global::Char.myCharz().cdir = 1;
														}
														else
														{
															global::Char.myCharz().cdir = -1;
														}
														int num13 = global::Math.abs(global::Char.myCharz().cx - global::Char.myCharz().charFocus.cx);
														int num14 = global::Math.abs(global::Char.myCharz().cy - global::Char.myCharz().charFocus.cy);
														bool flag55 = global::Char.myCharz().isMeCanAttackOtherPlayer(global::Char.myCharz().charFocus) || global::Char.myCharz().isSelectingSkillBuffToPlayer();
														if (flag55)
														{
															bool flag56 = global::Char.myCharz().myskill == null;
															if (flag56)
															{
																flag2 = false;
															}
															else
															{
																bool flag57 = !this.checkSkillValid();
																if (flag57)
																{
																	flag2 = false;
																}
																else
																{
																	bool flag58 = global::Char.myCharz().cx < global::Char.myCharz().charFocus.cx;
																	if (flag58)
																	{
																		global::Char.myCharz().cdir = 1;
																	}
																	else
																	{
																		global::Char.myCharz().cdir = -1;
																	}
																	global::Char.myCharz().cvx = 0;
																	bool flag59 = num13 <= global::Char.myCharz().myskill.dx && num14 <= global::Char.myCharz().myskill.dy;
																	if (flag59)
																	{
																		bool flag60 = global::Char.myCharz().myskill.template.id == 20;
																		if (flag60)
																		{
																			flag2 = true;
																		}
																		else
																		{
																			int num15 = 20;
																			bool flag61 = global::Char.myCharz().myskill.dx > 60;
																			if (flag61)
																			{
																				num15 = 60;
																				bool flag62 = num13 < 20;
																				if (flag62)
																				{
																					global::Char.myCharz().createShadow(global::Char.myCharz().cx, global::Char.myCharz().cy, 10);
																				}
																			}
																			bool flag63 = false;
																			bool flag64 = (TileMap.tileTypeAtPixel(global::Char.myCharz().cx, global::Char.myCharz().cy + 3) & 2) == 2;
																			if (flag64)
																			{
																				int num16 = ((global::Char.myCharz().cx > global::Char.myCharz().charFocus.cx) ? 1 : (-1));
																				bool flag65 = (TileMap.tileTypeAtPixel(global::Char.myCharz().charFocus.cx + num15 * num16, global::Char.myCharz().cy + 3) & 2) != 2;
																				if (flag65)
																				{
																					flag63 = true;
																				}
																			}
																			bool flag66 = num13 <= num15 && !flag63;
																			if (flag66)
																			{
																				bool flag67 = global::Char.myCharz().cx > global::Char.myCharz().charFocus.cx;
																				if (flag67)
																				{
																					global::Char.myCharz().cx = global::Char.myCharz().charFocus.cx + num15;
																					global::Char.myCharz().cdir = -1;
																				}
																				else
																				{
																					global::Char.myCharz().cx = global::Char.myCharz().charFocus.cx - num15;
																					global::Char.myCharz().cdir = 1;
																				}
																				Service.gI().charMove();
																			}
																			GameCanvas.clearKeyHold();
																			GameCanvas.clearKeyPressed();
																			flag2 = true;
																		}
																	}
																	else
																	{
																		int num17 = (global::Char.myCharz().myskill.dx - 20) * ((global::Char.myCharz().cx > global::Char.myCharz().charFocus.cx) ? 1 : (-1));
																		bool flag68 = num13 <= global::Char.myCharz().myskill.dx;
																		if (flag68)
																		{
																			num17 = 0;
																		}
																		global::Char.myCharz().currentMovePoint = new MovePoint(global::Char.myCharz().charFocus.cx + num17, global::Char.myCharz().charFocus.cy);
																		global::Char.myCharz().endMovePointCommand = new Command(null, null, 8002, null);
																		GameCanvas.clearKeyHold();
																		GameCanvas.clearKeyPressed();
																		flag2 = false;
																	}
																}
															}
														}
														else
														{
															bool flag69 = num13 < 60 && num14 < 40;
															if (flag69)
															{
																this.playerMenu(global::Char.myCharz().charFocus);
																bool flag70 = !GameCanvas.isTouch && global::Char.myCharz().charFocus.charID >= 0 && TileMap.mapID != 51 && TileMap.mapID != 52 && this.popUpYesNo == null;
																if (flag70)
																{
																	GameCanvas.panel.setTypePlayerMenu(global::Char.myCharz().charFocus);
																	GameCanvas.panel.show();
																	Service.gI().getPlayerMenu(global::Char.myCharz().charFocus.charID);
																	Service.gI().messagePlayerMenu(global::Char.myCharz().charFocus.charID);
																}
															}
															else
															{
																int num18 = (20 + Res.r.nextInt(20)) * ((global::Char.myCharz().cx > global::Char.myCharz().charFocus.cx) ? 1 : (-1));
																global::Char.myCharz().currentMovePoint = new MovePoint(global::Char.myCharz().charFocus.cx + num18, global::Char.myCharz().charFocus.cy);
																global::Char.myCharz().endMovePointCommand = new Command(null, null, 8002, null);
																GameCanvas.clearKeyHold();
																GameCanvas.clearKeyPressed();
															}
															flag2 = false;
														}
													}
												}
												else
												{
													bool flag71 = global::Char.myCharz().itemFocus != null;
													if (flag71)
													{
														this.pickItem();
														flag2 = false;
													}
													else
													{
														flag2 = true;
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
		return flag2;
	}

	// Token: 0x0600047F RID: 1151 RVA: 0x00056888 File Offset: 0x00054A88
	public bool isMeCanAttackMob(Mob m)
	{
		bool flag = m == null;
		bool flag2;
		if (flag)
		{
			flag2 = false;
		}
		else
		{
			bool flag3 = global::Char.myCharz().cTypePk == 5;
			if (flag3)
			{
				flag2 = true;
			}
			else
			{
				bool flag4 = global::Char.myCharz().isAttacPlayerStatus() && !m.isMobMe;
				if (flag4)
				{
					flag2 = false;
				}
				else
				{
					bool flag5 = global::Char.myCharz().mobMe != null && m.Equals(global::Char.myCharz().mobMe);
					if (flag5)
					{
						flag2 = false;
					}
					else
					{
						global::Char @char = GameScr.findCharInMap(m.mobId);
						bool flag6 = @char == null;
						if (flag6)
						{
							flag2 = true;
						}
						else
						{
							bool flag7 = @char.cTypePk == 5;
							if (flag7)
							{
								flag2 = true;
							}
							else
							{
								bool flag8 = global::Char.myCharz().isMeCanAttackOtherPlayer(@char);
								flag2 = flag8;
							}
						}
					}
				}
			}
		}
		return flag2;
	}

	// Token: 0x06000480 RID: 1152 RVA: 0x00056958 File Offset: 0x00054B58
	private bool checkSkillValid()
	{
		bool flag = global::Char.myCharz().myskill != null && ((global::Char.myCharz().myskill.template.manaUseType != 1 && global::Char.myCharz().cMP < (long)global::Char.myCharz().myskill.manaUse) || (global::Char.myCharz().myskill.template.manaUseType == 1 && global::Char.myCharz().cMP < global::Char.myCharz().cMPFull * (long)global::Char.myCharz().myskill.manaUse / 100L));
		bool flag2;
		if (flag)
		{
			GameScr.info1.addInfo(mResources.NOT_ENOUGH_MP, 0);
			this.auto = 0;
			flag2 = false;
		}
		else
		{
			bool flag3 = global::Char.myCharz().myskill == null || (global::Char.myCharz().myskill.template.maxPoint > 0 && global::Char.myCharz().myskill.point == 0);
			if (flag3)
			{
				GameCanvas.startOKDlg(mResources.SKILL_FAIL);
				flag2 = false;
			}
			else
			{
				flag2 = true;
			}
		}
		return flag2;
	}

	// Token: 0x06000481 RID: 1153 RVA: 0x00056A68 File Offset: 0x00054C68
	private bool checkSkillValid2()
	{
		bool flag = global::Char.myCharz().myskill != null && ((global::Char.myCharz().myskill.template.manaUseType != 1 && global::Char.myCharz().cMP < (long)global::Char.myCharz().myskill.manaUse) || (global::Char.myCharz().myskill.template.manaUseType == 1 && global::Char.myCharz().cMP < global::Char.myCharz().cMPFull * (long)global::Char.myCharz().myskill.manaUse / 100L));
		bool flag2;
		if (flag)
		{
			flag2 = false;
		}
		else
		{
			bool flag3 = global::Char.myCharz().myskill == null || (global::Char.myCharz().myskill.template.maxPoint > 0 && global::Char.myCharz().myskill.point == 0);
			flag2 = !flag3;
		}
		return flag2;
	}

	// Token: 0x06000482 RID: 1154 RVA: 0x00056B54 File Offset: 0x00054D54
	public void resetButton()
	{
		GameCanvas.menu.showMenu = false;
		ChatTextField.gI().close();
		ChatTextField.gI().center = null;
		this.isLockKey = false;
		this.typeTrade = 0;
		GameScr.indexMenu = 0;
		GameScr.indexSelect = 0;
		this.indexItemUse = -1;
		GameScr.indexRow = -1;
		GameScr.indexRowMax = 0;
		GameScr.indexTitle = 0;
		this.typeTrade = (this.typeTradeOrder = 0);
		mSystem.endKey();
		bool flag = global::Char.myCharz().cHP <= 0L || global::Char.myCharz().statusMe == 14 || global::Char.myCharz().statusMe == 5;
		if (flag)
		{
			bool meDead = global::Char.myCharz().meDead;
			if (meDead)
			{
				this.cmdDead = new Command(mResources.DIES[0], 11038);
				this.center = this.cmdDead;
				global::Char.myCharz().cHP = 0L;
			}
			GameScr.isHaveSelectSkill = false;
		}
		else
		{
			GameScr.isHaveSelectSkill = true;
		}
		GameScr.scrMain.clear();
	}

	// Token: 0x06000483 RID: 1155 RVA: 0x00004CE8 File Offset: 0x00002EE8
	public override void keyPress(int keyCode)
	{
		base.keyPress(keyCode);
	}

	// Token: 0x06000484 RID: 1156 RVA: 0x00056C58 File Offset: 0x00054E58
	public override void updateKey()
	{
		bool flag = Controller.isStopReadMessage || global::Char.myCharz().isTeleport || global::Char.myCharz().isPaintNewSkill || InfoDlg.isLock;
		if (!flag)
		{
			bool flag2 = GameCanvas.isTouch && !ChatTextField.gI().isShow && !GameCanvas.menu.showMenu;
			if (flag2)
			{
				this.updateKeyTouchControl();
			}
			this.checkAuto();
			GameCanvas.debug("F2", 0);
			bool flag3 = ChatPopup.currChatPopup != null;
			if (flag3)
			{
				Command cmdNextLine = ChatPopup.currChatPopup.cmdNextLine;
				bool flag4 = (GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] || mScreen.getCmdPointerLast(cmdNextLine)) && cmdNextLine != null;
				if (flag4)
				{
					GameCanvas.isPointerJustRelease = false;
					GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] = false;
					mScreen.keyTouch = -1;
					if (cmdNextLine != null)
					{
						cmdNextLine.performAction();
					}
				}
			}
			else
			{
				bool flag5 = !ChatTextField.gI().isShow;
				if (flag5)
				{
					bool flag6 = (GameCanvas.keyPressed[12] || mScreen.getCmdPointerLast(GameCanvas.currentScreen.left)) && this.left != null;
					if (flag6)
					{
						GameCanvas.isPointerJustRelease = false;
						GameCanvas.isPointerClick = false;
						GameCanvas.keyPressed[12] = false;
						mScreen.keyTouch = -1;
						bool flag7 = this.left != null;
						if (flag7)
						{
							this.left.performAction();
						}
					}
					bool flag8 = (GameCanvas.keyPressed[13] || mScreen.getCmdPointerLast(GameCanvas.currentScreen.right)) && this.right != null;
					if (flag8)
					{
						GameCanvas.isPointerJustRelease = false;
						GameCanvas.isPointerClick = false;
						GameCanvas.keyPressed[13] = false;
						mScreen.keyTouch = -1;
						bool flag9 = this.right != null;
						if (flag9)
						{
							this.right.performAction();
						}
					}
					bool flag10 = (GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] || mScreen.getCmdPointerLast(GameCanvas.currentScreen.center)) && this.center != null;
					if (flag10)
					{
						GameCanvas.isPointerJustRelease = false;
						GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] = false;
						mScreen.keyTouch = -1;
						bool flag11 = this.center != null;
						if (flag11)
						{
							this.center.performAction();
						}
					}
				}
				else
				{
					bool flag12 = ChatTextField.gI().left != null && (GameCanvas.keyPressed[12] || mScreen.getCmdPointerLast(ChatTextField.gI().left)) && ChatTextField.gI().left != null;
					if (flag12)
					{
						ChatTextField.gI().left.performAction();
					}
					bool flag13 = ChatTextField.gI().right != null && (GameCanvas.keyPressed[13] || mScreen.getCmdPointerLast(ChatTextField.gI().right)) && ChatTextField.gI().right != null;
					if (flag13)
					{
						ChatTextField.gI().right.performAction();
					}
					bool flag14 = ChatTextField.gI().center != null && (GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] || mScreen.getCmdPointerLast(ChatTextField.gI().center)) && ChatTextField.gI().center != null;
					if (flag14)
					{
						ChatTextField.gI().center.performAction();
					}
				}
			}
			GameCanvas.debug("F6", 0);
			this.updateKeyAlert();
			GameCanvas.debug("F7", 0);
			bool flag15 = global::Char.myCharz().currentMovePoint != null;
			if (flag15)
			{
				for (int i = 0; i < GameCanvas.keyPressed.Length; i++)
				{
					bool flag16 = GameCanvas.keyPressed[i];
					if (flag16)
					{
						global::Char.myCharz().currentMovePoint = null;
						break;
					}
				}
			}
			GameCanvas.debug("F8", 0);
			bool flag17 = ChatTextField.gI().isShow && GameCanvas.keyAsciiPress != 0;
			if (flag17)
			{
				ChatTextField.gI().keyPressed(GameCanvas.keyAsciiPress);
				GameCanvas.keyAsciiPress = 0;
			}
			else
			{
				bool flag18 = this.isLockKey;
				if (flag18)
				{
					GameCanvas.clearKeyHold();
					GameCanvas.clearKeyPressed();
				}
				else
				{
					bool flag19 = GameCanvas.menu.showMenu || this.isOpenUI() || global::Char.isLockKey;
					if (!flag19)
					{
						bool flag20 = GameCanvas.keyPressed[10];
						if (flag20)
						{
							GameCanvas.keyPressed[10] = false;
							this.doUseHP();
							GameCanvas.clearKeyPressed();
						}
						bool flag21 = GameCanvas.keyPressed[11] && this.mobCapcha == null;
						if (flag21)
						{
							bool flag22 = this.popUpYesNo != null;
							if (flag22)
							{
								this.popUpYesNo.cmdYes.performAction();
							}
							else
							{
								bool flag23 = GameScr.info2.info.info != null && GameScr.info2.info.info.charInfo != null;
								if (flag23)
								{
									GameCanvas.panel.setTypeMessage();
									GameCanvas.panel.show();
								}
							}
							GameCanvas.keyPressed[11] = false;
							GameCanvas.clearKeyPressed();
						}
						bool flag24 = GameCanvas.keyAsciiPress != 0 && TField.isQwerty && GameCanvas.keyAsciiPress == 32;
						if (flag24)
						{
							this.doUseHP();
							GameCanvas.keyAsciiPress = 0;
							GameCanvas.clearKeyPressed();
						}
						bool flag25 = GameCanvas.keyAsciiPress != 0 && this.mobCapcha == null && TField.isQwerty && GameCanvas.keyAsciiPress == 121;
						if (flag25)
						{
							bool flag26 = this.popUpYesNo != null;
							if (flag26)
							{
								this.popUpYesNo.cmdYes.performAction();
								GameCanvas.keyAsciiPress = 0;
								GameCanvas.clearKeyPressed();
							}
							else
							{
								bool flag27 = GameScr.info2.info.info != null && GameScr.info2.info.info.charInfo != null;
								if (flag27)
								{
									GameCanvas.panel.setTypeMessage();
									GameCanvas.panel.show();
									GameCanvas.keyAsciiPress = 0;
									GameCanvas.clearKeyPressed();
								}
							}
						}
						bool flag28 = GameCanvas.keyPressed[10] && this.mobCapcha == null;
						if (flag28)
						{
							GameCanvas.keyPressed[10] = false;
							GameScr.info2.doClick(10);
							GameCanvas.clearKeyPressed();
						}
						this.checkDrag();
						bool flag29 = !global::Char.myCharz().isFlyAndCharge;
						if (flag29)
						{
							this.checkClick();
						}
						bool flag30 = global::Char.myCharz().cmdMenu != null && global::Char.myCharz().cmdMenu.isPointerPressInside();
						if (flag30)
						{
							global::Char.myCharz().cmdMenu.performAction();
						}
						bool flag31 = global::Char.myCharz().skillPaint != null;
						if (!flag31)
						{
							bool flag32 = GameCanvas.keyAsciiPress != 0;
							if (flag32)
							{
								bool flag33 = this.mobCapcha == null;
								if (flag33)
								{
									bool isQwerty = TField.isQwerty;
									if (isQwerty)
									{
										bool flag34 = GameCanvas.keyPressed[1];
										if (flag34)
										{
											bool flag35 = GameScr.keySkill[0] != null;
											if (flag35)
											{
												this.doSelectSkill(GameScr.keySkill[0], true);
											}
										}
										else
										{
											bool flag36 = GameCanvas.keyPressed[2];
											if (flag36)
											{
												bool flag37 = GameScr.keySkill[1] != null;
												if (flag37)
												{
													this.doSelectSkill(GameScr.keySkill[1], true);
												}
											}
											else
											{
												bool flag38 = GameCanvas.keyPressed[3];
												if (flag38)
												{
													bool flag39 = GameScr.keySkill[2] != null;
													if (flag39)
													{
														this.doSelectSkill(GameScr.keySkill[2], true);
													}
												}
												else
												{
													bool flag40 = GameCanvas.keyPressed[4];
													if (flag40)
													{
														bool flag41 = GameScr.keySkill[3] != null;
														if (flag41)
														{
															this.doSelectSkill(GameScr.keySkill[3], true);
														}
													}
													else
													{
														bool flag42 = GameCanvas.keyPressed[5];
														if (flag42)
														{
															bool flag43 = GameScr.keySkill[4] != null;
															if (flag43)
															{
																this.doSelectSkill(GameScr.keySkill[4], true);
															}
														}
														else
														{
															bool flag44 = GameCanvas.keyPressed[6];
															if (flag44)
															{
																bool flag45 = GameScr.keySkill[5] != null;
																if (flag45)
																{
																	this.doSelectSkill(GameScr.keySkill[5], true);
																}
															}
															else
															{
																bool flag46 = GameCanvas.keyPressed[7];
																if (flag46)
																{
																	bool flag47 = GameScr.keySkill[6] != null;
																	if (flag47)
																	{
																		this.doSelectSkill(GameScr.keySkill[6], true);
																	}
																}
																else
																{
																	bool flag48 = GameCanvas.keyPressed[8];
																	if (flag48)
																	{
																		bool flag49 = GameScr.keySkill[7] != null;
																		if (flag49)
																		{
																			this.doSelectSkill(GameScr.keySkill[7], true);
																		}
																	}
																	else
																	{
																		bool flag50 = GameCanvas.keyPressed[9];
																		if (flag50)
																		{
																			bool flag51 = GameScr.keySkill[8] != null;
																			if (flag51)
																			{
																				this.doSelectSkill(GameScr.keySkill[8], true);
																			}
																		}
																		else
																		{
																			bool flag52 = GameCanvas.keyPressed[0];
																			if (flag52)
																			{
																				bool flag53 = GameScr.keySkill[9] != null;
																				if (flag53)
																				{
																					this.doSelectSkill(GameScr.keySkill[9], true);
																				}
																			}
																			else
																			{
																				bool flag54 = GameCanvas.keyAsciiPress == 105;
																				if (flag54)
																				{
																					MyVector myVector = new MyVector();
																					myVector.addElement(new Command("Mặc set 1", 999901));
																					myVector.addElement(new Command("Mặc set 2", 999902));
																					GameCanvas.menu.startAt(myVector, 3);
																				}
																				else
																				{
																					bool flag55 = GameCanvas.keyAsciiPress == 114;
																					if (flag55)
																					{
																						ChatTextField.gI().startChat(this, string.Empty);
																					}
																					else
																					{
																						bool flag56 = GameCanvas.keyAsciiPress == 120;
																						if (flag56)
																						{
																							XmapController.ShowXmapMenu();
																						}
																						else
																						{
																							bool flag57 = GameCanvas.keyAsciiPress == 98;
																							if (flag57)
																							{
																								bool flag58 = DataAccount.Type == 1;
																								if (flag58)
																								{
																									SocketOutPut.Send(string.Format("bom|{0}", DataAccount.Team));
																								}
																							}
																							else
																							{
																								bool flag59 = GameCanvas.keyAsciiPress == 103;
																								if (flag59)
																								{
																									bool flag60 = DataAccount.Type == 1;
																									if (flag60)
																									{
																										SocketOutPut.Send(string.Format("sp|{0}|{1}|{2}", TileMap.mapID, TileMap.zoneID, DataAccount.Team));
																									}
																								}
																								else
																								{
																									bool flag61 = GameCanvas.keyAsciiPress == 100;
																									if (flag61)
																									{
																										bool flag62 = DataAccount.Type == 1;
																										if (flag62)
																										{
																											SocketOutPut.Send("dokhu");
																										}
																									}
																									else
																									{
																										bool flag63 = GameCanvas.keyAsciiPress == 109;
																										if (flag63)
																										{
																											Service.gI().openUIZone();
																											GameCanvas.panel.setTypeZone();
																											GameCanvas.panel.show();
																										}
																										else
																										{
																											bool flag64 = GameCanvas.keyAsciiPress == 102;
																											if (flag64)
																											{
																												bool flag65 = DataAccount.Type == 1;
																												if (flag65)
																												{
																													SocketOutPut.Send(string.Format("hopthe|{0}", DataAccount.Team));
																												}
																											}
																											else
																											{
																												bool flag66 = GameCanvas.keyAsciiPress == 119;
																												if (flag66)
																												{
																													bool flag67 = DataAccount.Type == 1;
																													if (flag67)
																													{
																														SocketOutPut.Send(string.Format("bohuyet|{0}", DataAccount.Team));
																													}
																												}
																												else
																												{
																													bool flag68 = GameCanvas.keyAsciiPress == 113;
																													if (flag68)
																													{
																														bool flag69 = DataAccount.Type == 1;
																														if (flag69)
																														{
																															SocketOutPut.Send(string.Format("ttnl|{0}", DataAccount.Team));
																														}
																													}
																													else
																													{
																														bool flag70 = GameCanvas.keyAsciiPress == 106;
																														if (flag70)
																														{
																															bool flag71 = GameScr.getX(0) > 0 && GameScr.getY(0) > 0;
																															if (flag71)
																															{
																																GameScr.MoveMyChar(GameScr.getX(0), GameScr.getY(0));
																															}
																														}
																														else
																														{
																															bool flag72 = GameCanvas.keyAsciiPress == 107;
																															if (flag72)
																															{
																																bool flag73 = GameScr.getX(1) > 0 && GameScr.getY(1) > 0;
																																if (flag73)
																																{
																																	GameScr.MoveMyChar(GameScr.getX(1), GameScr.getY(1));
																																	Service.gI().getMapOffline();
																																	Service.gI().requestChangeMap();
																																}
																															}
																															else
																															{
																																bool flag74 = GameCanvas.keyAsciiPress == 108;
																																if (flag74)
																																{
																																	bool flag75 = GameScr.getX(2) > 0 && GameScr.getY(2) > 0;
																																	if (flag75)
																																	{
																																		GameScr.MoveMyChar(GameScr.getX(2), GameScr.getY(2));
																																	}
																																}
																															}
																														}
																													}
																												}
																											}
																										}
																									}
																								}
																							}
																						}
																					}
																				}
																			}
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
									else
									{
										bool flag76 = !GameCanvas.isMoveNumberPad;
										if (flag76)
										{
											ChatTextField.gI().startChat(GameCanvas.keyAsciiPress, this, string.Empty);
										}
										else
										{
											bool flag77 = GameCanvas.keyAsciiPress == 55;
											if (flag77)
											{
												bool flag78 = GameScr.keySkill[0] != null;
												if (flag78)
												{
													this.doSelectSkill(GameScr.keySkill[0], true);
												}
											}
											else
											{
												bool flag79 = GameCanvas.keyAsciiPress == 56;
												if (flag79)
												{
													bool flag80 = GameScr.keySkill[1] != null;
													if (flag80)
													{
														this.doSelectSkill(GameScr.keySkill[1], true);
													}
												}
												else
												{
													bool flag81 = GameCanvas.keyAsciiPress == 57;
													if (flag81)
													{
														bool flag82 = GameScr.keySkill[(!Main.isPC) ? 2 : 21] != null;
														if (flag82)
														{
															this.doSelectSkill(GameScr.keySkill[2], true);
														}
													}
													else
													{
														bool flag83 = GameCanvas.keyAsciiPress == 48;
														if (flag83)
														{
															ChatTextField.gI().startChat(this, string.Empty);
														}
													}
												}
											}
										}
									}
								}
								else
								{
									char[] array = this.keyInput.ToCharArray();
									MyVector myVector2 = new MyVector();
									for (int j = 0; j < array.Length; j++)
									{
										myVector2.addElement(array[j].ToString() + string.Empty);
									}
									myVector2.removeElementAt(0);
									string text = ((char)GameCanvas.keyAsciiPress).ToString() + string.Empty;
									bool flag84 = text.Equals(string.Empty) || text == null || text.Equals("\n");
									if (flag84)
									{
										text = "-";
									}
									myVector2.insertElementAt(text, myVector2.size());
									this.keyInput = string.Empty;
									for (int k = 0; k < myVector2.size(); k++)
									{
										this.keyInput += ((string)myVector2.elementAt(k)).ToUpper();
									}
									Service.gI().mobCapcha((char)GameCanvas.keyAsciiPress);
								}
								GameCanvas.keyAsciiPress = 0;
							}
							bool flag85 = global::Char.myCharz().statusMe == 1;
							if (flag85)
							{
								GameCanvas.debug("F10", 0);
								bool flag86 = !this.doSeleckSkillFlag;
								if (flag86)
								{
									bool flag87 = GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25];
									if (flag87)
									{
										GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] = false;
										this.doFire(false, false);
									}
									else
									{
										bool flag88 = GameCanvas.keyHold[(!Main.isPC) ? 2 : 21];
										if (flag88)
										{
											bool flag89 = !global::Char.myCharz().isLockMove;
											if (flag89)
											{
												this.setCharJump(0);
											}
										}
										else
										{
											bool flag90 = GameCanvas.keyHold[1] && this.mobCapcha == null;
											if (flag90)
											{
												bool flag91 = !Main.isPC;
												if (flag91)
												{
													global::Char.myCharz().cdir = -1;
													bool flag92 = !global::Char.myCharz().isLockMove;
													if (flag92)
													{
														this.setCharJump(-4);
													}
												}
											}
											else
											{
												bool flag93 = GameCanvas.keyHold[(!Main.isPC) ? 5 : 25] && this.mobCapcha == null;
												if (flag93)
												{
													bool flag94 = !Main.isPC;
													if (flag94)
													{
														global::Char.myCharz().cdir = 1;
														bool flag95 = !global::Char.myCharz().isLockMove;
														if (flag95)
														{
															this.setCharJump(4);
														}
													}
												}
												else
												{
													bool flag96 = GameCanvas.keyHold[(!Main.isPC) ? 4 : 23];
													if (flag96)
													{
														GameScr.isAutoPlay = false;
														global::Char.myCharz().isAttack = false;
														bool flag97 = global::Char.myCharz().cdir == 1;
														if (flag97)
														{
															global::Char.myCharz().cdir = -1;
														}
														else
														{
															bool flag98 = !global::Char.myCharz().isLockMove;
															if (flag98)
															{
																bool flag99 = global::Char.myCharz().cx - global::Char.myCharz().cxSend != 0;
																if (flag99)
																{
																	Service.gI().charMove();
																}
																global::Char.myCharz().statusMe = 2;
																global::Char.myCharz().cvx = -global::Char.myCharz().cspeed;
															}
														}
														global::Char.myCharz().holder = false;
													}
													else
													{
														bool flag100 = GameCanvas.keyHold[(!Main.isPC) ? 6 : 24];
														if (flag100)
														{
															GameScr.isAutoPlay = false;
															global::Char.myCharz().isAttack = false;
															bool flag101 = global::Char.myCharz().cdir == -1;
															if (flag101)
															{
																global::Char.myCharz().cdir = 1;
															}
															else
															{
																bool flag102 = !global::Char.myCharz().isLockMove;
																if (flag102)
																{
																	bool flag103 = global::Char.myCharz().cx - global::Char.myCharz().cxSend != 0;
																	if (flag103)
																	{
																		Service.gI().charMove();
																	}
																	global::Char.myCharz().statusMe = 2;
																	global::Char.myCharz().cvx = global::Char.myCharz().cspeed;
																}
															}
															global::Char.myCharz().holder = false;
														}
													}
												}
											}
										}
									}
								}
							}
							else
							{
								bool flag104 = global::Char.myCharz().statusMe == 2;
								if (flag104)
								{
									GameCanvas.debug("F11", 0);
									bool flag105 = GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25];
									if (flag105)
									{
										GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] = false;
										this.doFire(false, true);
									}
									else
									{
										bool flag106 = GameCanvas.keyHold[(!Main.isPC) ? 2 : 21];
										if (flag106)
										{
											bool flag107 = global::Char.myCharz().cx - global::Char.myCharz().cxSend != 0 || global::Char.myCharz().cy - global::Char.myCharz().cySend != 0;
											if (flag107)
											{
												Service.gI().charMove();
											}
											global::Char.myCharz().cvy = -10;
											global::Char.myCharz().statusMe = 3;
											global::Char.myCharz().cp1 = 0;
										}
										else
										{
											bool flag108 = GameCanvas.keyHold[1] && this.mobCapcha == null;
											if (flag108)
											{
												bool isPC = Main.isPC;
												if (isPC)
												{
													bool flag109 = global::Char.myCharz().cx - global::Char.myCharz().cxSend != 0 || global::Char.myCharz().cy - global::Char.myCharz().cySend != 0;
													if (flag109)
													{
														Service.gI().charMove();
													}
													global::Char.myCharz().cdir = -1;
													global::Char.myCharz().cvy = -10;
													global::Char.myCharz().cvx = -4;
													global::Char.myCharz().statusMe = 3;
													global::Char.myCharz().cp1 = 0;
												}
											}
											else
											{
												bool flag110 = GameCanvas.keyHold[3] && this.mobCapcha == null;
												if (flag110)
												{
													bool flag111 = !Main.isPC;
													if (flag111)
													{
														bool flag112 = global::Char.myCharz().cx - global::Char.myCharz().cxSend != 0 || global::Char.myCharz().cy - global::Char.myCharz().cySend != 0;
														if (flag112)
														{
															Service.gI().charMove();
														}
														global::Char.myCharz().cdir = 1;
														global::Char.myCharz().cvy = -10;
														global::Char.myCharz().cvx = 4;
														global::Char.myCharz().statusMe = 3;
														global::Char.myCharz().cp1 = 0;
													}
												}
												else
												{
													bool flag113 = GameCanvas.keyHold[(!Main.isPC) ? 4 : 23];
													if (flag113)
													{
														GameScr.isAutoPlay = false;
														bool flag114 = global::Char.myCharz().cdir == 1;
														if (flag114)
														{
															global::Char.myCharz().cdir = -1;
														}
														else
														{
															global::Char.myCharz().cvx = -global::Char.myCharz().cspeed + global::Char.myCharz().cBonusSpeed;
														}
													}
													else
													{
														bool flag115 = GameCanvas.keyHold[(!Main.isPC) ? 6 : 24];
														if (flag115)
														{
															GameScr.isAutoPlay = false;
															bool flag116 = global::Char.myCharz().cdir == -1;
															if (flag116)
															{
																global::Char.myCharz().cdir = 1;
															}
															else
															{
																global::Char.myCharz().cvx = global::Char.myCharz().cspeed + global::Char.myCharz().cBonusSpeed;
															}
														}
													}
												}
											}
										}
									}
								}
								else
								{
									bool flag117 = global::Char.myCharz().statusMe == 3;
									if (flag117)
									{
										GameScr.isAutoPlay = false;
										GameCanvas.debug("F12", 0);
										bool flag118 = GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25];
										if (flag118)
										{
											GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] = false;
											this.doFire(false, true);
										}
										bool flag119 = GameCanvas.keyHold[(!Main.isPC) ? 4 : 23] || (GameCanvas.keyHold[1] && this.mobCapcha == null);
										if (flag119)
										{
											bool flag120 = global::Char.myCharz().cdir == 1;
											if (flag120)
											{
												global::Char.myCharz().cdir = -1;
											}
											else
											{
												global::Char.myCharz().cvx = -global::Char.myCharz().cspeed;
											}
										}
										else
										{
											bool flag121 = GameCanvas.keyHold[(!Main.isPC) ? 6 : 24] || (GameCanvas.keyHold[3] && this.mobCapcha == null);
											if (flag121)
											{
												bool flag122 = global::Char.myCharz().cdir == -1;
												if (flag122)
												{
													global::Char.myCharz().cdir = 1;
												}
												else
												{
													global::Char.myCharz().cvx = global::Char.myCharz().cspeed;
												}
											}
										}
										bool flag123 = (GameCanvas.keyHold[(!Main.isPC) ? 2 : 21] || ((GameCanvas.keyHold[1] || GameCanvas.keyHold[3]) && this.mobCapcha == null)) && global::Char.myCharz().canFly && global::Char.myCharz().cMP > 0L && global::Char.myCharz().cp1 < 8 && global::Char.myCharz().cvy > -4;
										if (flag123)
										{
											global::Char.myCharz().cp1++;
											global::Char.myCharz().cvy = -7;
										}
									}
									else
									{
										bool flag124 = global::Char.myCharz().statusMe == 4;
										if (flag124)
										{
											GameCanvas.debug("F13", 0);
											bool flag125 = GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25];
											if (flag125)
											{
												GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] = false;
												this.doFire(false, true);
											}
											bool flag126 = GameCanvas.keyHold[(!Main.isPC) ? 2 : 21] && global::Char.myCharz().cMP > 0L && global::Char.myCharz().canFly;
											if (flag126)
											{
												GameScr.isAutoPlay = false;
												bool flag127 = (global::Char.myCharz().cx - global::Char.myCharz().cxSend != 0 || global::Char.myCharz().cy - global::Char.myCharz().cySend != 0) && (Res.abs(global::Char.myCharz().cx - global::Char.myCharz().cxSend) > 96 || Res.abs(global::Char.myCharz().cy - global::Char.myCharz().cySend) > 24);
												if (flag127)
												{
													Service.gI().charMove();
												}
												global::Char.myCharz().cvy = -10;
												global::Char.myCharz().statusMe = 3;
												global::Char.myCharz().cp1 = 0;
											}
											bool flag128 = GameCanvas.keyHold[(!Main.isPC) ? 4 : 23];
											if (flag128)
											{
												GameScr.isAutoPlay = false;
												bool flag129 = global::Char.myCharz().cdir == 1;
												if (flag129)
												{
													global::Char.myCharz().cdir = -1;
												}
												else
												{
													global::Char.myCharz().cp1++;
													global::Char.myCharz().cvx = -global::Char.myCharz().cspeed;
													bool flag130 = global::Char.myCharz().cp1 > 5 && global::Char.myCharz().cvy > 6;
													if (flag130)
													{
														global::Char.myCharz().statusMe = 10;
														global::Char.myCharz().cp1 = 0;
														global::Char.myCharz().cvy = 0;
													}
												}
											}
											else
											{
												bool flag131 = GameCanvas.keyHold[(!Main.isPC) ? 6 : 24];
												if (flag131)
												{
													GameScr.isAutoPlay = false;
													bool flag132 = global::Char.myCharz().cdir == -1;
													if (flag132)
													{
														global::Char.myCharz().cdir = 1;
													}
													else
													{
														global::Char.myCharz().cp1++;
														global::Char.myCharz().cvx = global::Char.myCharz().cspeed;
														bool flag133 = global::Char.myCharz().cp1 > 5 && global::Char.myCharz().cvy > 6;
														if (flag133)
														{
															global::Char.myCharz().statusMe = 10;
															global::Char.myCharz().cp1 = 0;
															global::Char.myCharz().cvy = 0;
														}
													}
												}
											}
										}
										else
										{
											bool flag134 = global::Char.myCharz().statusMe == 10;
											if (flag134)
											{
												GameCanvas.debug("F14", 0);
												bool flag135 = GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25];
												if (flag135)
												{
													GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] = false;
													this.doFire(false, true);
												}
												bool flag136 = global::Char.myCharz().canFly && global::Char.myCharz().cMP > 0L;
												if (flag136)
												{
													bool flag137 = GameCanvas.keyHold[(!Main.isPC) ? 2 : 21];
													if (flag137)
													{
														GameScr.isAutoPlay = false;
														bool flag138 = (global::Char.myCharz().cx - global::Char.myCharz().cxSend != 0 || global::Char.myCharz().cy - global::Char.myCharz().cySend != 0) && (Res.abs(global::Char.myCharz().cx - global::Char.myCharz().cxSend) > 96 || Res.abs(global::Char.myCharz().cy - global::Char.myCharz().cySend) > 24);
														if (flag138)
														{
															Service.gI().charMove();
														}
														global::Char.myCharz().cvy = -10;
														global::Char.myCharz().statusMe = 3;
														global::Char.myCharz().cp1 = 0;
													}
													else
													{
														bool flag139 = GameCanvas.keyHold[(!Main.isPC) ? 4 : 23];
														if (flag139)
														{
															GameScr.isAutoPlay = false;
															bool flag140 = global::Char.myCharz().cdir == 1;
															if (flag140)
															{
																global::Char.myCharz().cdir = -1;
															}
															else
															{
																global::Char.myCharz().cvx = -(global::Char.myCharz().cspeed + 1);
															}
														}
														else
														{
															bool flag141 = GameCanvas.keyHold[(!Main.isPC) ? 6 : 24];
															if (flag141)
															{
																bool flag142 = global::Char.myCharz().cdir == -1;
																if (flag142)
																{
																	global::Char.myCharz().cdir = 1;
																}
																else
																{
																	global::Char.myCharz().cvx = global::Char.myCharz().cspeed + 1;
																}
															}
														}
													}
												}
											}
											else
											{
												bool flag143 = global::Char.myCharz().statusMe == 7;
												if (flag143)
												{
													GameCanvas.debug("F15", 0);
													bool flag144 = GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25];
													if (flag144)
													{
														GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] = false;
													}
													bool flag145 = GameCanvas.keyHold[(!Main.isPC) ? 4 : 23];
													if (flag145)
													{
														GameScr.isAutoPlay = false;
														bool flag146 = global::Char.myCharz().cdir == 1;
														if (flag146)
														{
															global::Char.myCharz().cdir = -1;
														}
														else
														{
															global::Char.myCharz().cvx = -global::Char.myCharz().cspeed + 2;
														}
													}
													else
													{
														bool flag147 = GameCanvas.keyHold[(!Main.isPC) ? 6 : 24];
														if (flag147)
														{
															GameScr.isAutoPlay = false;
															bool flag148 = global::Char.myCharz().cdir == -1;
															if (flag148)
															{
																global::Char.myCharz().cdir = 1;
															}
															else
															{
																global::Char.myCharz().cvx = global::Char.myCharz().cspeed - 2;
															}
														}
													}
												}
											}
										}
									}
								}
							}
							GameCanvas.debug("F17", 0);
							bool flag149 = GameCanvas.keyPressed[(!Main.isPC) ? 8 : 22] && GameCanvas.keyAsciiPress != 56;
							if (flag149)
							{
								GameCanvas.keyPressed[(!Main.isPC) ? 8 : 22] = false;
								global::Char.myCharz().delayFall = 0;
							}
							bool flag150 = GameCanvas.keyPressed[10];
							if (flag150)
							{
								GameCanvas.keyPressed[10] = false;
								this.doUseHP();
							}
							GameCanvas.debug("F20", 0);
							GameCanvas.clearKeyPressed();
							GameCanvas.debug("F23", 0);
							this.doSeleckSkillFlag = false;
						}
					}
				}
			}
		}
	}

	// Token: 0x06000485 RID: 1157 RVA: 0x00050D3C File Offset: 0x0004EF3C
	public bool isVsMap()
	{
		return true;
	}

	// Token: 0x06000486 RID: 1158 RVA: 0x000588C0 File Offset: 0x00056AC0
	private void checkDrag()
	{
		bool flag = GameScr.isAnalog == 1 || GameScr.gamePad.disableCheckDrag();
		if (!flag)
		{
			global::Char.myCharz().cmtoChar = true;
			bool flag2 = GameScr.isUseTouch;
			if (!flag2)
			{
				bool isPointerJustDown = GameCanvas.isPointerJustDown;
				if (isPointerJustDown)
				{
					GameCanvas.isPointerJustDown = false;
					this.isPointerDowning = true;
					this.ptDownTime = 0;
					this.ptLastDownX = (this.ptFirstDownX = GameCanvas.px);
					this.ptLastDownY = (this.ptFirstDownY = GameCanvas.py);
				}
				bool flag3 = this.isPointerDowning;
				if (flag3)
				{
					int num = GameCanvas.px - this.ptLastDownX;
					int num2 = GameCanvas.py - this.ptLastDownY;
					bool flag4 = !this.isChangingCameraMode && (Res.abs(GameCanvas.px - this.ptFirstDownX) > 15 || Res.abs(GameCanvas.py - this.ptFirstDownY) > 15);
					if (flag4)
					{
						this.isChangingCameraMode = true;
					}
					this.ptLastDownX = GameCanvas.px;
					this.ptLastDownY = GameCanvas.py;
					this.ptDownTime++;
					bool flag5 = this.isChangingCameraMode;
					if (flag5)
					{
						global::Char.myCharz().cmtoChar = false;
						GameScr.cmx -= num;
						GameScr.cmy -= num2;
						bool flag6 = GameScr.cmx < 24;
						if (flag6)
						{
							int num3 = (24 - GameScr.cmx) / 3;
							bool flag7 = num3 != 0;
							if (flag7)
							{
								GameScr.cmx += num - num / num3;
							}
						}
						bool flag8 = GameScr.cmx < (this.isVsMap() ? 24 : 0);
						if (flag8)
						{
							GameScr.cmx = (this.isVsMap() ? 24 : 0);
						}
						bool flag9 = GameScr.cmx > GameScr.cmxLim;
						if (flag9)
						{
							int num4 = (GameScr.cmx - GameScr.cmxLim) / 3;
							bool flag10 = num4 != 0;
							if (flag10)
							{
								GameScr.cmx += num - num / num4;
							}
						}
						bool flag11 = GameScr.cmx > GameScr.cmxLim + ((!this.isVsMap()) ? 24 : 0);
						if (flag11)
						{
							GameScr.cmx = GameScr.cmxLim + ((!this.isVsMap()) ? 24 : 0);
						}
						bool flag12 = GameScr.cmy < 0;
						if (flag12)
						{
							int num5 = -GameScr.cmy / 3;
							bool flag13 = num5 != 0;
							if (flag13)
							{
								GameScr.cmy += num2 - num2 / num5;
							}
						}
						bool flag14 = GameScr.cmy < -((!this.isVsMap()) ? 24 : 0);
						if (flag14)
						{
							GameScr.cmy = -((!this.isVsMap()) ? 24 : 0);
						}
						bool flag15 = GameScr.cmy > GameScr.cmyLim;
						if (flag15)
						{
							GameScr.cmy = GameScr.cmyLim;
						}
						GameScr.cmtoX = GameScr.cmx;
						GameScr.cmtoY = GameScr.cmy;
					}
				}
				bool flag16 = this.isPointerDowning && GameCanvas.isPointerJustRelease;
				if (flag16)
				{
					this.isPointerDowning = false;
					this.isChangingCameraMode = false;
					bool flag17 = Res.abs(GameCanvas.px - this.ptFirstDownX) > 15 || Res.abs(GameCanvas.py - this.ptFirstDownY) > 15;
					if (flag17)
					{
						GameCanvas.isPointerJustRelease = false;
					}
				}
			}
		}
	}

	// Token: 0x06000487 RID: 1159 RVA: 0x00058C04 File Offset: 0x00056E04
	private void checkClick()
	{
		bool flag = this.isCharging();
		if (!flag)
		{
			bool flag2 = this.popUpYesNo != null && this.popUpYesNo.cmdYes != null && this.popUpYesNo.cmdYes.isPointerPressInside();
			if (flag2)
			{
				this.popUpYesNo.cmdYes.performAction();
			}
			else
			{
				bool flag3 = this.checkClickToCapcha();
				if (!flag3)
				{
					long num = mSystem.currentTimeMillis();
					bool flag4 = this.lastSingleClick != 0L;
					if (flag4)
					{
						this.lastSingleClick = 0L;
						GameCanvas.isPointerJustDown = false;
						bool flag5 = !this.disableSingleClick;
						if (flag5)
						{
							this.checkSingleClick();
							GameCanvas.isPointerJustRelease = false;
							this.isWaitingDoubleClick = true;
							this.timeStartDblClick = mSystem.currentTimeMillis();
						}
					}
					bool flag6 = this.isWaitingDoubleClick;
					if (flag6)
					{
						this.timeEndDblClick = mSystem.currentTimeMillis();
						bool flag7 = this.timeEndDblClick - this.timeStartDblClick < 300L && GameCanvas.isPointerJustRelease;
						if (flag7)
						{
							this.isWaitingDoubleClick = false;
							this.checkDoubleClick();
						}
					}
					bool isPointerJustRelease = GameCanvas.isPointerJustRelease;
					if (isPointerJustRelease)
					{
						this.disableSingleClick = this.checkSingleClickEarly();
						this.lastSingleClick = num;
						this.lastClickCMX = GameScr.cmx;
						this.lastClickCMY = GameScr.cmy;
						GameCanvas.isPointerJustRelease = false;
					}
				}
			}
		}
	}

	// Token: 0x06000488 RID: 1160 RVA: 0x00058D58 File Offset: 0x00056F58
	private IMapObject findClickToItem(int px, int py)
	{
		IMapObject mapObject = null;
		int num = 0;
		int num2 = 30;
		MyVector[] array = new MyVector[]
		{
			GameScr.vMob,
			GameScr.vNpc,
			GameScr.vItemMap,
			GameScr.vCharInMap
		};
		for (int i = 0; i < array.Length; i++)
		{
			for (int j = 0; j < array[i].size(); j++)
			{
				IMapObject mapObject2 = (IMapObject)array[i].elementAt(j);
				bool flag = mapObject2.isInvisible();
				if (!flag)
				{
					bool flag2 = mapObject2 is Mob;
					if (flag2)
					{
						Mob mob = (Mob)mapObject2;
						bool flag3 = mob.isMobMe && mob.Equals(global::Char.myCharz().mobMe);
						if (flag3)
						{
							goto IL_015F;
						}
					}
					int x = mapObject2.getX();
					int y = mapObject2.getY();
					int w = mapObject2.getW();
					int h = mapObject2.getH();
					bool flag4 = !this.inRectangle(px, py, x - w / 2 - num2, y - h - num2, w + num2 * 2, h + num2 * 2);
					if (!flag4)
					{
						bool flag5 = mapObject == null;
						if (flag5)
						{
							mapObject = mapObject2;
							num = Res.abs(px - x) + Res.abs(py - y);
							bool flag6 = i == 1;
							if (flag6)
							{
								return mapObject;
							}
						}
						else
						{
							int num3 = Res.abs(px - x) + Res.abs(py - y);
							bool flag7 = num3 < num;
							if (flag7)
							{
								mapObject = mapObject2;
								num = num3;
							}
						}
					}
				}
				IL_015F:;
			}
		}
		return mapObject;
	}

	// Token: 0x06000489 RID: 1161 RVA: 0x00058F00 File Offset: 0x00057100
	private Mob findClickToMOB(int px, int py)
	{
		int num = 30;
		Mob mob = null;
		int num2 = 0;
		for (int i = 0; i < GameScr.vMob.size(); i++)
		{
			Mob mob2 = (Mob)GameScr.vMob.elementAt(i);
			bool flag = mob2.isInvisible();
			if (!flag)
			{
				bool flag2 = mob2 != null;
				if (flag2)
				{
					Mob mob3 = mob2;
					bool flag3 = mob3.isMobMe && mob3.Equals(global::Char.myCharz().mobMe);
					if (flag3)
					{
						goto IL_0110;
					}
				}
				int x = mob2.getX();
				int y = mob2.getY();
				int w = mob2.getW();
				int h = mob2.getH();
				bool flag4 = !this.inRectangle(px, py, x - w / 2 - num, y - h - num, w + num * 2, h + num * 2);
				if (!flag4)
				{
					bool flag5 = mob == null;
					if (flag5)
					{
						mob = mob2;
						num2 = Res.abs(px - x) + Res.abs(py - y);
					}
					else
					{
						int num3 = Res.abs(px - x) + Res.abs(py - y);
						bool flag6 = num3 < num2;
						if (flag6)
						{
							mob = mob2;
							num2 = num3;
						}
					}
				}
			}
			IL_0110:;
		}
		return mob;
	}

	// Token: 0x0600048A RID: 1162 RVA: 0x00059040 File Offset: 0x00057240
	private bool inRectangle(int xClick, int yClick, int x, int y, int w, int h)
	{
		return xClick >= x && xClick <= x + w && yClick >= y && yClick <= y + h;
	}

	// Token: 0x0600048B RID: 1163 RVA: 0x00059070 File Offset: 0x00057270
	private bool checkSingleClickEarly()
	{
		int num = GameCanvas.px + GameScr.cmx;
		int num2 = GameCanvas.py + GameScr.cmy;
		global::Char.myCharz().cancelAttack();
		IMapObject mapObject = this.findClickToItem(num, num2);
		bool flag = mapObject != null;
		bool flag5;
		if (flag)
		{
			bool flag2 = global::Char.myCharz().isAttacPlayerStatus() && global::Char.myCharz().charFocus != null && !mapObject.Equals(global::Char.myCharz().charFocus) && !mapObject.Equals(global::Char.myCharz().charFocus.mobMe) && mapObject is global::Char;
			if (flag2)
			{
				global::Char @char = (global::Char)mapObject;
				bool flag3 = @char.cTypePk != 5 && !@char.isAttacPlayerStatus();
				if (flag3)
				{
					this.checkClickMoveTo(num, num2, 2);
					return false;
				}
			}
			bool flag4 = global::Char.myCharz().mobFocus == mapObject || global::Char.myCharz().itemFocus == mapObject;
			if (flag4)
			{
				this.doDoubleClickToObj(mapObject);
				flag5 = true;
			}
			else
			{
				bool flag6 = TileMap.mapID == 51 && mapObject.Equals(global::Char.myCharz().npcFocus);
				if (flag6)
				{
					this.checkClickMoveTo(num, num2, 3);
					flag5 = false;
				}
				else
				{
					bool flag7 = global::Char.myCharz().skillPaint != null || global::Char.myCharz().arr != null || global::Char.myCharz().dart != null || global::Char.myCharz().skillInfoPaint() != null;
					if (flag7)
					{
						flag5 = false;
					}
					else
					{
						global::Char.myCharz().focusManualTo(mapObject);
						mapObject.stopMoving();
						flag5 = false;
					}
				}
			}
		}
		else
		{
			flag5 = false;
		}
		return flag5;
	}

	// Token: 0x0600048C RID: 1164 RVA: 0x00059208 File Offset: 0x00057408
	private void checkDoubleClick()
	{
		int num = GameCanvas.px + this.lastClickCMX;
		int num2 = GameCanvas.py + this.lastClickCMY;
		int cy = global::Char.myCharz().cy;
		bool flag = this.isLockKey;
		if (!flag)
		{
			IMapObject mapObject = this.findClickToItem(num, num2);
			bool flag2 = mapObject != null;
			if (flag2)
			{
				bool flag3 = mapObject is Mob && !this.isMeCanAttackMob((Mob)mapObject);
				if (flag3)
				{
					this.checkClickMoveTo(num, num2, 4);
				}
				else
				{
					bool flag4 = this.checkClickToBotton(mapObject) || (!mapObject.Equals(global::Char.myCharz().npcFocus) && this.mobCapcha != null);
					if (!flag4)
					{
						bool flag5 = global::Char.myCharz().isAttacPlayerStatus() && global::Char.myCharz().charFocus != null && !mapObject.Equals(global::Char.myCharz().charFocus) && !mapObject.Equals(global::Char.myCharz().charFocus.mobMe) && mapObject is global::Char;
						if (flag5)
						{
							global::Char @char = (global::Char)mapObject;
							bool flag6 = @char.cTypePk != 5 && !@char.isAttacPlayerStatus();
							if (flag6)
							{
								this.checkClickMoveTo(num, num2, 5);
								return;
							}
						}
						bool flag7 = TileMap.mapID == 51 && mapObject.Equals(global::Char.myCharz().npcFocus);
						if (flag7)
						{
							this.checkClickMoveTo(num, num2, 6);
						}
						else
						{
							this.doDoubleClickToObj(mapObject);
						}
					}
				}
			}
			else
			{
				bool flag8 = !this.checkClickToPopup(num, num2) && !this.checkClipTopChatPopUp(num, num2) && !Main.isPC;
				if (flag8)
				{
					this.checkClickMoveTo(num, num2, 7);
				}
			}
		}
	}

	// Token: 0x0600048D RID: 1165 RVA: 0x000593C4 File Offset: 0x000575C4
	private bool checkClickToBotton(IMapObject Object)
	{
		bool flag = Object == null;
		bool flag2;
		if (flag)
		{
			flag2 = false;
		}
		else
		{
			int i = Object.getY();
			int num = global::Char.myCharz().cy;
			bool flag3 = i < num;
			if (flag3)
			{
				while (i < num)
				{
					num -= 5;
					bool flag4 = TileMap.tileTypeAt(global::Char.myCharz().cx, num, 8192);
					if (flag4)
					{
						this.auto = 0;
						global::Char.myCharz().cancelAttack();
						global::Char.myCharz().currentMovePoint = null;
						return true;
					}
				}
			}
			flag2 = false;
		}
		return flag2;
	}

	// Token: 0x0600048E RID: 1166 RVA: 0x00059454 File Offset: 0x00057654
	private void doDoubleClickToObj(IMapObject obj)
	{
		bool flag = (obj.Equals(global::Char.myCharz().npcFocus) || this.mobCapcha == null) && !this.checkClickToBotton(obj);
		if (flag)
		{
			this.checkEffToObj(obj, false);
			global::Char.myCharz().cancelAttack();
			global::Char.myCharz().currentMovePoint = null;
			global::Char.myCharz().cvx = (global::Char.myCharz().cvy = 0);
			obj.stopMoving();
			this.auto = 10;
			this.doFire(false, true);
			this.clickToX = obj.getX();
			this.clickToY = obj.getY();
			this.clickOnTileTop = false;
			this.clickMoving = true;
			this.clickMovingRed = true;
			this.clickMovingTimeOut = 20;
			this.clickMovingP1 = 30;
		}
	}

	// Token: 0x0600048F RID: 1167 RVA: 0x00059520 File Offset: 0x00057720
	private void checkSingleClick()
	{
		int num = GameCanvas.px + this.lastClickCMX;
		int num2 = GameCanvas.py + this.lastClickCMY;
		bool flag = !this.isLockKey && !this.checkClickToPopup(num, num2) && !this.checkClipTopChatPopUp(num, num2);
		if (flag)
		{
			this.checkClickMoveTo(num, num2, 0);
		}
	}

	// Token: 0x06000490 RID: 1168 RVA: 0x00059578 File Offset: 0x00057778
	private bool checkClipTopChatPopUp(int xClick, int yClick)
	{
		bool flag = this.Equals(GameScr.info2) && GameScr.gI().popUpYesNo != null;
		bool flag2;
		if (flag)
		{
			flag2 = false;
		}
		else
		{
			bool flag3 = GameScr.info2.info.info != null && GameScr.info2.info.info.charInfo != null;
			if (flag3)
			{
				int num = Res.abs(GameScr.info2.cmx) + GameScr.info2.info.X - 40;
				int num2 = Res.abs(GameScr.info2.cmy) + GameScr.info2.info.Y;
				bool flag4 = this.inRectangle(xClick - GameScr.cmx, yClick - GameScr.cmy, num, num2, 200, GameScr.info2.info.H);
				if (flag4)
				{
					GameScr.info2.doClick(10);
					return true;
				}
			}
			flag2 = false;
		}
		return flag2;
	}

	// Token: 0x06000491 RID: 1169 RVA: 0x00059674 File Offset: 0x00057874
	private bool checkClickToPopup(int xClick, int yClick)
	{
		for (int i = 0; i < PopUp.vPopups.size(); i++)
		{
			PopUp popUp = (PopUp)PopUp.vPopups.elementAt(i);
			bool flag = this.inRectangle(xClick, yClick, popUp.cx, popUp.cy, popUp.cw, popUp.ch);
			if (flag)
			{
				bool flag2 = popUp.cy <= 24 && TileMap.isInAirMap() && global::Char.myCharz().cTypePk != 0;
				bool flag3;
				if (flag2)
				{
					flag3 = false;
				}
				else
				{
					bool flag4 = popUp.isPaint;
					if (!flag4)
					{
						goto IL_0086;
					}
					popUp.doClick(10);
					flag3 = true;
				}
				return flag3;
			}
			IL_0086:;
		}
		return false;
	}

	// Token: 0x06000492 RID: 1170 RVA: 0x0005972C File Offset: 0x0005792C
	private void checkClickMoveTo(int xClick, int yClick, int index)
	{
		bool flag = GameScr.gamePad.disableClickMove();
		if (!flag)
		{
			global::Char.myCharz().cancelAttack();
			bool flag2 = xClick < TileMap.pxw && xClick > TileMap.pxw - 32;
			if (flag2)
			{
				global::Char.myCharz().currentMovePoint = new MovePoint(TileMap.pxw, yClick);
			}
			else
			{
				bool flag3 = xClick < 32 && xClick > 0;
				if (flag3)
				{
					global::Char.myCharz().currentMovePoint = new MovePoint(0, yClick);
				}
				else
				{
					bool flag4 = xClick < TileMap.pxw && xClick > TileMap.pxw - 48;
					if (flag4)
					{
						global::Char.myCharz().currentMovePoint = new MovePoint(TileMap.pxw, yClick);
					}
					else
					{
						bool flag5 = xClick < 48 && xClick > 0;
						if (flag5)
						{
							global::Char.myCharz().currentMovePoint = new MovePoint(0, yClick);
						}
						else
						{
							this.clickToX = xClick;
							this.clickToY = yClick;
							this.clickOnTileTop = false;
							global::Char.myCharz().delayFall = 0;
							int num = ((!global::Char.myCharz().canFly || global::Char.myCharz().cMP <= 0L) ? 1000 : 0);
							bool flag6 = this.clickToY > global::Char.myCharz().cy && Res.abs(this.clickToX - global::Char.myCharz().cx) < 12;
							if (!flag6)
							{
								int num2 = 0;
								while (num2 < 60 + num && this.clickToY + num2 < TileMap.pxh - 24)
								{
									bool flag7 = TileMap.tileTypeAt(this.clickToX, this.clickToY + num2, 2);
									if (flag7)
									{
										this.clickToY = TileMap.tileYofPixel(this.clickToY + num2);
										this.clickOnTileTop = true;
										break;
									}
									num2 += 24;
								}
								for (int i = 0; i < 40 + num; i += 24)
								{
									bool flag8 = TileMap.tileTypeAt(this.clickToX, this.clickToY - i, 2);
									if (flag8)
									{
										this.clickToY = TileMap.tileYofPixel(this.clickToY - i);
										this.clickOnTileTop = true;
										break;
									}
								}
								this.clickMoving = true;
								this.clickMovingRed = false;
								this.clickMovingP1 = ((!this.clickOnTileTop) ? 30 : ((yClick >= this.clickToY) ? this.clickToY : yClick));
								global::Char.myCharz().delayFall = 0;
								bool flag9 = !this.clickOnTileTop && this.clickToY < global::Char.myCharz().cy - 50;
								if (flag9)
								{
									global::Char.myCharz().delayFall = 20;
								}
								this.clickMovingTimeOut = 30;
								this.auto = 0;
								bool holder = global::Char.myCharz().holder;
								if (holder)
								{
									global::Char.myCharz().removeHoleEff();
								}
								global::Char.myCharz().currentMovePoint = new MovePoint(this.clickToX, this.clickToY);
								global::Char.myCharz().cdir = ((global::Char.myCharz().cx - global::Char.myCharz().currentMovePoint.xEnd <= 0) ? 1 : (-1));
								global::Char.myCharz().endMovePointCommand = null;
								GameScr.isAutoPlay = false;
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06000493 RID: 1171 RVA: 0x00059A48 File Offset: 0x00057C48
	private void checkAuto()
	{
		long num = mSystem.currentTimeMillis();
		bool flag = GameCanvas.keyPressed[(!Main.isPC) ? 2 : 21] || GameCanvas.keyPressed[(!Main.isPC) ? 4 : 23] || GameCanvas.keyPressed[(!Main.isPC) ? 6 : 24] || GameCanvas.keyPressed[1] || GameCanvas.keyPressed[3];
		if (flag)
		{
			this.auto = 0;
			GameScr.isAutoPlay = false;
		}
		bool flag2 = GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] && !this.isPaintPopup();
		if (flag2)
		{
			bool flag3 = this.auto == 0;
			if (flag3)
			{
				bool flag4 = num - this.lastFire < 800L && this.checkSkillValid2() && (global::Char.myCharz().mobFocus != null || (global::Char.myCharz().charFocus != null && global::Char.myCharz().isMeCanAttackOtherPlayer(global::Char.myCharz().charFocus)));
				if (flag4)
				{
					Res.outz("toi day");
					this.auto = 10;
					GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] = false;
				}
			}
			else
			{
				this.auto = 0;
				GameCanvas.keyPressed[(!Main.isPC) ? 4 : 23] = (GameCanvas.keyPressed[(!Main.isPC) ? 6 : 24] = false);
			}
			this.lastFire = num;
		}
		bool flag5 = GameCanvas.gameTick % 5 == 0 && this.auto > 0 && global::Char.myCharz().currentMovePoint == null;
		if (flag5)
		{
			bool flag6 = global::Char.myCharz().myskill != null && (global::Char.myCharz().myskill.template.isUseAlone() || global::Char.myCharz().myskill.paintCanNotUseSkill);
			if (flag6)
			{
				return;
			}
			bool flag7 = (global::Char.myCharz().mobFocus != null && global::Char.myCharz().mobFocus.status != 1 && global::Char.myCharz().mobFocus.status != 0 && global::Char.myCharz().charFocus == null) || (global::Char.myCharz().charFocus != null && global::Char.myCharz().isMeCanAttackOtherPlayer(global::Char.myCharz().charFocus));
			if (flag7)
			{
				bool paintCanNotUseSkill = global::Char.myCharz().myskill.paintCanNotUseSkill;
				if (paintCanNotUseSkill)
				{
					return;
				}
				this.doFire(false, true);
			}
		}
		bool flag8 = this.auto > 1;
		if (flag8)
		{
			this.auto--;
		}
	}

	// Token: 0x06000494 RID: 1172 RVA: 0x00059CC8 File Offset: 0x00057EC8
	public void doUseHP()
	{
		bool flag = global::Char.myCharz().stone || global::Char.myCharz().blindEff || global::Char.myCharz().holdEffID > 0;
		if (!flag)
		{
			long num = mSystem.currentTimeMillis();
			bool flag2 = num - this.lastUsePotion >= 10000L;
			if (flag2)
			{
				bool flag3 = !global::Char.myCharz().doUsePotion();
				if (flag3)
				{
					GameScr.info1.addInfo(mResources.HP_EMPTY, 0);
				}
				else
				{
					ServerEffect.addServerEffect(11, global::Char.myCharz(), 5);
					ServerEffect.addServerEffect(104, global::Char.myCharz(), 4);
					this.lastUsePotion = num;
					SoundMn.gI().eatPeans();
				}
			}
		}
	}

	// Token: 0x06000495 RID: 1173 RVA: 0x00059D78 File Offset: 0x00057F78
	public void activeSuperPower(int x, int y)
	{
		bool flag = !this.isSuperPower;
		if (flag)
		{
			SoundMn.gI().bigeExlode();
			this.isSuperPower = true;
			this.tPower = 0;
			this.dxPower = 0;
			this.xPower = x - GameScr.cmx;
			this.yPower = y - GameScr.cmy;
		}
	}

	// Token: 0x06000496 RID: 1174 RVA: 0x00059DD0 File Offset: 0x00057FD0
	public void activeRongThanEff(bool isMe)
	{
		this.activeRongThan = true;
		this.isUseFreez = true;
		this.isMeCallRongThan = true;
		if (isMe)
		{
			Effect effect = new Effect(20, global::Char.myCharz().cx, global::Char.myCharz().cy - 77, 2, 8, 1);
			EffecMn.addEff(effect);
		}
	}

	// Token: 0x06000497 RID: 1175 RVA: 0x00004CF3 File Offset: 0x00002EF3
	public void hideRongThanEff()
	{
		this.activeRongThan = false;
		this.isUseFreez = true;
		this.isMeCallRongThan = false;
	}

	// Token: 0x06000498 RID: 1176 RVA: 0x00004D0B File Offset: 0x00002F0B
	public void doiMauTroi()
	{
		this.isRongThanXuatHien = true;
		this.mautroi = mGraphics.blendColor(0.4f, 0, GameCanvas.colorTop[GameCanvas.colorTop.Length - 1]);
	}

	// Token: 0x06000499 RID: 1177 RVA: 0x00059E24 File Offset: 0x00058024
	public void callRongThan(int x, int y)
	{
		Res.outz("VE RONG THAN O VI TRI x= " + x.ToString() + " y=" + y.ToString());
		this.doiMauTroi();
		Effect effect = new Effect((!this.isRongNamek) ? 17 : 25, x, y - 77, 2, -1, 1);
		EffecMn.addEff(effect);
	}

	// Token: 0x0600049A RID: 1178 RVA: 0x00059E80 File Offset: 0x00058080
	public void hideRongThan()
	{
		this.isRongThanXuatHien = false;
		EffecMn.removeEff(17);
		bool flag = this.isRongNamek;
		if (flag)
		{
			this.isRongNamek = false;
			EffecMn.removeEff(25);
		}
	}

	// Token: 0x0600049B RID: 1179 RVA: 0x00059EB8 File Offset: 0x000580B8
	private void autoPlay()
	{
	}

	// Token: 0x0600049C RID: 1180 RVA: 0x00059EC8 File Offset: 0x000580C8
	private void doFire(bool isFireByShortCut, bool skipWaypoint)
	{
		GameScr.tam++;
		Waypoint waypoint = global::Char.myCharz().isInEnterOfflinePoint();
		Waypoint waypoint2 = global::Char.myCharz().isInEnterOnlinePoint();
		bool flag = !skipWaypoint && waypoint != null && (global::Char.myCharz().mobFocus == null || (global::Char.myCharz().mobFocus != null && global::Char.myCharz().mobFocus.templateId == 0));
		if (flag)
		{
			waypoint.popup.command.performAction();
		}
		else
		{
			bool flag2 = !skipWaypoint && waypoint2 != null && (global::Char.myCharz().mobFocus == null || (global::Char.myCharz().mobFocus != null && global::Char.myCharz().mobFocus.templateId == 0));
			if (flag2)
			{
				waypoint2.popup.command.performAction();
			}
			else
			{
				bool flag3 = (TileMap.mapID == 51 && global::Char.myCharz().npcFocus != null) || global::Char.myCharz().statusMe == 14;
				if (!flag3)
				{
					global::Char.myCharz().cvx = (global::Char.myCharz().cvy = 0);
					bool flag4 = global::Char.myCharz().isSelectingSkillUseAlone() && global::Char.myCharz().focusToAttack();
					if (flag4)
					{
						bool flag5 = this.checkSkillValid();
						if (flag5)
						{
							global::Char.myCharz().currentFireByShortcut = isFireByShortCut;
							global::Char.myCharz().useSkillNotFocus();
						}
					}
					else
					{
						bool flag6 = this.isAttack();
						if (flag6)
						{
							bool flag7 = global::Char.myCharz().isUseChargeSkill() && global::Char.myCharz().focusToAttack();
							if (flag7)
							{
								bool flag8 = this.checkSkillValid();
								if (flag8)
								{
									global::Char.myCharz().currentFireByShortcut = isFireByShortCut;
									global::Char.myCharz().sendUseChargeSkill();
								}
								else
								{
									global::Char.myCharz().stopUseChargeSkill();
								}
							}
							else
							{
								bool flag9 = TileMap.tileTypeAt(global::Char.myCharz().cx, global::Char.myCharz().cy, 2);
								global::Char.myCharz().setSkillPaint(GameScr.sks[(int)global::Char.myCharz().myskill.skillId], (!flag9) ? 1 : 0);
								bool flag10 = flag9;
								if (flag10)
								{
									global::Char.myCharz().delayFall = 20;
								}
								global::Char.myCharz().currentFireByShortcut = isFireByShortCut;
							}
						}
					}
					bool flag11 = global::Char.myCharz().isSelectingSkillBuffToPlayer();
					if (flag11)
					{
						this.auto = 0;
					}
				}
			}
		}
	}

	// Token: 0x0600049D RID: 1181 RVA: 0x0005A124 File Offset: 0x00058324
	private void askToPick()
	{
		Npc npc = new Npc(5, 0, -100, 100, 5, GameScr.info1.charId[global::Char.myCharz().cgender][2]);
		string nhatvatpham = mResources.nhatvatpham;
		string[] array = new string[]
		{
			mResources.YES,
			mResources.NO
		};
		npc.idItem = 673;
		GameScr.gI().createMenu(array, npc);
		ChatPopup.addChatPopupWithIcon(nhatvatpham, 100000, npc, 5820);
	}

	// Token: 0x0600049E RID: 1182 RVA: 0x0005A1A0 File Offset: 0x000583A0
	private void pickItem()
	{
		bool flag = global::Char.myCharz().itemFocus == null;
		if (!flag)
		{
			bool flag2 = global::Char.myCharz().cx < global::Char.myCharz().itemFocus.x;
			if (flag2)
			{
				global::Char.myCharz().cdir = 1;
			}
			else
			{
				global::Char.myCharz().cdir = -1;
			}
			int num = global::Math.abs(global::Char.myCharz().cx - global::Char.myCharz().itemFocus.x);
			int num2 = global::Math.abs(global::Char.myCharz().cy - global::Char.myCharz().itemFocus.y);
			bool flag3 = num <= 40 && num2 < 40;
			if (flag3)
			{
				GameCanvas.clearKeyHold();
				GameCanvas.clearKeyPressed();
				bool flag4 = global::Char.myCharz().itemFocus.template.id != 673;
				if (flag4)
				{
					Service.gI().pickItem(global::Char.myCharz().itemFocus.itemMapID);
				}
				else
				{
					this.askToPick();
				}
			}
			else
			{
				global::Char.myCharz().currentMovePoint = new MovePoint(global::Char.myCharz().itemFocus.x, global::Char.myCharz().itemFocus.y);
				global::Char.myCharz().endMovePointCommand = new Command(null, null, 8002, null);
				GameCanvas.clearKeyHold();
				GameCanvas.clearKeyPressed();
			}
		}
	}

	// Token: 0x0600049F RID: 1183 RVA: 0x0005A300 File Offset: 0x00058500
	public bool isCharging()
	{
		return global::Char.myCharz().isFlyAndCharge || global::Char.myCharz().isUseSkillAfterCharge || global::Char.myCharz().isStandAndCharge || global::Char.myCharz().isWaitMonkey || this.isSuperPower || global::Char.myCharz().isFreez;
	}

	// Token: 0x060004A0 RID: 1184 RVA: 0x0005A364 File Offset: 0x00058564
	public void doSelectSkill(Skill skill, bool isShortcut)
	{
		bool flag = global::Char.myCharz().isCreateDark || this.isCharging() || global::Char.myCharz().taskMaint.taskId <= 1;
		if (!flag)
		{
			global::Char.myCharz().myskill = skill;
			bool flag2 = this.lastSkill != skill && this.lastSkill != null;
			if (flag2)
			{
				Service.gI().selectSkill((int)skill.template.id);
				this.saveRMSCurrentSkill(skill.template.id);
				this.resetButton();
				this.lastSkill = skill;
				this.selectedIndexSkill = -1;
				GameScr.gI().auto = 0;
			}
			else
			{
				bool flag3 = global::Char.myCharz().isUseSkillSpec();
				if (flag3)
				{
					Res.outz(">>>use skill spec: " + skill.template.id.ToString());
					global::Char.myCharz().sendNewAttack((short)skill.template.id);
					this.saveRMSCurrentSkill(skill.template.id);
					this.resetButton();
					this.lastSkill = skill;
					this.selectedIndexSkill = -1;
					GameScr.gI().auto = 0;
				}
				else
				{
					bool flag4 = global::Char.myCharz().isSelectingSkillUseAlone();
					if (flag4)
					{
						Res.outz("use skill not focus");
						this.doUseSkillNotFocus(skill);
						this.lastSkill = skill;
					}
					else
					{
						this.selectedIndexSkill = -1;
						bool flag5 = skill == null;
						if (!flag5)
						{
							Res.outz("only select skill");
							bool flag6 = this.lastSkill != skill;
							if (flag6)
							{
								Service.gI().selectSkill((int)skill.template.id);
								this.saveRMSCurrentSkill(skill.template.id);
								this.resetButton();
							}
							bool flag7 = global::Char.myCharz().charFocus != null || !global::Char.myCharz().isSelectingSkillBuffToPlayer();
							if (flag7)
							{
								bool flag8 = global::Char.myCharz().focusToAttack();
								if (flag8)
								{
									this.doFire(isShortcut, true);
									this.doSeleckSkillFlag = true;
								}
								this.lastSkill = skill;
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x060004A1 RID: 1185 RVA: 0x0005A578 File Offset: 0x00058778
	public void doUseSkill(Skill skill, bool isShortcut)
	{
		bool flag = (TileMap.mapID == 112 || TileMap.mapID == 113) && global::Char.myCharz().cTypePk == 0;
		if (!flag)
		{
			bool flag2 = global::Char.myCharz().isSelectingSkillUseAlone();
			if (flag2)
			{
				Res.outz("HERE");
				this.doUseSkillNotFocus(skill);
			}
			else
			{
				this.selectedIndexSkill = -1;
				bool flag3 = skill != null;
				if (flag3)
				{
					Service.gI().selectSkill((int)skill.template.id);
					this.saveRMSCurrentSkill(skill.template.id);
					this.resetButton();
					global::Char.myCharz().myskill = skill;
					this.doFire(isShortcut, true);
				}
			}
		}
	}

	// Token: 0x060004A2 RID: 1186 RVA: 0x0005A628 File Offset: 0x00058828
	public void doUseSkillNotFocus(Skill skill)
	{
		bool flag = ((TileMap.mapID != 112 && TileMap.mapID != 113) || global::Char.myCharz().cTypePk != 0) && this.checkSkillValid();
		if (flag)
		{
			this.selectedIndexSkill = -1;
			bool flag2 = skill != null;
			if (flag2)
			{
				Service.gI().selectSkill((int)skill.template.id);
				this.saveRMSCurrentSkill(skill.template.id);
				this.resetButton();
				global::Char.myCharz().myskill = skill;
				global::Char.myCharz().useSkillNotFocus();
				global::Char.myCharz().currentFireByShortcut = true;
				this.auto = 0;
			}
		}
	}

	// Token: 0x060004A3 RID: 1187 RVA: 0x0005A6CC File Offset: 0x000588CC
	public void sortSkill()
	{
		for (int i = 0; i < global::Char.myCharz().vSkillFight.size() - 1; i++)
		{
			Skill skill = (Skill)global::Char.myCharz().vSkillFight.elementAt(i);
			for (int j = i + 1; j < global::Char.myCharz().vSkillFight.size(); j++)
			{
				Skill skill2 = (Skill)global::Char.myCharz().vSkillFight.elementAt(j);
				bool flag = skill2.template.id < skill.template.id;
				if (flag)
				{
					Skill skill3 = skill2;
					skill2 = skill;
					skill = skill3;
					global::Char.myCharz().vSkillFight.setElementAt(skill, i);
					global::Char.myCharz().vSkillFight.setElementAt(skill2, j);
				}
			}
		}
	}

	// Token: 0x060004A4 RID: 1188 RVA: 0x0005A7A0 File Offset: 0x000589A0
	public void updateKeyTouchCapcha()
	{
		bool flag = this.isNotPaintTouchControl();
		if (!flag)
		{
			for (int i = 0; i < this.strCapcha.Length; i++)
			{
				this.keyCapcha[i] = -1;
				bool flag2 = !GameCanvas.isTouchControl;
				if (!flag2)
				{
					int num = (GameCanvas.w - this.strCapcha.Length * GameScr.disXC) / 2;
					int num2 = this.strCapcha.Length * GameScr.disXC;
					int num3 = GameCanvas.h - 40;
					int num4 = GameScr.disXC;
					bool flag3 = !GameCanvas.isPointerHoldIn(num, num3, num2, num4);
					if (!flag3)
					{
						int num5 = (GameCanvas.px - num) / GameScr.disXC;
						bool flag4 = i == num5;
						if (flag4)
						{
							this.keyCapcha[i] = 1;
						}
						bool flag5 = GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease && i == num5;
						if (flag5)
						{
							char[] array = this.keyInput.ToCharArray();
							MyVector myVector = new MyVector();
							for (int j = 0; j < array.Length; j++)
							{
								myVector.addElement(array[j].ToString() + string.Empty);
							}
							myVector.removeElementAt(0);
							myVector.insertElementAt(this.strCapcha[i].ToString() + string.Empty, myVector.size());
							this.keyInput = string.Empty;
							for (int k = 0; k < myVector.size(); k++)
							{
								this.keyInput += ((string)myVector.elementAt(k)).ToUpper();
							}
							Service.gI().mobCapcha(this.strCapcha[i]);
						}
					}
				}
			}
		}
	}

	// Token: 0x060004A5 RID: 1189 RVA: 0x0005A988 File Offset: 0x00058B88
	public bool checkClickToCapcha()
	{
		bool flag = this.mobCapcha == null;
		bool flag2;
		if (flag)
		{
			flag2 = false;
		}
		else
		{
			int num = (GameCanvas.w - 5 * GameScr.disXC) / 2;
			int num2 = 5 * GameScr.disXC;
			int num3 = GameCanvas.h - 40;
			int num4 = GameScr.disXC;
			bool flag3 = GameCanvas.isPointerHoldIn(num, num3, num2, num4);
			flag2 = flag3;
		}
		return flag2;
	}

	// Token: 0x060004A6 RID: 1190 RVA: 0x0005A9F0 File Offset: 0x00058BF0
	public void checkMouseChat()
	{
		bool flag = GameCanvas.isMouseFocus(GameScr.xC, GameScr.yC, 34, 34);
		if (flag)
		{
			bool flag2 = !TileMap.isOfflineMap();
			if (flag2)
			{
				mScreen.keyMouse = 15;
			}
		}
		else
		{
			bool flag3 = GameCanvas.isMouseFocus(GameScr.xHP, GameScr.yHP, 40, 40);
			if (flag3)
			{
				bool flag4 = global::Char.myCharz().statusMe != 14;
				if (flag4)
				{
					mScreen.keyMouse = 10;
				}
			}
			else
			{
				bool flag5 = GameCanvas.isMouseFocus(GameScr.xF, GameScr.yF, 40, 40);
				if (flag5)
				{
					bool flag6 = global::Char.myCharz().statusMe != 14;
					if (flag6)
					{
						mScreen.keyMouse = 5;
					}
				}
				else
				{
					bool flag7 = this.cmdMenu != null && GameCanvas.isMouseFocus(this.cmdMenu.x, this.cmdMenu.y, this.cmdMenu.w / 2, this.cmdMenu.h);
					if (flag7)
					{
						mScreen.keyMouse = 1;
					}
					else
					{
						mScreen.keyMouse = -1;
					}
				}
			}
		}
	}

	// Token: 0x060004A7 RID: 1191 RVA: 0x0005AB00 File Offset: 0x00058D00
	private void updateKeyTouchControl()
	{
		bool flag = this.isNotPaintTouchControl();
		if (!flag)
		{
			mScreen.keyTouch = -1;
			bool isTouchControl = GameCanvas.isTouchControl;
			if (isTouchControl)
			{
				bool flag2 = GameCanvas.isPointerHoldIn(0, 0, 60, 50) && GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease;
				if (flag2)
				{
					bool flag3 = global::Char.myCharz().cmdMenu != null;
					if (flag3)
					{
						global::Char.myCharz().cmdMenu.performAction();
					}
					global::Char.myCharz().currentMovePoint = null;
					GameCanvas.clearAllPointerEvent();
					this.flareFindFocus = true;
					this.flareTime = 5;
					return;
				}
				bool isPC = Main.isPC;
				if (isPC)
				{
					this.checkMouseChat();
				}
				bool flag4 = !TileMap.isOfflineMap() && GameCanvas.isPointerHoldIn(GameScr.xC, GameScr.yC, 34, 34);
				if (flag4)
				{
					mScreen.keyTouch = 15;
					GameCanvas.isPointerJustDown = false;
					this.isPointerDowning = false;
					bool flag5 = GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease;
					if (flag5)
					{
						ChatTextField.gI().startChat(this, string.Empty);
						SoundMn.gI().buttonClick();
						global::Char.myCharz().currentMovePoint = null;
						GameCanvas.clearAllPointerEvent();
						return;
					}
				}
				bool flag6 = global::Char.myCharz().cmdMenu != null && GameCanvas.isPointerHoldIn(global::Char.myCharz().cmdMenu.x - 17, global::Char.myCharz().cmdMenu.y - 17, 34, 34);
				if (flag6)
				{
					mScreen.keyTouch = 20;
					GameCanvas.isPointerJustDown = false;
					this.isPointerDowning = false;
					bool flag7 = GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease;
					if (flag7)
					{
						GameCanvas.clearAllPointerEvent();
						global::Char.myCharz().cmdMenu.performAction();
						return;
					}
				}
				this.updateGamePad();
				bool flag8 = ((GameScr.isAnalog != 0) ? GameCanvas.isPointerHoldIn(GameScr.xHP, GameScr.yHP + 10, 34, 34) : GameCanvas.isPointerHoldIn(GameScr.xHP, GameScr.yHP + 10, 40, 40)) && global::Char.myCharz().statusMe != 14 && this.mobCapcha == null;
				if (flag8)
				{
					mScreen.keyTouch = 10;
					GameCanvas.isPointerJustDown = false;
					this.isPointerDowning = false;
					bool flag9 = GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease;
					if (flag9)
					{
						GameCanvas.keyPressed[10] = true;
						GameCanvas.isPointerClick = (GameCanvas.isPointerJustDown = (GameCanvas.isPointerJustRelease = false));
					}
				}
				bool flag10 = ((GameScr.isAnalog != 0) ? GameCanvas.isPointerHoldIn(GameScr.xHP + 5, GameScr.yHP - 6 - 34 + 10, 34, 34) : GameCanvas.isPointerHoldIn(GameScr.xHP + 5, GameScr.yHP - 6 - 40 + 10, 40, 40)) && global::Char.myCharz().statusMe != 14 && this.mobCapcha == null;
				if (flag10)
				{
					bool flag11 = GameScr.isPickNgocRong;
					if (flag11)
					{
						mScreen.keyTouch = 14;
						GameCanvas.isPointerJustDown = false;
						this.isPointerDowning = false;
						bool flag12 = GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease;
						if (flag12)
						{
							GameCanvas.keyPressed[14] = true;
							GameCanvas.isPointerClick = (GameCanvas.isPointerJustDown = (GameCanvas.isPointerJustRelease = false));
							GameScr.isPickNgocRong = false;
							Service.gI().useItem(-1, -1, -1, -1);
						}
					}
					else
					{
						bool flag13 = GameScr.isudungCapsun4;
						if (flag13)
						{
							mScreen.keyTouch = 14;
							GameCanvas.isPointerJustDown = false;
							this.isPointerDowning = false;
							bool flag14 = GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease;
							if (flag14)
							{
								GameCanvas.keyPressed[14] = true;
								GameCanvas.isPointerClick = (GameCanvas.isPointerJustDown = (GameCanvas.isPointerJustRelease = false));
								for (int i = 0; i < global::Char.myCharz().arrItemBag.Length; i++)
								{
									Item item = global::Char.myCharz().arrItemBag[i];
									bool flag15 = item == null;
									if (!flag15)
									{
										Res.err("find " + item.template.id.ToString());
										bool flag16 = item.template.id == 194;
										if (flag16)
										{
											GameScr.isudungCapsun4 = item.quantity > 0;
											bool flag17 = GameScr.isudungCapsun4;
											if (flag17)
											{
												Service.gI().useItem(0, 1, (sbyte)i, -1);
												break;
											}
										}
									}
								}
							}
						}
						else
						{
							bool flag18 = GameScr.isudungCapsun3;
							if (flag18)
							{
								mScreen.keyTouch = 14;
								GameCanvas.isPointerJustDown = false;
								this.isPointerDowning = false;
								bool flag19 = GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease;
								if (flag19)
								{
									GameCanvas.keyPressed[14] = true;
									GameCanvas.isPointerClick = (GameCanvas.isPointerJustDown = (GameCanvas.isPointerJustRelease = false));
									for (int j = 0; j < global::Char.myCharz().arrItemBag.Length; j++)
									{
										Item item2 = global::Char.myCharz().arrItemBag[j];
										bool flag20 = item2 != null && item2.template.id == 193;
										if (flag20)
										{
											GameScr.isudungCapsun3 = item2.quantity > 0;
											bool flag21 = GameScr.isudungCapsun3;
											if (flag21)
											{
												Service.gI().useItem(0, 1, (sbyte)j, -1);
												break;
											}
										}
									}
								}
							}
						}
					}
				}
			}
			bool flag22 = this.mobCapcha != null;
			if (flag22)
			{
				this.updateKeyTouchCapcha();
			}
			else
			{
				bool flag23 = GameScr.isHaveSelectSkill;
				if (flag23)
				{
					bool flag24 = this.isCharging();
					if (flag24)
					{
						return;
					}
					this.keyTouchSkill = -1;
					bool flag25 = false;
					bool flag26 = GameScr.onScreenSkill.Length > 5 && (GameCanvas.isPointerHoldIn(GameScr.xSkill + GameScr.xS[0] - GameScr.wSkill / 2 + 12, GameScr.yS[0] - GameScr.wSkill / 2 + 12, 5 * GameScr.wSkill, GameScr.wSkill) || GameCanvas.isPointerHoldIn(GameScr.xSkill + GameScr.xS[5] - GameScr.wSkill / 2 + 12, GameScr.yS[5] - GameScr.wSkill / 2 + 12, 5 * GameScr.wSkill, GameScr.wSkill));
					if (flag26)
					{
						flag25 = true;
					}
					bool flag27 = flag25 || GameCanvas.isPointerHoldIn(GameScr.xSkill + GameScr.xS[0] - GameScr.wSkill / 2 + 12, GameScr.yS[0] - GameScr.wSkill / 2 + 12, 5 * GameScr.wSkill, GameScr.wSkill) || (!GameCanvas.isTouchControl && GameCanvas.isPointerHoldIn(GameScr.xSkill + GameScr.xS[0] - GameScr.wSkill / 2 + 12, GameScr.yS[0] - GameScr.wSkill / 2 + 12, GameScr.wSkill, GameScr.onScreenSkill.Length * GameScr.wSkill));
					if (flag27)
					{
						GameCanvas.isPointerJustDown = false;
						this.isPointerDowning = false;
						int num = (GameCanvas.pxLast - (GameScr.xSkill + GameScr.xS[0] - GameScr.wSkill / 2 + 12)) / GameScr.wSkill;
						bool flag28 = flag25 && GameCanvas.pyLast < GameScr.yS[0];
						if (flag28)
						{
							num += 5;
						}
						this.keyTouchSkill = num;
						bool flag29 = GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease;
						if (flag29)
						{
							GameCanvas.isPointerClick = (GameCanvas.isPointerJustDown = (GameCanvas.isPointerJustRelease = false));
							this.selectedIndexSkill = num;
							bool flag30 = GameScr.indexSelect < 0;
							if (flag30)
							{
								GameScr.indexSelect = 0;
							}
							bool flag31 = !Main.isPC;
							if (flag31)
							{
								bool flag32 = this.selectedIndexSkill > GameScr.onScreenSkill.Length - 1;
								if (flag32)
								{
									this.selectedIndexSkill = GameScr.onScreenSkill.Length - 1;
								}
							}
							else
							{
								bool flag33 = this.selectedIndexSkill > GameScr.keySkill.Length - 1;
								if (flag33)
								{
									this.selectedIndexSkill = GameScr.keySkill.Length - 1;
								}
							}
							Skill skill = (Main.isPC ? GameScr.keySkill[this.selectedIndexSkill] : GameScr.onScreenSkill[this.selectedIndexSkill]);
							bool flag34 = skill != null;
							if (flag34)
							{
								this.doSelectSkill(skill, true);
							}
						}
					}
				}
			}
			bool isPointerJustRelease = GameCanvas.isPointerJustRelease;
			if (isPointerJustRelease)
			{
				bool flag35 = GameCanvas.keyHold[1] || GameCanvas.keyHold[(!Main.isPC) ? 2 : 21] || GameCanvas.keyHold[3] || GameCanvas.keyHold[(!Main.isPC) ? 4 : 23] || GameCanvas.keyHold[(!Main.isPC) ? 6 : 24];
				if (flag35)
				{
					GameCanvas.isPointerJustRelease = false;
				}
				GameCanvas.keyHold[1] = false;
				GameCanvas.keyHold[(!Main.isPC) ? 2 : 21] = false;
				GameCanvas.keyHold[3] = false;
				GameCanvas.keyHold[(!Main.isPC) ? 4 : 23] = false;
				GameCanvas.keyHold[(!Main.isPC) ? 6 : 24] = false;
			}
		}
	}

	// Token: 0x060004A8 RID: 1192 RVA: 0x00004D35 File Offset: 0x00002F35
	public void setCharJumpAtt()
	{
		global::Char.myCharz().cvy = -10;
		global::Char.myCharz().statusMe = 3;
		global::Char.myCharz().cp1 = 0;
	}

	// Token: 0x060004A9 RID: 1193 RVA: 0x0005B394 File Offset: 0x00059594
	public void setCharJump(int cvx)
	{
		bool flag = global::Char.myCharz().cx - global::Char.myCharz().cxSend != 0 || global::Char.myCharz().cy - global::Char.myCharz().cySend != 0;
		if (flag)
		{
			Service.gI().charMove();
		}
		global::Char.myCharz().cvy = -10;
		global::Char.myCharz().cvx = cvx;
		global::Char.myCharz().statusMe = 3;
		global::Char.myCharz().cp1 = 0;
	}

	// Token: 0x060004AA RID: 1194 RVA: 0x0005B414 File Offset: 0x00059614
	public void updateOpen()
	{
		bool flag = this.isstarOpen;
		if (flag)
		{
			bool flag2 = this.moveUp > -3;
			if (flag2)
			{
				this.moveUp -= 4;
			}
			else
			{
				this.moveUp = -2;
			}
			bool flag3 = this.moveDow < GameCanvas.h + 3;
			if (flag3)
			{
				this.moveDow += 4;
			}
			else
			{
				this.moveDow = GameCanvas.h + 2;
			}
			bool flag4 = this.moveUp <= -2 && this.moveDow >= GameCanvas.h + 2;
			if (flag4)
			{
				this.isstarOpen = false;
			}
		}
	}

	// Token: 0x060004AB RID: 1195 RVA: 0x00003E4C File Offset: 0x0000204C
	public void initCreateCommand()
	{
	}

	// Token: 0x060004AC RID: 1196 RVA: 0x00003E4C File Offset: 0x0000204C
	public void checkCharFocus()
	{
	}

	// Token: 0x060004AD RID: 1197 RVA: 0x0005B4BC File Offset: 0x000596BC
	public void updateXoSo()
	{
		bool flag = this.tShow == 0;
		if (!flag)
		{
			GameScr.currXS = mSystem.currentTimeMillis();
			bool flag2 = GameScr.currXS - GameScr.lastXS > 1000L;
			if (flag2)
			{
				GameScr.lastXS = mSystem.currentTimeMillis();
				GameScr.secondXS++;
			}
			bool flag3 = GameScr.secondXS > 20;
			if (flag3)
			{
				for (int i = 0; i < this.winnumber.Length; i++)
				{
					this.randomNumber[i] = this.winnumber[i];
				}
				this.tShow--;
				bool flag4 = this.tShow == 0;
				if (flag4)
				{
					this.yourNumber = string.Empty;
					GameScr.info1.addInfo(this.strFinish, 0);
					GameScr.secondXS = 0;
				}
			}
			else
			{
				bool flag5 = this.moveIndex > this.winnumber.Length - 1;
				if (flag5)
				{
					this.tShow--;
					bool flag6 = this.tShow == 0;
					if (flag6)
					{
						this.yourNumber = string.Empty;
						GameScr.info1.addInfo(this.strFinish, 0);
					}
				}
				else
				{
					bool flag7 = this.moveIndex < this.randomNumber.Length;
					if (flag7)
					{
						bool flag8 = this.tMove[this.moveIndex] == 15;
						if (flag8)
						{
							bool flag9 = this.randomNumber[this.moveIndex] == this.winnumber[this.moveIndex] - 1;
							if (flag9)
							{
								this.delayMove[this.moveIndex] = 10;
							}
							bool flag10 = this.randomNumber[this.moveIndex] == this.winnumber[this.moveIndex];
							if (flag10)
							{
								this.tMove[this.moveIndex] = -1;
								this.moveIndex++;
							}
						}
						else
						{
							bool flag11 = GameCanvas.gameTick % 5 == 0;
							if (flag11)
							{
								this.tMove[this.moveIndex]++;
							}
						}
					}
					for (int j = 0; j < this.winnumber.Length; j++)
					{
						bool flag12 = this.tMove[j] == -1;
						if (!flag12)
						{
							this.moveCount[j]++;
							bool flag13 = this.moveCount[j] > this.tMove[j] + this.delayMove[j];
							if (flag13)
							{
								this.moveCount[j] = 0;
								this.randomNumber[j]++;
								bool flag14 = this.randomNumber[j] >= 10;
								if (flag14)
								{
									this.randomNumber[j] = 0;
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x060004AE RID: 1198 RVA: 0x00004D5A File Offset: 0x00002F5A
	public void MyDoDoubleClickToObj(IMapObject obj)
	{
		this.doDoubleClickToObj(obj);
	}

	// Token: 0x060004AF RID: 1199 RVA: 0x0005B774 File Offset: 0x00059974
	public static int getX(sbyte type)
	{
		int i = 0;
		while (i < TileMap.vGo.size())
		{
			Waypoint waypoint = (Waypoint)TileMap.vGo.elementAt(i);
			bool flag = waypoint.maxX < 60 && type == 0;
			int num;
			if (flag)
			{
				num = 15;
			}
			else
			{
				bool flag2 = (int)waypoint.minX <= TileMap.pxw - 60 && waypoint.maxX >= 60 && type == 1;
				if (flag2)
				{
					num = (int)((waypoint.minX + waypoint.maxX) / 2);
				}
				else
				{
					bool flag3 = (int)waypoint.minX > TileMap.pxw - 60 && type == 2;
					if (!flag3)
					{
						i++;
						continue;
					}
					num = TileMap.pxw - 15;
				}
			}
			return num;
		}
		return 0;
	}

	// Token: 0x060004B0 RID: 1200 RVA: 0x0005B83C File Offset: 0x00059A3C
	public static int getY(sbyte type)
	{
		int i = 0;
		while (i < TileMap.vGo.size())
		{
			Waypoint waypoint = (Waypoint)TileMap.vGo.elementAt(i);
			bool flag = waypoint.maxX < 60 && type == 0;
			int num;
			if (flag)
			{
				num = (int)waypoint.maxY;
			}
			else
			{
				bool flag2 = (int)waypoint.minX <= TileMap.pxw - 60 && waypoint.maxX >= 60 && type == 1;
				if (flag2)
				{
					num = (int)waypoint.maxY;
				}
				else
				{
					bool flag3 = (int)waypoint.minX > TileMap.pxw - 60 && type == 2;
					if (!flag3)
					{
						i++;
						continue;
					}
					num = (int)waypoint.maxY;
				}
			}
			return num;
		}
		return 0;
	}

	// Token: 0x060004B1 RID: 1201 RVA: 0x0000A9D4 File Offset: 0x00008BD4
	private static void MoveMyChar(int x, int y)
	{
		global::Char.myCharz().cx = x;
		global::Char.myCharz().cy = y;
		Service.gI().charMove();
		bool flag = ItemTime.isExistItem(4387);
		if (!flag)
		{
			global::Char.myCharz().cx = x;
			global::Char.myCharz().cy = y + 1;
			Service.gI().charMove();
			global::Char.myCharz().cx = x;
			global::Char.myCharz().cy = y;
			Service.gI().charMove();
		}
	}

	// Token: 0x060004B2 RID: 1202 RVA: 0x0005B8FC File Offset: 0x00059AFC
	public override void update()
	{
		bool flag = !Pk9rXmap.IsXmapRunning;
		if (flag)
		{
			Status.Update();
			AutoBroly.Update();
			ChucNangPhu.Update();
			ChucNangPhu2.Update();
			ChucNangPhu3.Update();
			ChucNangPhu4.Update();
		}
		bool flag2 = GameCanvas.gameTick % 20 == 0;
		if (flag2)
		{
			SocketInPut.Start();
		}
		bool flag3 = DataAccount.Type > 1;
		if (flag3)
		{
			KsSupper.Update();
			bool flag4 = DataAccount.Type == 3;
			if (flag4)
			{
				DovaBaoKhu.Update();
			}
		}
		bool flag5 = GameCanvas.keyPressed[16];
		if (flag5)
		{
			GameCanvas.keyPressed[16] = false;
			global::Char.myCharz().findNextFocusByKey();
		}
		bool flag6 = GameCanvas.keyPressed[13] && !GameCanvas.panel.isShow;
		if (flag6)
		{
			GameCanvas.keyPressed[13] = false;
			global::Char.myCharz().findNextFocusByKey();
		}
		bool flag7 = GameCanvas.keyPressed[17];
		if (flag7)
		{
			GameCanvas.keyPressed[17] = false;
			global::Char.myCharz().searchItem();
			bool flag8 = global::Char.myCharz().itemFocus != null;
			if (flag8)
			{
				this.pickItem();
			}
		}
		bool flag9 = GameCanvas.gameTick % 100 == 0 && TileMap.mapID == 137;
		if (flag9)
		{
			GameScr.shock_scr = 30;
		}
		bool flag10 = GameScr.isAutoPlay && GameCanvas.gameTick % 20 == 0;
		if (flag10)
		{
			this.autoPlay();
		}
		this.updateXoSo();
		mSystem.checkAdComlete();
		SmallImage.update();
		try
		{
			bool isContinueToLogin = LoginScr.isContinueToLogin;
			if (isContinueToLogin)
			{
				LoginScr.isContinueToLogin = false;
			}
			bool flag11 = GameScr.tickMove == 1;
			if (flag11)
			{
				GameScr.lastTick = mSystem.currentTimeMillis();
			}
			bool flag12 = GameScr.tickMove == 100;
			if (flag12)
			{
				GameScr.tickMove = 0;
				GameScr.currTick = mSystem.currentTimeMillis();
				int num = (int)(GameScr.currTick - GameScr.lastTick) / 1000;
				Service.gI().checkMMove(num);
			}
			bool flag13 = GameScr.lockTick > 0;
			if (flag13)
			{
				GameScr.lockTick--;
				bool flag14 = GameScr.lockTick == 0;
				if (flag14)
				{
					Controller.isStopReadMessage = false;
				}
			}
			this.checkCharFocus();
			GameCanvas.debug("E1", 0);
			GameScr.updateCamera();
			GameCanvas.debug("E2", 0);
			ChatTextField.gI().update();
			GameCanvas.debug("E3", 0);
			for (int i = 0; i < GameScr.vCharInMap.size(); i++)
			{
				((global::Char)GameScr.vCharInMap.elementAt(i)).update();
			}
			for (int j = 0; j < Teleport.vTeleport.size(); j++)
			{
				((Teleport)Teleport.vTeleport.elementAt(j)).update();
			}
			global::Char.myCharz().update();
			bool flag15 = global::Char.myCharz().statusMe == 1;
			if (flag15)
			{
			}
			bool flag16 = this.popUpYesNo != null;
			if (flag16)
			{
				this.popUpYesNo.update();
			}
			EffecMn.update();
			GameCanvas.debug("E5x", 0);
			for (int k = 0; k < GameScr.vMob.size(); k++)
			{
				((Mob)GameScr.vMob.elementAt(k)).update();
			}
			GameCanvas.debug("E6", 0);
			for (int l = 0; l < GameScr.vNpc.size(); l++)
			{
				((Npc)GameScr.vNpc.elementAt(l)).update();
			}
			this.nSkill = GameScr.onScreenSkill.Length;
			for (int m = GameScr.onScreenSkill.Length - 1; m >= 0; m--)
			{
				Skill skill = GameScr.onScreenSkill[m];
				bool flag17 = skill != null;
				if (flag17)
				{
					this.nSkill = m + 1;
					break;
				}
				this.nSkill--;
			}
			GameScr.setSkillBarPosition();
			GameCanvas.debug("E7", 0);
			GameCanvas.gI().updateDust();
			GameCanvas.debug("E8", 0);
			GameScr.updateFlyText();
			PopUp.updateAll();
			GameScr.updateSplash();
			this.updateSS();
			GameCanvas.updateBG();
			GameCanvas.debug("E9", 0);
			this.updateClickToArrow();
			GameCanvas.debug("E10", 0);
			for (int n = 0; n < GameScr.vItemMap.size(); n++)
			{
				((ItemMap)GameScr.vItemMap.elementAt(n)).update();
			}
			GameCanvas.debug("E11", 0);
			GameCanvas.debug("E13", 0);
			for (int num2 = Effect2.vRemoveEffect2.size() - 1; num2 >= 0; num2--)
			{
				Effect2.vEffect2.removeElement(Effect2.vRemoveEffect2.elementAt(num2));
				Effect2.vRemoveEffect2.removeElementAt(num2);
			}
			for (int num3 = 0; num3 < Effect2.vEffect2.size(); num3++)
			{
				Effect2 effect = (Effect2)Effect2.vEffect2.elementAt(num3);
				effect.update();
			}
			for (int num4 = 0; num4 < Effect2.vEffect2Outside.size(); num4++)
			{
				Effect2 effect2 = (Effect2)Effect2.vEffect2Outside.elementAt(num4);
				effect2.update();
			}
			for (int num5 = 0; num5 < Effect2.vAnimateEffect.size(); num5++)
			{
				Effect2 effect3 = (Effect2)Effect2.vAnimateEffect.elementAt(num5);
				effect3.update();
			}
			for (int num6 = 0; num6 < Effect2.vEffectFeet.size(); num6++)
			{
				Effect2 effect4 = (Effect2)Effect2.vEffectFeet.elementAt(num6);
				effect4.update();
			}
			for (int num7 = 0; num7 < Effect2.vEffect3.size(); num7++)
			{
				Effect2 effect5 = (Effect2)Effect2.vEffect3.elementAt(num7);
				effect5.update();
			}
			BackgroudEffect.updateEff();
			GameScr.info1.update();
			GameScr.info2.update();
			GameCanvas.debug("E15", 0);
			bool flag18 = GameScr.currentCharViewInfo != null && !GameScr.currentCharViewInfo.Equals(global::Char.myCharz());
			if (flag18)
			{
				GameScr.currentCharViewInfo.update();
			}
			this.runArrow++;
			bool flag19 = this.runArrow > 3;
			if (flag19)
			{
				this.runArrow = 0;
			}
			bool flag20 = this.isInjureHp;
			if (flag20)
			{
				this.twHp += 1L;
				bool flag21 = this.twHp == 20L;
				if (flag21)
				{
					this.twHp = 0L;
					this.isInjureHp = false;
				}
			}
			else
			{
				bool flag22 = this.dHP > global::Char.myCharz().cHP;
				if (flag22)
				{
					long num8 = this.dHP - global::Char.myCharz().cHP >> 1;
					bool flag23 = num8 < 1L;
					if (flag23)
					{
						num8 = 1L;
					}
					this.dHP -= num8;
				}
				else
				{
					this.dHP = global::Char.myCharz().cHP;
				}
			}
			bool flag24 = this.isInjureMp;
			if (flag24)
			{
				this.twMp += 1L;
				bool flag25 = this.twMp == 20L;
				if (flag25)
				{
					this.twMp = 0L;
					this.isInjureMp = false;
				}
			}
			else
			{
				bool flag26 = this.dMP > global::Char.myCharz().cMP;
				if (flag26)
				{
					long num9 = this.dMP - global::Char.myCharz().cMP >> 1;
					bool flag27 = num9 < 1L;
					if (flag27)
					{
						num9 = 1L;
					}
					this.dMP -= num9;
				}
				else
				{
					this.dMP = global::Char.myCharz().cMP;
				}
			}
			bool flag28 = this.tMenuDelay > 0;
			if (flag28)
			{
				this.tMenuDelay--;
			}
			bool flag29 = this.isRongThanMenu();
			if (flag29)
			{
				int num10 = 100;
				while (this.yR - num10 < GameScr.cmy)
				{
					GameScr.cmy--;
				}
			}
			for (int num11 = 0; num11 < global::Char.vItemTime.size(); num11++)
			{
				((ItemTime)global::Char.vItemTime.elementAt(num11)).update();
			}
			for (int num12 = 0; num12 < GameScr.textTime.size(); num12++)
			{
				((ItemTime)GameScr.textTime.elementAt(num12)).update();
			}
			this.updateChatVip();
		}
		catch (Exception)
		{
		}
		int num13 = GameCanvas.gameTick % 4000;
		bool flag30 = num13 == 1000;
		if (flag30)
		{
			GameScr.checkRemoveImage();
		}
		EffectManager.update();
	}

	// Token: 0x060004B3 RID: 1203 RVA: 0x00003E4C File Offset: 0x0000204C
	public void updateKeyChatPopUp()
	{
	}

	// Token: 0x060004B4 RID: 1204 RVA: 0x0005C200 File Offset: 0x0005A400
	public bool isRongThanMenu()
	{
		return this.isMeCallRongThan;
	}

	// Token: 0x060004B5 RID: 1205 RVA: 0x0005C224 File Offset: 0x0005A424
	public void paintEffect(mGraphics g)
	{
		for (int i = 0; i < Effect2.vEffect2.size(); i++)
		{
			Effect2 effect = (Effect2)Effect2.vEffect2.elementAt(i);
			bool flag = effect != null && !(effect is ChatPopup);
			if (flag)
			{
				effect.paint(g);
			}
		}
		bool flag2 = !GameCanvas.lowGraphic;
		if (flag2)
		{
			for (int j = 0; j < Effect2.vAnimateEffect.size(); j++)
			{
				Effect2 effect2 = (Effect2)Effect2.vAnimateEffect.elementAt(j);
				effect2.paint(g);
			}
		}
		for (int k = 0; k < Effect2.vEffect2Outside.size(); k++)
		{
			Effect2 effect3 = (Effect2)Effect2.vEffect2Outside.elementAt(k);
			effect3.paint(g);
		}
	}

	// Token: 0x060004B6 RID: 1206 RVA: 0x0005C308 File Offset: 0x0005A508
	public void paintBgItem(mGraphics g, int layer)
	{
		for (int i = 0; i < TileMap.vCurrItem.size(); i++)
		{
			BgItem bgItem = (BgItem)TileMap.vCurrItem.elementAt(i);
			bool flag = bgItem.idImage != -1 && (int)bgItem.layer == layer;
			if (flag)
			{
				bgItem.paint(g);
			}
		}
		bool flag2 = TileMap.mapID == 48 && layer == 3 && GameCanvas.bgW != null && GameCanvas.bgW[0] != 0;
		if (flag2)
		{
			for (int j = 0; j < TileMap.pxw / GameCanvas.bgW[0] + 1; j++)
			{
				g.drawImage(GameCanvas.imgBG[0], j * GameCanvas.bgW[0], TileMap.pxh - GameCanvas.bgH[0] - 70, 0);
			}
		}
	}

	// Token: 0x060004B7 RID: 1207 RVA: 0x0005C3E0 File Offset: 0x0005A5E0
	public void paintBlackSky(mGraphics g)
	{
		bool flag = !GameCanvas.lowGraphic;
		if (flag)
		{
			g.fillTrans(GameScr.imgTrans, 0, 0, GameCanvas.w, GameCanvas.h);
		}
	}

	// Token: 0x060004B8 RID: 1208 RVA: 0x0005C414 File Offset: 0x0005A614
	public void paintCapcha(mGraphics g)
	{
		MobCapcha.paint(g, global::Char.myCharz().cx, global::Char.myCharz().cy);
		g.translate(-g.getTranslateX(), -g.getTranslateY());
		bool flag = GameCanvas.menu.showMenu || GameCanvas.panel.isShow || ChatPopup.currChatPopup != null || !GameCanvas.isTouch;
		if (!flag)
		{
			for (int i = 0; i < this.strCapcha.Length; i++)
			{
				int num = (GameCanvas.w - this.strCapcha.Length * GameScr.disXC) / 2 + i * GameScr.disXC + GameScr.disXC / 2;
				bool flag2 = this.keyCapcha[i] == -1;
				if (flag2)
				{
					g.drawImage(GameScr.imgNut, num, GameCanvas.h - 25, 3);
					mFont.tahoma_7b_dark.drawString(g, this.strCapcha[i].ToString() + string.Empty, num, GameCanvas.h - 30, 2);
				}
				else
				{
					g.drawImage(GameScr.imgNutF, num, GameCanvas.h - 25, 3);
					mFont.tahoma_7b_green2.drawString(g, this.strCapcha[i].ToString() + string.Empty, num, GameCanvas.h - 30, 2);
				}
			}
		}
	}

	// Token: 0x060004B9 RID: 1209 RVA: 0x0005C580 File Offset: 0x0005A780
	public override void paint(mGraphics g)
	{
		GameScr.countEff = 0;
		bool flag = !GameScr.isPaint;
		if (!flag)
		{
			GameCanvas.debug("PA1", 1);
			bool flag2 = this.isUseFreez && ChatPopup.currChatPopup == null;
			if (flag2)
			{
				this.dem++;
				bool flag3 = (this.dem < 30 && this.dem >= 0 && GameCanvas.gameTick % 4 == 0) || (this.dem >= 30 && this.dem <= 50 && GameCanvas.gameTick % 3 == 0) || this.dem > 50;
				if (flag3)
				{
					bool flag4 = this.dem <= 50;
					if (flag4)
					{
						return;
					}
					bool flag5 = this.isUseFreez;
					if (flag5)
					{
						this.isUseFreez = false;
						this.dem = 0;
						bool flag6 = this.activeRongThan;
						if (flag6)
						{
							this.callRongThan(this.xR, this.yR);
						}
						else
						{
							this.hideRongThan();
						}
					}
					this.paintInfoBar(g);
					g.translate(-GameScr.cmx, -GameScr.cmy);
					g.translate(0, GameCanvas.transY);
					global::Char.myCharz().paint(g);
					mSystem.paintFlyText(g);
					GameScr.resetTranslate(g);
					this.paintSelectedSkill(g);
					return;
				}
			}
			GameCanvas.debug("PA2", 1);
			GameCanvas.paintBGGameScr(g);
			this.paint_ios_bg(g);
			bool flag7 = (this.isRongThanXuatHien || this.isFireWorks) && TileMap.bgID != 3;
			if (flag7)
			{
				this.paintBlackSky(g);
			}
			GameCanvas.debug("PA3", 1);
			bool flag8 = GameScr.shock_scr > 0;
			if (flag8)
			{
				g.translate(-GameScr.cmx + GameScr.shock_x[GameScr.shock_scr % GameScr.shock_x.Length], -GameScr.cmy + GameScr.shock_y[GameScr.shock_scr % GameScr.shock_y.Length]);
				GameScr.shock_scr--;
			}
			else
			{
				g.translate(-GameScr.cmx, -GameScr.cmy);
			}
			bool flag9 = this.isSuperPower;
			if (flag9)
			{
				int num = ((GameCanvas.gameTick % 3 != 0) ? (-3) : 3);
				g.translate(num, 0);
			}
			BackgroudEffect.paintBehindTileAll(g);
			EffecMn.paintLayer1(g);
			TileMap.paintTilemap(g);
			TileMap.paintOutTilemap(g);
			for (int i = 0; i < GameScr.vCharInMap.size(); i++)
			{
				global::Char @char = (global::Char)GameScr.vCharInMap.elementAt(i);
				bool flag10 = @char.isMabuHold && TileMap.mapID == 128;
				if (flag10)
				{
					@char.paintHeadWithXY(g, @char.cx, @char.cy, 0);
				}
			}
			bool flag11 = global::Char.myCharz().isMabuHold && TileMap.mapID == 128;
			if (flag11)
			{
				global::Char.myCharz().paintHeadWithXY(g, global::Char.myCharz().cx, global::Char.myCharz().cy, 0);
			}
			this.paintBgItem(g, 2);
			bool flag12 = global::Char.myCharz().cmdMenu != null && GameCanvas.isTouch;
			if (flag12)
			{
				bool flag13 = mScreen.keyTouch == 20;
				if (flag13)
				{
					g.drawImage(GameScr.imgChat2, global::Char.myCharz().cmdMenu.x + GameScr.cmx, global::Char.myCharz().cmdMenu.y + GameScr.cmy, mGraphics.HCENTER | mGraphics.VCENTER);
				}
				else
				{
					g.drawImage(GameScr.imgChat, global::Char.myCharz().cmdMenu.x + GameScr.cmx, global::Char.myCharz().cmdMenu.y + GameScr.cmy, mGraphics.HCENTER | mGraphics.VCENTER);
				}
			}
			GameCanvas.debug("PA4", 1);
			GameCanvas.debug("PA5", 1);
			BackgroudEffect.paintBackAll(g);
			EffectManager.lowEffects.paintAll(g);
			for (int j = 0; j < Effect2.vEffectFeet.size(); j++)
			{
				Effect2 effect = (Effect2)Effect2.vEffectFeet.elementAt(j);
				effect.paint(g);
			}
			for (int k = 0; k < Teleport.vTeleport.size(); k++)
			{
				((Teleport)Teleport.vTeleport.elementAt(k)).paintHole(g);
			}
			for (int l = 0; l < GameScr.vNpc.size(); l++)
			{
				Npc npc = (Npc)GameScr.vNpc.elementAt(l);
				bool flag14 = npc.cHP > 0L;
				if (flag14)
				{
					npc.paintShadow(g);
				}
			}
			for (int m = 0; m < GameScr.vNpc.size(); m++)
			{
				((Npc)GameScr.vNpc.elementAt(m)).paint(g);
			}
			g.translate(0, GameCanvas.transY);
			GameCanvas.debug("PA7", 1);
			GameCanvas.debug("PA8", 1);
			for (int n = 0; n < GameScr.vCharInMap.size(); n++)
			{
				global::Char char2 = null;
				try
				{
					char2 = (global::Char)GameScr.vCharInMap.elementAt(n);
				}
				catch (Exception ex)
				{
					Cout.LogError("Loi ham paint char gamesc: " + ex.ToString());
				}
				bool flag15 = char2 != null && (!GameCanvas.panel.isShow || !GameCanvas.panel.isTypeShop()) && char2.isShadown;
				if (flag15)
				{
					char2.paintShadow(g);
				}
			}
			global::Char.myCharz().paintShadow(g);
			EffecMn.paintLayer2(g);
			for (int num2 = 0; num2 < GameScr.vMob.size(); num2++)
			{
				((Mob)GameScr.vMob.elementAt(num2)).paint(g);
			}
			for (int num3 = 0; num3 < Teleport.vTeleport.size(); num3++)
			{
				((Teleport)Teleport.vTeleport.elementAt(num3)).paint(g);
			}
			for (int num4 = 0; num4 < GameScr.vCharInMap.size(); num4++)
			{
				global::Char char3 = null;
				try
				{
					char3 = (global::Char)GameScr.vCharInMap.elementAt(num4);
				}
				catch (Exception)
				{
				}
				bool flag16 = char3 != null && (!GameCanvas.panel.isShow || !GameCanvas.panel.isTypeShop());
				if (flag16)
				{
					char3.paint(g);
				}
			}
			global::Char.myCharz().paint(g);
			bool flag17 = global::Char.myCharz().skillPaint != null && global::Char.myCharz().skillInfoPaint() != null && global::Char.myCharz().indexSkill < global::Char.myCharz().skillInfoPaint().Length;
			if (flag17)
			{
				global::Char.myCharz().paintCharWithSkill(g);
				global::Char.myCharz().paintMount2(g);
			}
			for (int num5 = 0; num5 < GameScr.vCharInMap.size(); num5++)
			{
				global::Char char4 = null;
				try
				{
					char4 = (global::Char)GameScr.vCharInMap.elementAt(num5);
				}
				catch (Exception ex2)
				{
					Cout.LogError("Loi ham paint char gamescr: " + ex2.ToString());
				}
				bool flag18 = char4 != null && (!GameCanvas.panel.isShow || !GameCanvas.panel.isTypeShop()) && char4.skillPaint != null && char4.skillInfoPaint() != null && char4.indexSkill < char4.skillInfoPaint().Length;
				if (flag18)
				{
					char4.paintCharWithSkill(g);
					char4.paintMount2(g);
				}
			}
			for (int num6 = 0; num6 < GameScr.vItemMap.size(); num6++)
			{
				((ItemMap)GameScr.vItemMap.elementAt(num6)).paint(g);
			}
			g.translate(0, -GameCanvas.transY);
			GameCanvas.debug("PA9", 1);
			GameScr.paintSplash(g);
			GameCanvas.debug("PA10", 1);
			GameCanvas.debug("PA11", 1);
			GameCanvas.debug("PA13", 1);
			this.paintEffect(g);
			this.paintBgItem(g, 3);
			for (int num7 = 0; num7 < GameScr.vNpc.size(); num7++)
			{
				Npc npc2 = (Npc)GameScr.vNpc.elementAt(num7);
				npc2.paintName(g);
			}
			EffecMn.paintLayer3(g);
			for (int num8 = 0; num8 < GameScr.vNpc.size(); num8++)
			{
				Npc npc3 = (Npc)GameScr.vNpc.elementAt(num8);
				bool flag19 = npc3.chatInfo != null;
				if (flag19)
				{
					if (npc3 != null)
					{
						npc3.chatInfo.paint(g, npc3.cx, npc3.cy - npc3.ch - GameCanvas.transY, npc3.cdir);
					}
				}
			}
			for (int num9 = 0; num9 < GameScr.vCharInMap.size(); num9++)
			{
				global::Char char5 = null;
				try
				{
					char5 = (global::Char)GameScr.vCharInMap.elementAt(num9);
				}
				catch (Exception)
				{
				}
				bool flag20 = char5 != null && char5.chatInfo != null;
				if (flag20)
				{
					char5.chatInfo.paint(g, char5.cx, char5.cy - char5.ch, char5.cdir);
				}
			}
			bool flag21 = global::Char.myCharz().chatInfo != null;
			if (flag21)
			{
				global::Char.myCharz().chatInfo.paint(g, global::Char.myCharz().cx, global::Char.myCharz().cy - global::Char.myCharz().ch, global::Char.myCharz().cdir);
			}
			EffectManager.mid_2Effects.paintAll(g);
			EffectManager.midEffects.paintAll(g);
			BackgroudEffect.paintFrontAll(g);
			for (int num10 = 0; num10 < TileMap.vCurrItem.size(); num10++)
			{
				BgItem bgItem = (BgItem)TileMap.vCurrItem.elementAt(num10);
				bool flag22 = bgItem.idImage != -1 && bgItem.layer > 3;
				if (flag22)
				{
					bgItem.paint(g);
				}
			}
			PopUp.paintAll(g);
			bool flag23 = TileMap.mapID == 120;
			if (flag23)
			{
				bool flag24 = this.percentMabu != 100;
				if (flag24)
				{
					int num11 = (int)this.percentMabu * mGraphics.getImageWidth(GameScr.imgHPLost) / 100;
					int num12 = (int)this.percentMabu;
					g.drawImage(GameScr.imgHPLost, TileMap.pxw / 2 - mGraphics.getImageWidth(GameScr.imgHPLost) / 2, 220, 0);
					g.setClip(TileMap.pxw / 2 - mGraphics.getImageWidth(GameScr.imgHPLost) / 2, 220, num11, 10);
					g.drawImage(GameScr.imgHP, TileMap.pxw / 2 - mGraphics.getImageWidth(GameScr.imgHPLost) / 2, 220, 0);
					g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
				}
				bool flag25 = this.mabuEff;
				if (flag25)
				{
					this.tMabuEff++;
					bool flag26 = GameCanvas.gameTick % 3 == 0;
					if (flag26)
					{
						Effect effect2 = new Effect(19, Res.random(TileMap.pxw / 2 - 50, TileMap.pxw / 2 + 50), 340, 2, 1, -1);
						EffecMn.addEff(effect2);
					}
					bool flag27 = GameCanvas.gameTick % 15 == 0;
					if (flag27)
					{
						Effect effect3 = new Effect(18, Res.random(TileMap.pxw / 2 - 5, TileMap.pxw / 2 + 5), Res.random(300, 320), 2, 1, -1);
						EffecMn.addEff(effect3);
					}
					bool flag28 = this.tMabuEff == 100;
					if (flag28)
					{
						this.activeSuperPower(TileMap.pxw / 2, 300);
					}
					bool flag29 = this.tMabuEff == 110;
					if (flag29)
					{
						this.tMabuEff = 0;
						this.mabuEff = false;
					}
				}
			}
			BackgroudEffect.paintFog(g);
			bool flag30 = true;
			for (int num13 = 0; num13 < BackgroudEffect.vBgEffect.size(); num13++)
			{
				BackgroudEffect backgroudEffect = (BackgroudEffect)BackgroudEffect.vBgEffect.elementAt(num13);
				bool flag31 = backgroudEffect.typeEff == 0;
				if (flag31)
				{
					flag30 = false;
					break;
				}
			}
			bool flag32 = mGraphics.zoomLevel <= 1 || Main.isIpod || Main.isIphone4;
			if (flag32)
			{
				flag30 = false;
			}
			bool flag33 = flag30 && !this.isRongThanXuatHien;
			if (flag33)
			{
				int num14 = TileMap.pxw / (mGraphics.getImageWidth(TileMap.imgLight) + 50);
				bool flag34 = num14 <= 0;
				if (flag34)
				{
					num14 = 1;
				}
				bool flag35 = TileMap.tileID != 28;
				if (flag35)
				{
					for (int num15 = 0; num15 < num14; num15++)
					{
						int num16 = 100 + num15 * (mGraphics.getImageWidth(TileMap.imgLight) + 50) - GameScr.cmx / 2;
						int num17 = -20;
						int imageWidth = mGraphics.getImageWidth(TileMap.imgLight);
						bool flag36 = num16 + imageWidth >= GameScr.cmx && num16 <= GameScr.cmx + GameCanvas.w && num17 + mGraphics.getImageHeight(TileMap.imgLight) >= GameScr.cmy && num17 <= GameScr.cmy + GameCanvas.h;
						if (flag36)
						{
							g.drawImage(TileMap.imgLight, 100 + num15 * (mGraphics.getImageWidth(TileMap.imgLight) + 50) - GameScr.cmx / 2, num17, 0);
						}
					}
				}
			}
			mSystem.paintFlyText(g);
			GameCanvas.debug("PA14", 1);
			GameCanvas.debug("PA15", 1);
			GameCanvas.debug("PA16", 1);
			this.paintArrowPointToNPC(g);
			GameCanvas.debug("PA17", 1);
			bool flag37 = !GameScr.isPaintOther && GameScr.isPaintRada == 1 && !GameCanvas.panel.isShow;
			if (flag37)
			{
				this.paintInfoBar(g);
			}
			GameScr.resetTranslate(g);
			this.paint_xp_bar(g);
			bool flag38 = !GameScr.isPaintOther;
			if (flag38)
			{
				AutoBroly.Painting(g);
				GameCanvas.debug("PA21", 1);
				GameCanvas.debug("PA18", 1);
				g.translate(-g.getTranslateX(), -g.getTranslateY());
				bool flag39 = (TileMap.mapID == 128 || TileMap.mapID == 127) && GameScr.mabuPercent != 0;
				if (flag39)
				{
					int num18 = 30;
					int num19 = 200;
					g.setColor(0);
					g.fillRect(num18 - 27, num19 - 112, 54, 8);
					g.setColor(16711680);
					g.setClip(num18 - 25, num19 - 110, (int)GameScr.mabuPercent, 4);
					g.fillRect(num18 - 25, num19 - 110, 50, 4);
					g.setClip(0, 0, 3000, 3000);
					mFont.tahoma_7b_white.drawString(g, "Mabu", num18, num19 - 112 + 10, 2, mFont.tahoma_7b_dark);
				}
				bool isFusion = global::Char.myCharz().isFusion;
				if (isFusion)
				{
					global::Char.myCharz().tFusion++;
					bool flag40 = GameCanvas.gameTick % 3 == 0;
					if (flag40)
					{
						g.setColor(16777215);
						g.fillRect(0, 0, GameCanvas.w, GameCanvas.h);
					}
					bool flag41 = global::Char.myCharz().tFusion >= 100;
					if (flag41)
					{
						global::Char.myCharz().fusionComplete();
					}
				}
				for (int num20 = 0; num20 < GameScr.vCharInMap.size(); num20++)
				{
					global::Char char6 = null;
					try
					{
						char6 = (global::Char)GameScr.vCharInMap.elementAt(num20);
					}
					catch (Exception)
					{
					}
					bool flag42 = char6 != null && char6.isFusion && global::Char.isCharInScreen(char6);
					if (flag42)
					{
						char6.tFusion++;
						bool flag43 = GameCanvas.gameTick % 3 == 0;
						if (flag43)
						{
							g.setColor(16777215);
							g.fillRect(0, 0, GameCanvas.w, GameCanvas.h);
						}
						bool flag44 = char6.tFusion >= 100;
						if (flag44)
						{
							char6.fusionComplete();
						}
					}
				}
				GameCanvas.paintz.paintTabSoft(g);
				GameCanvas.debug("PA19", 1);
				GameCanvas.debug("PA20", 1);
				GameScr.resetTranslate(g);
				this.paintSelectedSkill(g);
				GameCanvas.debug("PA22", 1);
				GameScr.resetTranslate(g);
				bool flag45 = GameCanvas.isTouch && GameCanvas.isTouchControl;
				if (flag45)
				{
					this.paintTouchControl(g);
				}
				GameScr.resetTranslate(g);
				this.paintChatVip(g);
				bool flag46 = !GameCanvas.panel.isShow && GameCanvas.currentDialog == null && ChatPopup.currChatPopup == null && ChatPopup.serverChatPopUp == null && GameCanvas.currentScreen.Equals(GameScr.instance);
				if (flag46)
				{
					base.paint(g);
					bool flag47 = mScreen.keyMouse == 1 && this.cmdMenu != null;
					if (flag47)
					{
						g.drawImage(ItemMap.imageFlare, this.cmdMenu.x + 7, this.cmdMenu.y + 15, 3);
					}
				}
				GameScr.resetTranslate(g);
				int num21 = 100 + ((global::Char.vItemTime.size() != 0) ? (GameScr.textTime.size() * 12) : 0);
				bool flag48 = global::Char.myCharz().clan != null;
				if (flag48)
				{
					int num22 = 0;
					int num23 = 0;
					int num24 = (GameCanvas.h - 100 - 60) / 12;
					for (int num25 = 0; num25 < GameScr.vCharInMap.size(); num25++)
					{
						global::Char char7 = (global::Char)GameScr.vCharInMap.elementAt(num25);
						bool flag49 = char7.clanID == -1 || char7.clanID != global::Char.myCharz().clan.ID;
						if (!flag49)
						{
							bool flag50 = char7.isOutX() && char7.cx < global::Char.myCharz().cx;
							if (flag50)
							{
								int num26 = num24;
								bool flag51 = global::Char.vItemTime.size() != 0;
								if (flag51)
								{
									num26 -= GameScr.textTime.size();
								}
								bool flag52 = num22 <= num26;
								if (flag52)
								{
									mFont.tahoma_7_green.drawString(g, char7.cName, 20, num21 - 12 + num22 * 12, mFont.LEFT, mFont.tahoma_7_grey);
									char7.paintHp(g, 10, num21 + num22 * 12 - 5);
									num22++;
								}
							}
							else
							{
								bool flag53 = char7.isOutX() && char7.cx > global::Char.myCharz().cx && num23 <= num24;
								if (flag53)
								{
									mFont.tahoma_7_green.drawString(g, char7.cName, GameCanvas.w - 25, num21 - 12 + num23 * 12, mFont.RIGHT, mFont.tahoma_7_grey);
									char7.paintHp(g, GameCanvas.w - 15, num21 + num23 * 12 - 5);
									num23++;
								}
							}
						}
					}
				}
				ChatTextField.gI().paint(g);
				bool flag54 = GameScr.isNewClanMessage && !GameCanvas.panel.isShow && GameCanvas.gameTick % 4 == 0;
				if (flag54)
				{
					g.drawImage(ItemMap.imageFlare, this.cmdMenu.x + 15, this.cmdMenu.y + 30, mGraphics.BOTTOM | mGraphics.HCENTER);
				}
				bool flag55 = this.isSuperPower;
				if (flag55)
				{
					this.dxPower += 5;
					bool flag56 = this.tPower >= 0;
					if (flag56)
					{
						this.tPower += this.dxPower;
					}
					Res.outz("x power= " + this.xPower.ToString());
					bool flag57 = this.tPower < 0;
					if (flag57)
					{
						this.tPower--;
						bool flag58 = this.tPower == -20;
						if (flag58)
						{
							this.isSuperPower = false;
							this.tPower = 0;
							this.dxPower = 0;
						}
					}
					else
					{
						bool flag59 = (this.xPower - this.tPower > 0 || this.tPower < TileMap.pxw) && this.tPower > 0;
						if (flag59)
						{
							g.setColor(16777215);
							bool flag60 = !GameCanvas.lowGraphic;
							if (flag60)
							{
								g.fillArg(0, 0, GameCanvas.w, GameCanvas.h, 0, 0);
							}
							else
							{
								g.fillRect(0, 0, GameCanvas.w, GameCanvas.h);
							}
						}
						else
						{
							this.tPower = -1;
						}
					}
				}
				for (int num27 = 0; num27 < global::Char.vItemTime.size(); num27++)
				{
					((ItemTime)global::Char.vItemTime.elementAt(num27)).paint(g, this.cmdMenu.x + 32 + num27 * 24, 55);
				}
				for (int num28 = 0; num28 < GameScr.textTime.size(); num28++)
				{
					((ItemTime)GameScr.textTime.elementAt(num28)).paintText(g, this.cmdMenu.x + ((global::Char.vItemTime.size() == 0) ? 25 : 5), ((global::Char.vItemTime.size() == 0) ? 45 : 90) + num28 * 12);
				}
				this.paintXoSo(g);
				bool flag61 = mResources.language == 1;
				if (flag61)
				{
					long num29 = mSystem.currentTimeMillis() - GameScr.deltaTime;
					mFont.tahoma_7b_white.drawString(g, NinjaUtil.getDate2(num29), 10, GameCanvas.h - 65, 0, mFont.tahoma_7b_dark);
				}
				bool flag62 = !this.yourNumber.Equals(string.Empty);
				if (flag62)
				{
					for (int num30 = 0; num30 < this.strPaint.Length; num30++)
					{
						mFont.tahoma_7b_white.drawString(g, this.strPaint[num30], 5, 85 + num30 * 18, 0, mFont.tahoma_7b_dark);
					}
				}
			}
			int num31 = 0;
			int num32 = GameCanvas.hw;
			bool flag63 = num32 > 200;
			if (flag63)
			{
				num32 = 200;
			}
			this.paintPhuBanBar(g, num31 + GameCanvas.w / 2, 0, num32);
			EffectManager.hiEffects.paintAll(g);
			bool flag64 = GameScr.nCT_timeBallte > mSystem.currentTimeMillis() && TileMap.mapID == 170 && GameScr.isPaint_CT && GameScr.nCT_nBoyBaller / 2 > 0;
			if (flag64)
			{
				try
				{
					this.paint_CT(g, num31 + GameCanvas.w / 2, 0, num32);
				}
				catch (Exception)
				{
				}
			}
			bool flag65 = TileMap.mapID == 172;
			if (flag65)
			{
				string text = string.Concat(new string[]
				{
					mResources.WAIT,
					"  ",
					GameScr.nUSER_CT.ToString(),
					"/",
					GameScr.nUSER_MAX_CT.ToString()
				});
				mFont.tahoma_7b_dark.drawString(g, string.Concat(new string[]
				{
					mResources.WAIT,
					"  ",
					GameScr.nUSER_CT.ToString(),
					"/",
					GameScr.nUSER_MAX_CT.ToString()
				}), GameCanvas.w - 10, 40, 1);
			}
		}
	}

	// Token: 0x060004BA RID: 1210 RVA: 0x0005DD64 File Offset: 0x0005BF64
	private void paintXoSo(mGraphics g)
	{
		bool flag = this.tShow != 0;
		if (flag)
		{
			string text = string.Empty;
			for (int i = 0; i < this.winnumber.Length; i++)
			{
				text = text + this.randomNumber[i].ToString() + " ";
			}
			PopUp.paintPopUp(g, 20, 45, 95, 35, 16777215, false);
			mFont.tahoma_7b_dark.drawString(g, mResources.kquaVongQuay, 68, 50, 2);
			mFont.tahoma_7b_dark.drawString(g, text + string.Empty, 68, 65, 2);
		}
	}

	// Token: 0x060004BB RID: 1211 RVA: 0x0005DE08 File Offset: 0x0005C008
	private void checkEffToObj(IMapObject obj, bool isnew)
	{
		bool flag = obj == null || this.tDoubleDelay > 0;
		if (!flag)
		{
			this.tDoubleDelay = 10;
			int x = obj.getX();
			int num = Res.abs(global::Char.myCharz().cx - x);
			int num2 = ((num <= 80) ? 1 : ((num > 80 && num <= 200) ? 2 : ((num <= 200 || num > 400) ? 4 : 3)));
			bool flag2 = !isnew;
			if (flag2)
			{
				bool flag3 = obj.Equals(global::Char.myCharz().mobFocus) || (obj.Equals(global::Char.myCharz().charFocus) && global::Char.myCharz().isMeCanAttackOtherPlayer(global::Char.myCharz().charFocus));
				if (flag3)
				{
					ServerEffect.addServerEffect(135, obj.getX(), obj.getY(), num2);
				}
				else
				{
					bool flag4 = obj.Equals(global::Char.myCharz().npcFocus) || obj.Equals(global::Char.myCharz().itemFocus) || obj.Equals(global::Char.myCharz().charFocus);
					if (flag4)
					{
						ServerEffect.addServerEffect(136, obj.getX(), obj.getY(), num2);
					}
				}
			}
			else
			{
				ServerEffect.addServerEffect(136, obj.getX(), obj.getY(), num2);
			}
		}
	}

	// Token: 0x060004BC RID: 1212 RVA: 0x0005DF64 File Offset: 0x0005C164
	private void updateClickToArrow()
	{
		bool flag = this.tDoubleDelay > 0;
		if (flag)
		{
			this.tDoubleDelay--;
		}
		bool flag2 = this.clickMoving;
		if (flag2)
		{
			this.clickMoving = false;
			IMapObject mapObject = this.findClickToItem(this.clickToX, this.clickToY);
			bool flag3 = mapObject == null || (mapObject != null && mapObject.Equals(global::Char.myCharz().npcFocus) && TileMap.mapID == 51);
			if (flag3)
			{
				ServerEffect.addServerEffect(134, this.clickToX, this.clickToY + GameCanvas.transY / 2, 3);
			}
		}
	}

	// Token: 0x060004BD RID: 1213 RVA: 0x0005E004 File Offset: 0x0005C204
	private void paintWaypointArrow(mGraphics g)
	{
		int num = 10;
		Task taskMaint = global::Char.myCharz().taskMaint;
		bool flag = taskMaint != null && taskMaint.taskId == 0 && ((taskMaint.index != 1 && taskMaint.index < 6) || taskMaint.index == 0);
		if (!flag)
		{
			for (int i = 0; i < TileMap.vGo.size(); i++)
			{
				Waypoint waypoint = (Waypoint)TileMap.vGo.elementAt(i);
				bool flag2 = waypoint.minY == 0 || (int)waypoint.maxY >= TileMap.pxh - 24;
				if (flag2)
				{
					bool flag3 = (int)waypoint.maxY <= TileMap.pxh / 2;
					if (flag3)
					{
						int num2 = (int)(waypoint.minX + (waypoint.maxX - waypoint.minX) / 2);
						int num3 = (int)(waypoint.minY + (waypoint.maxY - waypoint.minY) / 2) + this.runArrow;
						bool isTouch = GameCanvas.isTouch;
						if (isTouch)
						{
							num3 = (int)(waypoint.maxY + (waypoint.maxY - waypoint.minY)) + this.runArrow + num;
						}
						g.drawRegion(GameScr.arrow, 0, 0, 13, 16, 6, num2, num3, StaticObj.VCENTER_HCENTER);
					}
					else
					{
						bool flag4 = (int)waypoint.minY >= TileMap.pxh / 2;
						if (flag4)
						{
							g.drawRegion(GameScr.arrow, 0, 0, 13, 16, 4, (int)(waypoint.minX + (waypoint.maxX - waypoint.minX) / 2), (int)(waypoint.minY - 12) - this.runArrow, StaticObj.VCENTER_HCENTER);
						}
					}
				}
				else
				{
					bool flag5 = waypoint.minX >= 0 && waypoint.minX < 24;
					if (flag5)
					{
						bool flag6 = !GameCanvas.isTouch;
						if (flag6)
						{
							g.drawRegion(GameScr.arrow, 0, 0, 13, 16, 2, (int)(waypoint.maxX + 12) + this.runArrow, (int)(waypoint.maxY - 12), StaticObj.VCENTER_HCENTER);
						}
						else
						{
							g.drawRegion(GameScr.arrow, 0, 0, 13, 16, 2, (int)(waypoint.maxX + 12) + this.runArrow, (int)(waypoint.maxY - 32), StaticObj.VCENTER_HCENTER);
						}
					}
					else
					{
						bool flag7 = (int)waypoint.minX <= TileMap.tmw * 24 && (int)waypoint.minX >= TileMap.tmw * 24 - 48;
						if (flag7)
						{
							bool flag8 = !GameCanvas.isTouch;
							if (flag8)
							{
								g.drawRegion(GameScr.arrow, 0, 0, 13, 16, 0, (int)(waypoint.minX - 12) - this.runArrow, (int)(waypoint.maxY - 12), StaticObj.VCENTER_HCENTER);
							}
							else
							{
								g.drawRegion(GameScr.arrow, 0, 0, 13, 16, 0, (int)(waypoint.minX - 12) - this.runArrow, (int)(waypoint.maxY - 32), StaticObj.VCENTER_HCENTER);
							}
						}
						else
						{
							g.drawRegion(GameScr.arrow, 0, 0, 13, 16, 4, (int)(waypoint.minX + (waypoint.maxX - waypoint.minX) / 2), (int)(waypoint.maxY - 48) - this.runArrow, StaticObj.VCENTER_HCENTER);
						}
					}
				}
			}
		}
	}

	// Token: 0x060004BE RID: 1214 RVA: 0x0005E358 File Offset: 0x0005C558
	public static Npc findNPCInMap(short id)
	{
		for (int i = 0; i < GameScr.vNpc.size(); i++)
		{
			Npc npc = (Npc)GameScr.vNpc.elementAt(i);
			bool flag = npc.template.npcTemplateId == (int)id;
			if (flag)
			{
				return npc;
			}
		}
		return null;
	}

	// Token: 0x060004BF RID: 1215 RVA: 0x0005E3B0 File Offset: 0x0005C5B0
	public static global::Char findCharInMap(int charId)
	{
		for (int i = 0; i < GameScr.vCharInMap.size(); i++)
		{
			global::Char @char = (global::Char)GameScr.vCharInMap.elementAt(i);
			bool flag = @char.charID == charId;
			if (flag)
			{
				return @char;
			}
		}
		return null;
	}

	// Token: 0x060004C0 RID: 1216 RVA: 0x0005E404 File Offset: 0x0005C604
	public static Mob findMobInMap(sbyte mobIndex)
	{
		return (Mob)GameScr.vMob.elementAt((int)mobIndex);
	}

	// Token: 0x060004C1 RID: 1217 RVA: 0x0005E428 File Offset: 0x0005C628
	public static Mob findMobInMap(int mobId)
	{
		for (int i = 0; i < GameScr.vMob.size(); i++)
		{
			Mob mob = (Mob)GameScr.vMob.elementAt(i);
			bool flag = mob.mobId == mobId;
			if (flag)
			{
				return mob;
			}
		}
		return null;
	}

	// Token: 0x060004C2 RID: 1218 RVA: 0x0005E47C File Offset: 0x0005C67C
	public static Npc getNpcTask()
	{
		for (int i = 0; i < GameScr.vNpc.size(); i++)
		{
			Npc npc = (Npc)GameScr.vNpc.elementAt(i);
			bool flag = npc.template.npcTemplateId == (int)GameScr.getTaskNpcId();
			if (flag)
			{
				return npc;
			}
		}
		return null;
	}

	// Token: 0x060004C3 RID: 1219 RVA: 0x0005E4D8 File Offset: 0x0005C6D8
	private void paintArrowPointToNPC(mGraphics g)
	{
		try
		{
			bool flag = ChatPopup.currChatPopup != null;
			if (!flag)
			{
				int taskNpcId = (int)GameScr.getTaskNpcId();
				bool flag2 = taskNpcId == -1;
				if (!flag2)
				{
					Npc npc = null;
					for (int i = 0; i < GameScr.vNpc.size(); i++)
					{
						Npc npc2 = (Npc)GameScr.vNpc.elementAt(i);
						bool flag3 = npc2.template.npcTemplateId == taskNpcId;
						if (flag3)
						{
							bool flag4 = npc == null;
							if (flag4)
							{
								npc = npc2;
							}
							else
							{
								bool flag5 = Res.abs(npc2.cx - global::Char.myCharz().cx) < Res.abs(npc.cx - global::Char.myCharz().cx);
								if (flag5)
								{
									npc = npc2;
								}
							}
						}
					}
					bool flag6 = npc == null || npc.statusMe == 15 || (npc.cx > GameScr.cmx && npc.cx < GameScr.cmx + GameScr.gW && npc.cy > GameScr.cmy && npc.cy < GameScr.cmy + GameScr.gH) || GameCanvas.gameTick % 10 < 5;
					if (!flag6)
					{
						int num = npc.cx - global::Char.myCharz().cx;
						int num2 = npc.cy - global::Char.myCharz().cy;
						int num3 = 0;
						int num4 = 0;
						int num5 = 0;
						bool flag7 = num > 0 && num2 >= 0;
						if (flag7)
						{
							bool flag8 = Res.abs(num) >= Res.abs(num2);
							if (flag8)
							{
								num3 = GameScr.gW - 10;
								num4 = GameScr.gH / 2 + 30;
								bool isTouch = GameCanvas.isTouch;
								if (isTouch)
								{
									num4 = GameScr.gH / 2 + 10;
								}
								num5 = 0;
							}
							else
							{
								num3 = GameScr.gW / 2;
								num4 = GameScr.gH - 10;
								num5 = 5;
							}
						}
						else
						{
							bool flag9 = num >= 0 && num2 < 0;
							if (flag9)
							{
								bool flag10 = Res.abs(num) >= Res.abs(num2);
								if (flag10)
								{
									num3 = GameScr.gW - 10;
									num4 = GameScr.gH / 2 + 30;
									bool isTouch2 = GameCanvas.isTouch;
									if (isTouch2)
									{
										num4 = GameScr.gH / 2 + 10;
									}
									num5 = 0;
								}
								else
								{
									num3 = GameScr.gW / 2;
									num4 = 10;
									num5 = 6;
								}
							}
						}
						bool flag11 = num < 0 && num2 >= 0;
						if (flag11)
						{
							bool flag12 = Res.abs(num) >= Res.abs(num2);
							if (flag12)
							{
								num3 = 10;
								num4 = GameScr.gH / 2 + 30;
								bool isTouch3 = GameCanvas.isTouch;
								if (isTouch3)
								{
									num4 = GameScr.gH / 2 + 10;
								}
								num5 = 3;
							}
							else
							{
								num3 = GameScr.gW / 2;
								num4 = GameScr.gH - 10;
								num5 = 5;
							}
						}
						else
						{
							bool flag13 = num <= 0 && num2 < 0;
							if (flag13)
							{
								bool flag14 = Res.abs(num) >= Res.abs(num2);
								if (flag14)
								{
									num3 = 10;
									num4 = GameScr.gH / 2 + 30;
									bool isTouch4 = GameCanvas.isTouch;
									if (isTouch4)
									{
										num4 = GameScr.gH / 2 + 10;
									}
									num5 = 3;
								}
								else
								{
									num3 = GameScr.gW / 2;
									num4 = 10;
									num5 = 6;
								}
							}
						}
						GameScr.resetTranslate(g);
						g.drawRegion(GameScr.arrow, 0, 0, 13, 16, num5, num3, num4, StaticObj.VCENTER_HCENTER);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Cout.LogError("Loi ham arrow to npc: " + ex.ToString());
		}
	}

	// Token: 0x060004C4 RID: 1220 RVA: 0x00004D65 File Offset: 0x00002F65
	public static void resetTranslate(mGraphics g)
	{
		g.translate(-g.getTranslateX(), -g.getTranslateY());
		g.setClip(0, -200, GameCanvas.w, 200 + GameCanvas.h);
	}

	// Token: 0x060004C5 RID: 1221 RVA: 0x0005E874 File Offset: 0x0005CA74
	private void paintTouchControl(mGraphics g)
	{
		bool flag = this.isNotPaintTouchControl();
		if (!flag)
		{
			GameScr.resetTranslate(g);
			bool flag2 = !TileMap.isOfflineMap() && !this.isVS();
			if (flag2)
			{
				bool flag3 = mScreen.keyTouch == 15 || mScreen.keyMouse == 15;
				if (flag3)
				{
					g.drawImage((!Main.isPC) ? GameScr.imgChat2 : GameScr.imgChatsPC2, GameScr.xC + 17, GameScr.yC + 17 + mGraphics.addYWhenOpenKeyBoard, mGraphics.HCENTER | mGraphics.VCENTER);
				}
				else
				{
					g.drawImage((!Main.isPC) ? GameScr.imgChat : GameScr.imgChatPC, GameScr.xC + 17, GameScr.yC + 17 + mGraphics.addYWhenOpenKeyBoard, mGraphics.HCENTER | mGraphics.VCENTER);
				}
			}
			bool flag4 = GameScr.isUseTouch;
			if (flag4)
			{
			}
		}
	}

	// Token: 0x060004C6 RID: 1222 RVA: 0x0005E958 File Offset: 0x0005CB58
	public void paintImageBarRight(mGraphics g, global::Char c)
	{
		int num = (int)(c.cHP * GameScr.hpBarW / c.cHPFull);
		int num2 = (int)(c.cMP * (long)GameScr.mpBarW / c.cMPFull);
		int num3 = (int)(this.dHP * GameScr.hpBarW / c.cHPFull);
		int num4 = (int)(this.dMP * (long)GameScr.mpBarW / c.cMPFull);
		g.setClip(GameCanvas.w / 2 + 58 - mGraphics.getImageWidth(GameScr.imgPanel), 0, 95, 100);
		g.drawRegion(GameScr.imgPanel, 0, 0, mGraphics.getImageWidth(GameScr.imgPanel), mGraphics.getImageHeight(GameScr.imgPanel), 2, GameCanvas.w / 2 + 60, 0, mGraphics.RIGHT | mGraphics.TOP);
		g.setClip((int)((long)(GameCanvas.w / 2 + 60 - 83) - GameScr.hpBarW + GameScr.hpBarW - (long)num3), 5, num3, 10);
		g.drawImage(GameScr.imgHPLost, GameCanvas.w / 2 + 60 - 83, 5, mGraphics.RIGHT | mGraphics.TOP);
		g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
		g.setClip((int)((long)(GameCanvas.w / 2 + 60 - 83) - GameScr.hpBarW + GameScr.hpBarW - (long)num), 5, num, 10);
		g.drawImage(GameScr.imgHP, GameCanvas.w / 2 + 60 - 83, 5, mGraphics.RIGHT | mGraphics.TOP);
		g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
		g.setClip((int)((long)(GameCanvas.w / 2 + 60 - 83 - GameScr.mpBarW) + GameScr.hpBarW - (long)num4), 20, num4, 6);
		g.drawImage(GameScr.imgMPLost, GameCanvas.w / 2 + 60 - 83, 20, mGraphics.RIGHT | mGraphics.TOP);
		g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
		g.setClip((int)((long)(GameCanvas.w / 2 + 60 - 83 - GameScr.mpBarW) + GameScr.hpBarW - (long)num2), 20, num2, 6);
		g.drawImage(GameScr.imgMP, GameCanvas.w / 2 + 60 - 83, 20, mGraphics.RIGHT | mGraphics.TOP);
		g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
	}

	// Token: 0x060004C7 RID: 1223 RVA: 0x0005EB9C File Offset: 0x0005CD9C
	private void paintImageBar(mGraphics g, bool isLeft, global::Char c)
	{
		bool flag = c != null;
		if (flag)
		{
			bool flag2 = c.charID == global::Char.myCharz().charID;
			int num;
			int num2;
			int num3;
			int num4;
			if (flag2)
			{
				num = (int)(this.dHP * GameScr.hpBarW / c.cHPFull);
				num2 = (int)(this.dMP * (long)GameScr.mpBarW / c.cMPFull);
				num3 = (int)(c.cHP * GameScr.hpBarW / c.cHPFull);
				num4 = (int)(c.cMP * (long)GameScr.mpBarW / c.cMPFull);
			}
			else
			{
				num = (int)(c.dHP * GameScr.hpBarW / c.cHPFull);
				num2 = c.perCentMp * GameScr.mpBarW / 100;
				num3 = (int)(c.cHP * GameScr.hpBarW / c.cHPFull);
				num4 = c.perCentMp * GameScr.mpBarW / 100;
			}
			bool flag3 = global::Char.myCharz().secondPower > 0;
			if (flag3)
			{
				int num5 = (int)global::Char.myCharz().powerPoint * GameScr.spBarW / (int)global::Char.myCharz().maxPowerPoint;
				g.drawImage(GameScr.imgPanel2, 58, 29, 0);
				g.setClip(83, 31, num5, 10);
				g.drawImage(GameScr.imgSP, 83, 31, 0);
				g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
				mFont.tahoma_7_white.drawString(g, string.Concat(new string[]
				{
					global::Char.myCharz().strInfo,
					":",
					global::Char.myCharz().powerPoint.ToString(),
					"/",
					global::Char.myCharz().maxPowerPoint.ToString()
				}), 115, 29, 2);
			}
			bool flag4 = c.charID != global::Char.myCharz().charID;
			if (flag4)
			{
				g.setClip(mGraphics.getImageWidth(GameScr.imgPanel) - 95, 0, 95, 100);
			}
			g.drawImage(GameScr.imgPanel, 0, 0, 0);
			if (isLeft)
			{
				g.setClip(83, 5, num, 10);
			}
			else
			{
				g.setClip((int)(83L + GameScr.hpBarW - (long)num), 5, num, 10);
			}
			g.drawImage(GameScr.imgHPLost, 83, 5, 0);
			g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
			if (isLeft)
			{
				g.setClip(83, 5, num3, 10);
			}
			else
			{
				g.setClip((int)(83L + GameScr.hpBarW - (long)num3), 5, num3, 10);
			}
			g.drawImage(GameScr.imgHP, 83, 5, 0);
			g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
			if (isLeft)
			{
				g.setClip(83, 20, num2, 6);
			}
			else
			{
				g.setClip(83 + GameScr.mpBarW - num2, 20, num2, 6);
			}
			g.drawImage(GameScr.imgMPLost, 83, 20, 0);
			g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
			if (isLeft)
			{
				g.setClip(83, 20, num2, 6);
			}
			else
			{
				g.setClip(83 + GameScr.mpBarW - num4, 20, num4, 6);
			}
			g.drawImage(GameScr.imgMP, 83, 20, 0);
			g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
			bool flag5 = global::Char.myCharz().cMP == 0L && GameCanvas.gameTick % 10 > 5;
			if (flag5)
			{
				g.setClip(83, 20, 2, 6);
				g.drawImage(GameScr.imgMPLost, 83, 20, 0);
				g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
			}
		}
	}

	// Token: 0x060004C8 RID: 1224 RVA: 0x00003E4C File Offset: 0x0000204C
	public void getInjure()
	{
	}

	// Token: 0x060004C9 RID: 1225 RVA: 0x0005EF40 File Offset: 0x0005D140
	public void starVS()
	{
		this.curr = (this.last = mSystem.currentTimeMillis());
		this.secondVS = 180;
	}

	// Token: 0x060004CA RID: 1226 RVA: 0x0005EF70 File Offset: 0x0005D170
	private global::Char findCharVS1()
	{
		for (int i = 0; i < GameScr.vCharInMap.size(); i++)
		{
			global::Char @char = (global::Char)GameScr.vCharInMap.elementAt(i);
			bool flag = @char.cTypePk != 0;
			if (flag)
			{
				return @char;
			}
		}
		return null;
	}

	// Token: 0x060004CB RID: 1227 RVA: 0x0005EFC4 File Offset: 0x0005D1C4
	private global::Char findCharVS2()
	{
		for (int i = 0; i < GameScr.vCharInMap.size(); i++)
		{
			global::Char @char = (global::Char)GameScr.vCharInMap.elementAt(i);
			bool flag = @char.cTypePk != 0 && @char != this.findCharVS1();
			if (flag)
			{
				return @char;
			}
		}
		return null;
	}

	// Token: 0x060004CC RID: 1228 RVA: 0x0005F028 File Offset: 0x0005D228
	private void paintInfoBar(mGraphics g)
	{
		GameScr.resetTranslate(g);
		bool flag = TileMap.mapID == 130 && this.findCharVS1() != null && this.findCharVS2() != null;
		if (flag)
		{
			g.translate(GameCanvas.w / 2 - 62, 0);
			this.paintImageBar(g, true, this.findCharVS1());
			g.translate(-(GameCanvas.w / 2 - 65), 0);
			this.paintImageBarRight(g, this.findCharVS2());
			this.findCharVS1().paintHeadWithXY(g, 137, 25, 0);
			this.findCharVS2().paintHeadWithXY(g, GameCanvas.w - 15 - 122, 25, 2);
		}
		else
		{
			bool flag2 = this.isVS() && global::Char.myCharz().charFocus != null;
			if (flag2)
			{
				g.translate(GameCanvas.w / 2 - 62, 0);
				this.paintImageBar(g, true, global::Char.myCharz().charFocus);
				g.translate(-(GameCanvas.w / 2 - 65), 0);
				this.paintImageBarRight(g, global::Char.myCharz());
				global::Char.myCharz().paintHeadWithXY(g, 137, 25, 0);
				global::Char.myCharz().charFocus.paintHeadWithXY(g, GameCanvas.w - 15 - 122, 25, 2);
			}
			else
			{
				bool flag3 = GameScr.ispaintPhubangBar() && GameScr.isSmallScr();
				if (flag3)
				{
					GameScr.paintHPBar_NEW(g, 1, 1, global::Char.myCharz());
				}
				else
				{
					this.paintImageBar(g, true, global::Char.myCharz());
					bool flag4 = global::Char.myCharz().isInEnterOfflinePoint() != null || global::Char.myCharz().isInEnterOnlinePoint() != null;
					if (flag4)
					{
						mFont.tahoma_7_green2.drawString(g, mResources.enter, this.imgScrW / 2, 8 + mGraphics.addYWhenOpenKeyBoard, mFont.CENTER);
					}
					else
					{
						bool flag5 = global::Char.myCharz().mobFocus != null;
						if (!flag5)
						{
							bool flag6 = global::Char.myCharz().npcFocus != null;
							if (!flag6)
							{
								bool flag7 = global::Char.myCharz().charFocus != null;
								if (flag7)
								{
								}
							}
						}
					}
				}
			}
		}
		g.translate(-g.getTranslateX(), -g.getTranslateY());
		bool flag8 = this.isVS() && this.secondVS > 0;
		if (flag8)
		{
			this.curr = mSystem.currentTimeMillis();
			bool flag9 = this.curr - this.last >= 1000L;
			if (flag9)
			{
				this.last = mSystem.currentTimeMillis();
				this.secondVS--;
			}
			mFont.tahoma_7b_white.drawString(g, this.secondVS.ToString() + string.Empty, GameCanvas.w / 2, 13, 2, mFont.tahoma_7b_dark);
		}
		bool flag10 = this.flareFindFocus;
		if (flag10)
		{
			g.drawImage(ItemMap.imageFlare, 40, 35, mGraphics.BOTTOM | mGraphics.HCENTER);
			this.flareTime--;
			bool flag11 = this.flareTime < 0;
			if (flag11)
			{
				this.flareTime = 0;
				this.flareFindFocus = false;
			}
		}
	}

	// Token: 0x060004CD RID: 1229 RVA: 0x0005F338 File Offset: 0x0005D538
	public bool isVS()
	{
		return TileMap.isVoDaiMap() && (global::Char.myCharz().cTypePk != 0 || (TileMap.mapID == 130 && this.findCharVS1() != null && this.findCharVS2() != null));
	}

	// Token: 0x060004CE RID: 1230 RVA: 0x0005F390 File Offset: 0x0005D590
	private void paintSelectedSkill(mGraphics g)
	{
		bool flag = this.mobCapcha != null;
		if (flag)
		{
			this.paintCapcha(g);
		}
		else
		{
			bool flag2 = GameCanvas.currentDialog != null || ChatPopup.currChatPopup != null || GameCanvas.menu.showMenu || this.isPaintPopup() || GameCanvas.panel.isShow || global::Char.myCharz().taskMaint.taskId == 0 || ChatTextField.gI().isShow || GameCanvas.currentScreen == MoneyCharge.instance;
			if (!flag2)
			{
				long num = mSystem.currentTimeMillis();
				long num2 = num - this.lastUsePotion;
				int num3 = 0;
				bool flag3 = num2 < 10000L;
				if (flag3)
				{
					num3 = (int)(num2 * 20L / 10000L);
				}
				bool flag4 = !GameCanvas.isTouch;
				if (flag4)
				{
					g.drawImage((mScreen.keyTouch != 10) ? GameScr.imgSkill : GameScr.imgSkill2, GameScr.xSkill + GameScr.xHP - 1, GameScr.yHP - 1, 0);
					SmallImage.drawSmallImage(g, 542, GameScr.xSkill + GameScr.xHP + 3, GameScr.yHP + 3, 0, 0);
					mFont.number_gray.drawString(g, string.Empty + GameScr.hpPotion.ToString(), GameScr.xSkill + GameScr.xHP + 22, GameScr.yHP + 15, 1);
					bool flag5 = num2 < 10000L;
					if (flag5)
					{
						g.setColor(2721889);
						num3 = (int)(num2 * 20L / 10000L);
						g.fillRect(GameScr.xSkill + GameScr.xHP + 3, GameScr.yHP + 3 + num3, 20, 20 - num3);
					}
				}
				else
				{
					bool flag6 = global::Char.myCharz().statusMe != 14;
					if (flag6)
					{
						bool isSmallGamePad = GameScr.gamePad.isSmallGamePad;
						if (isSmallGamePad)
						{
							bool flag7 = GameScr.isAnalog != 1;
							if (flag7)
							{
								g.setColor(9670800);
								g.fillRect(GameScr.xHP + 9, GameScr.yHP + 10 + 10, 22, 20);
								g.setColor(16777215);
								g.fillRect(GameScr.xHP + 9, GameScr.yHP + 10 + ((num3 != 0) ? (20 - num3) : 0) + 10, 22, (num3 == 0) ? 20 : num3);
								g.drawImage((mScreen.keyTouch != 10) ? GameScr.imgHP1 : GameScr.imgHP2, GameScr.xHP, GameScr.yHP + 10, 0);
								mFont.tahoma_7_red.drawString(g, string.Empty + GameScr.hpPotion.ToString(), GameScr.xHP + 20, GameScr.yHP + 15 + 10, 2);
								bool flag8 = GameScr.isPickNgocRong;
								if (flag8)
								{
									g.drawImage((mScreen.keyTouch != 14) ? GameScr.imgNR1 : GameScr.imgNR2, GameScr.xHP + 5, GameScr.yHP - 6 - 40 + 10, 0);
								}
								else
								{
									bool flag9 = GameScr.isudungCapsun4;
									if (flag9)
									{
										g.drawImage((mScreen.keyTouch != 14) ? GameScr.imgNutF : GameScr.imgNut, GameScr.xHP + 5, GameScr.yHP - 6 - 40 + 10, 0);
										SmallImage.drawSmallImage(g, 1088, GameScr.xHP - 7 + 5, GameScr.yHP - 6 - 40 - 7 + 10, 0, 0);
									}
									else
									{
										bool flag10 = GameScr.isudungCapsun3;
										if (flag10)
										{
											g.drawImage((mScreen.keyTouch != 14) ? GameScr.imgNutF : GameScr.imgNut, GameScr.xHP + 5, GameScr.yHP - 6 - 40 + 10, 0);
											SmallImage.drawSmallImage(g, 1087, GameScr.xHP - 7 + 5, GameScr.yHP - 6 - 40 - 7 + 10, 0, 0);
										}
									}
								}
							}
							else
							{
								bool flag11 = GameScr.isAnalog == 1;
								if (flag11)
								{
									int num4 = 10;
									g.drawImage((mScreen.keyTouch != 10) ? GameScr.imgSkill : GameScr.imgSkill2, GameScr.xSkill + GameScr.xHP - 1, GameScr.yHP - 1 + num4, 0);
									SmallImage.drawSmallImage(g, 542, GameScr.xSkill + GameScr.xHP + 3, GameScr.yHP + 3 + num4, 0, 0);
									mFont.number_gray.drawString(g, string.Empty + GameScr.hpPotion.ToString(), GameScr.xSkill + GameScr.xHP + 22, GameScr.yHP + 13 + num4, 1);
									bool flag12 = num2 < 10000L;
									if (flag12)
									{
										g.setColor(2721889);
										num3 = (int)(num2 * 20L / 10000L);
										g.fillRect(GameScr.xSkill + GameScr.xHP + 3, GameScr.yHP + 3 + num3 + num4, 20, 20 - num3);
									}
									bool flag13 = GameScr.isPickNgocRong;
									if (flag13)
									{
										g.drawImage((mScreen.keyTouch != 14) ? GameScr.imgNR3 : GameScr.imgNR4, GameScr.xHP + 20 + 5, GameScr.yHP + 20 - 6 - 40 + 10, mGraphics.HCENTER | mGraphics.VCENTER);
									}
									else
									{
										bool flag14 = GameScr.isudungCapsun4;
										if (flag14)
										{
											g.drawImage((mScreen.keyTouch != 14) ? GameScr.imgNut : GameScr.imgNutF, GameScr.xHP + 20 + 5, GameScr.yHP + 20 - 6 - 40 + 10, mGraphics.HCENTER | mGraphics.VCENTER);
											SmallImage.drawSmallImage(g, 1088, GameScr.xHP + 20 - 7 + 5, GameScr.yHP + 20 - 6 - 40 - 7 + 10, 0, 0);
										}
										else
										{
											bool flag15 = GameScr.isudungCapsun3;
											if (flag15)
											{
												g.drawImage((mScreen.keyTouch != 14) ? GameScr.imgNut : GameScr.imgNutF, GameScr.xHP + 20 + 5, GameScr.yHP + 20 - 6 - 40 + 10, mGraphics.HCENTER | mGraphics.VCENTER);
												SmallImage.drawSmallImage(g, 1087, GameScr.xHP + 20 - 7 + 5, GameScr.yHP + 20 - 6 - 40 - 7 + 10, 0, 0);
											}
										}
									}
								}
							}
						}
						else
						{
							bool flag16 = GameScr.isAnalog != 1;
							if (flag16)
							{
								g.setColor(9670800);
								g.fillRect(GameScr.xHP + 9, GameScr.yHP + 10 - 6, 22, 20);
								g.setColor(16777215);
								g.fillRect(GameScr.xHP + 9, GameScr.yHP + 10 + ((num3 != 0) ? (20 - num3) : 0) - 6, 22, (num3 == 0) ? 20 : num3);
								g.drawImage((mScreen.keyTouch != 10) ? GameScr.imgHP1 : GameScr.imgHP2, GameScr.xHP, GameScr.yHP - 6, 0);
								mFont.tahoma_7_red.drawString(g, string.Empty + GameScr.hpPotion.ToString(), GameScr.xHP + 20, GameScr.yHP + 15 - 6, 2);
								bool flag17 = GameScr.isPickNgocRong;
								if (flag17)
								{
									g.drawImage((mScreen.keyTouch != 14) ? GameScr.imgNR1 : GameScr.imgNR2, GameScr.xHP, GameScr.yHP - 6 - 40, 0);
								}
								else
								{
									bool flag18 = GameScr.isudungCapsun4;
									if (flag18)
									{
										g.drawImage((mScreen.keyTouch != 14) ? GameScr.imgNut : GameScr.imgNutF, GameScr.xHP + 20, GameScr.yHP + 20 - 6 - 40, mGraphics.HCENTER | mGraphics.VCENTER);
										SmallImage.drawSmallImage(g, 1088, GameScr.xHP + 20 - 7, GameScr.yHP + 20 - 6 - 40 - 7, 0, 0);
									}
									else
									{
										bool flag19 = GameScr.isudungCapsun3;
										if (flag19)
										{
											g.drawImage((mScreen.keyTouch != 14) ? GameScr.imgNut : GameScr.imgNutF, GameScr.xHP + 20, GameScr.yHP + 20 - 6 - 40, mGraphics.HCENTER | mGraphics.VCENTER);
											SmallImage.drawSmallImage(g, 1087, GameScr.xHP + 20 - 7, GameScr.yHP + 20 - 6 - 40 - 7, 0, 0);
										}
									}
								}
							}
							else
							{
								g.setColor(9670800);
								g.fillRect(GameScr.xHP + 10, GameScr.yHP + 10 - 6 + 10, 20, 18);
								g.setColor(16777215);
								g.fillRect(GameScr.xHP + 10, GameScr.yHP + 10 + ((num3 != 0) ? (20 - num3) : 0) - 6 + 10, 20, (num3 == 0) ? 18 : num3);
								g.drawImage((mScreen.keyTouch != 10) ? GameScr.imgHP3 : GameScr.imgHP4, GameScr.xHP + 20, GameScr.yHP + 20 - 6 + 10, mGraphics.HCENTER | mGraphics.VCENTER);
								mFont.tahoma_7_red.drawString(g, string.Empty + GameScr.hpPotion.ToString(), GameScr.xHP + 20, GameScr.yHP + 15 - 6 + 10, 2);
								bool flag20 = GameScr.isPickNgocRong;
								if (flag20)
								{
									g.drawImage((mScreen.keyTouch != 14) ? GameScr.imgNR3 : GameScr.imgNR4, GameScr.xHP + 20 + 5, GameScr.yHP + 20 - 6 - 40 + 10, mGraphics.HCENTER | mGraphics.VCENTER);
								}
								else
								{
									bool flag21 = GameScr.isudungCapsun4;
									if (flag21)
									{
										g.drawImage((mScreen.keyTouch != 14) ? GameScr.imgNut : GameScr.imgNutF, GameScr.xHP + 20 + 5, GameScr.yHP + 20 - 6 - 40 + 10, mGraphics.HCENTER | mGraphics.VCENTER);
										SmallImage.drawSmallImage(g, 1088, GameScr.xHP + 20 - 7 + 5, GameScr.yHP + 20 - 6 - 40 - 7 + 10, 0, 0);
									}
									else
									{
										bool flag22 = GameScr.isudungCapsun3;
										if (flag22)
										{
											g.drawImage((mScreen.keyTouch != 14) ? GameScr.imgNut : GameScr.imgNutF, GameScr.xHP + 20 + 5, GameScr.yHP + 20 - 6 - 40 + 10, mGraphics.HCENTER | mGraphics.VCENTER);
											SmallImage.drawSmallImage(g, 1087, GameScr.xHP + 20 - 7 + 5, GameScr.yHP + 20 - 6 - 40 - 7 + 10, 0, 0);
										}
									}
								}
							}
						}
					}
				}
				bool flag23 = GameScr.isHaveSelectSkill;
				if (flag23)
				{
					Skill[] array = (Main.isPC ? GameScr.keySkill : ((!GameCanvas.isTouch) ? GameScr.keySkill : GameScr.onScreenSkill));
					bool flag24 = mScreen.keyTouch == 10;
					if (flag24)
					{
					}
					bool flag25 = !GameCanvas.isTouch;
					if (flag25)
					{
						g.setColor(11152401);
						g.fillRect(GameScr.xSkill + GameScr.xHP + 2, GameScr.yHP - 10 + 6, 20, 10);
						mFont.tahoma_7_white.drawString(g, "*", GameScr.xSkill + GameScr.xHP + 12, GameScr.yHP - 8 + 6, mFont.CENTER);
					}
					int num5 = (Main.isPC ? array.Length : ((!GameCanvas.isTouch) ? array.Length : this.nSkill));
					for (int i = 0; i < num5; i++)
					{
						bool isPC = Main.isPC;
						if (isPC)
						{
							string[] array3;
							if (!TField.isQwerty)
							{
								string[] array2 = new string[5];
								array2[0] = "";
								array2[1] = "";
								array2[2] = "";
								array2[3] = "";
								array3 = array2;
								array2[4] = "";
							}
							else
							{
								string[] array4 = new string[10];
								array4[0] = "";
								array4[1] = "";
								array4[2] = "";
								array4[3] = "";
								array4[4] = "";
								array4[5] = "";
								array4[6] = "";
								array4[7] = "";
								array4[8] = "";
								array3 = array4;
								array4[9] = "";
							}
							string[] array5 = array3;
							int num6 = -13;
							bool flag26 = num5 > 5 && i < 5;
							if (flag26)
							{
								num6 = 27;
							}
							mFont.tahoma_7b_dark.drawString(g, array5[i], GameScr.xSkill + GameScr.xS[i] + 14, GameScr.yS[i] + num6, mFont.CENTER);
							mFont.tahoma_7b_white.drawString(g, array5[i], GameScr.xSkill + GameScr.xS[i] + 14, GameScr.yS[i] + num6 + 1, mFont.CENTER);
						}
						else
						{
							bool flag27 = !GameCanvas.isTouch;
							if (flag27)
							{
								string[] array7;
								if (!TField.isQwerty)
								{
									string[] array6 = new string[5];
									array6[0] = "7";
									array6[1] = "8";
									array6[2] = "9";
									array6[3] = "1";
									array7 = array6;
									array6[4] = "3";
								}
								else
								{
									string[] array8 = new string[5];
									array8[0] = "Q";
									array8[1] = "W";
									array8[2] = "E";
									array8[3] = "R";
									array7 = array8;
									array8[4] = "T";
								}
								string[] array9 = array7;
								g.setColor(11152401);
								g.fillRect(GameScr.xSkill + GameScr.xS[i] + 2, GameScr.yS[i] - 10 + 8, 20, 10);
								mFont.tahoma_7_white.drawString(g, array9[i], GameScr.xSkill + GameScr.xS[i] + 12, GameScr.yS[i] - 10 + 6, mFont.CENTER);
							}
						}
						Skill skill = array[i];
						bool flag28 = skill != global::Char.myCharz().myskill;
						if (flag28)
						{
							g.drawImage(GameScr.imgSkill, GameScr.xSkill + GameScr.xS[i] - 1, GameScr.yS[i] - 1, 0);
						}
						bool flag29 = skill == null;
						if (!flag29)
						{
							bool flag30 = skill == global::Char.myCharz().myskill;
							if (flag30)
							{
								g.drawImage(GameScr.imgSkill2, GameScr.xSkill + GameScr.xS[i] - 1, GameScr.yS[i] - 1, 0);
								bool flag31 = GameCanvas.isTouch && !Main.isPC;
								if (flag31)
								{
									g.drawRegion(Mob.imgHP, 0, 12, 9, 6, 0, GameScr.xSkill + GameScr.xS[i] + 8, GameScr.yS[i] - 7, 0);
								}
							}
							skill.paint(GameScr.xSkill + GameScr.xS[i] + 13, GameScr.yS[i] + 13, g);
							bool flag32 = (i == this.selectedIndexSkill && !this.isPaintUI() && GameCanvas.gameTick % 10 > 5) || i == this.keyTouchSkill;
							if (flag32)
							{
								g.drawImage(ItemMap.imageFlare, GameScr.xSkill + GameScr.xS[i] + 13, GameScr.yS[i] + 14, 3);
							}
							long num7 = (long)skill.coolDown - mSystem.currentTimeMillis() + skill.lastTimeUseThisSkill;
							bool flag33 = skill.template.id == 7;
							if (flag33)
							{
								GameScr.timehoichieubuff = (long)skill.coolDown - mSystem.currentTimeMillis() + skill.lastTimeUseThisSkill;
							}
							bool flag34 = skill.template.id == 19;
							if (flag34)
							{
								GameScr.timehoikhien = (long)skill.coolDown - mSystem.currentTimeMillis() + skill.lastTimeUseThisSkill;
							}
							bool flag35 = skill.template.id == 8;
							if (flag35)
							{
								GameScr.timehoiskill3 = (long)skill.coolDown - mSystem.currentTimeMillis() + skill.lastTimeUseThisSkill;
							}
							bool flag36 = skill.template.id == 1;
							if (flag36)
							{
								GameScr.ccc1 = (long)skill.coolDown - mSystem.currentTimeMillis() + skill.lastTimeUseThisSkill;
							}
							bool flag37 = skill.template.id == 3;
							if (flag37)
							{
								GameScr.ccc3 = (long)skill.coolDown - mSystem.currentTimeMillis() + skill.lastTimeUseThisSkill;
							}
							bool flag38 = skill.template.id == 5;
							if (flag38)
							{
								GameScr.ccc5 = (long)skill.coolDown - mSystem.currentTimeMillis() + skill.lastTimeUseThisSkill;
							}
							bool flag39 = skill.template.id == 14;
							if (flag39)
							{
								GameScr.timehoibom = (long)skill.coolDown - mSystem.currentTimeMillis() + skill.lastTimeUseThisSkill;
							}
							bool flag40 = skill.template.id == 22;
							if (flag40)
							{
								GameScr.timehoithoimien = (long)skill.coolDown - mSystem.currentTimeMillis() + skill.lastTimeUseThisSkill;
							}
							bool flag41 = skill.template.id == 24 || skill.template.id == 25 || skill.template.id == 26;
							if (flag41)
							{
								GameScr.timehoiskill9 = (long)skill.coolDown - mSystem.currentTimeMillis() + skill.lastTimeUseThisSkill;
							}
							mFont.tahoma_7b_white.drawString(g, (num7 > 0L) ? string.Concat(num7 / 1000L) : string.Empty, GameScr.xSkill + GameScr.xS[i] + 14, GameScr.yS[i] + 8, mFont.CENTER, mFont.tahoma_7b_red);
						}
					}
				}
				this.paintGamePad(g);
			}
		}
	}

	// Token: 0x060004CF RID: 1231 RVA: 0x0006047C File Offset: 0x0005E67C
	public void paintOpen(mGraphics g)
	{
		bool flag = this.isstarOpen;
		if (flag)
		{
			g.translate(-g.getTranslateX(), -g.getTranslateY());
			g.fillRect(0, 0, GameCanvas.w, this.moveUp);
			g.setColor(10275899);
			g.fillRect(0, this.moveUp - 1, GameCanvas.w, 1);
			g.fillRect(0, this.moveDow + 1, GameCanvas.w, 1);
		}
	}

	// Token: 0x060004D0 RID: 1232 RVA: 0x000604F8 File Offset: 0x0005E6F8
	public static void startFlyText(string flyString, int x, int y, int dx, int dy, int color)
	{
		int num = -1;
		for (int i = 0; i < 5; i++)
		{
			bool flag = GameScr.flyTextState[i] == -1;
			if (flag)
			{
				num = i;
				break;
			}
		}
		bool flag2 = num == -1;
		if (!flag2)
		{
			GameScr.flyTextColor[num] = color;
			GameScr.flyTextString[num] = flyString;
			GameScr.flyTextX[num] = x;
			GameScr.flyTextY[num] = y;
			GameScr.flyTextDx[num] = dx;
			GameScr.flyTextDy[num] = ((dy >= 0) ? 5 : (-5));
			GameScr.flyTextState[num] = 0;
			GameScr.flyTime[num] = 0;
			GameScr.flyTextYTo[num] = 10;
			for (int j = 0; j < 5; j++)
			{
				bool flag3 = GameScr.flyTextState[j] != -1 && num != j && GameScr.flyTextDy[num] < 0 && Res.abs(GameScr.flyTextX[num] - GameScr.flyTextX[j]) <= 20 && GameScr.flyTextYTo[num] == GameScr.flyTextYTo[j];
				if (flag3)
				{
					GameScr.flyTextYTo[num] += 10;
				}
			}
		}
	}

	// Token: 0x060004D1 RID: 1233 RVA: 0x00060608 File Offset: 0x0005E808
	public static void updateFlyText()
	{
		for (int i = 0; i < 5; i++)
		{
			bool flag = GameScr.flyTextState[i] == -1;
			if (!flag)
			{
				bool flag2 = GameScr.flyTextState[i] > GameScr.flyTextYTo[i];
				if (flag2)
				{
					GameScr.flyTime[i]++;
					bool flag3 = GameScr.flyTime[i] == 25;
					if (flag3)
					{
						GameScr.flyTime[i] = 0;
						GameScr.flyTextState[i] = -1;
						GameScr.flyTextYTo[i] = 0;
						GameScr.flyTextDx[i] = 0;
						GameScr.flyTextX[i] = 0;
					}
				}
				else
				{
					GameScr.flyTextState[i] += Res.abs(GameScr.flyTextDy[i]);
					GameScr.flyTextX[i] += GameScr.flyTextDx[i];
					GameScr.flyTextY[i] += GameScr.flyTextDy[i];
				}
			}
		}
	}

	// Token: 0x060004D2 RID: 1234 RVA: 0x000606F0 File Offset: 0x0005E8F0
	public static void loadSplash()
	{
		bool flag = GameScr.imgSplash == null;
		if (flag)
		{
			GameScr.imgSplash = new Image[3];
			for (int i = 0; i < 3; i++)
			{
				GameScr.imgSplash[i] = GameCanvas.loadImage("/e/sp" + i.ToString() + ".png");
			}
		}
		GameScr.splashX = new int[2];
		GameScr.splashY = new int[2];
		GameScr.splashState = new int[2];
		GameScr.splashF = new int[2];
		GameScr.splashDir = new int[2];
		GameScr.splashState[0] = (GameScr.splashState[1] = -1);
	}

	// Token: 0x060004D3 RID: 1235 RVA: 0x00060794 File Offset: 0x0005E994
	public static bool startSplash(int x, int y, int dir)
	{
		int num = ((GameScr.splashState[0] != -1) ? 1 : 0);
		bool flag = GameScr.splashState[num] != -1;
		bool flag2;
		if (flag)
		{
			flag2 = false;
		}
		else
		{
			GameScr.splashState[num] = 0;
			GameScr.splashDir[num] = dir;
			GameScr.splashX[num] = x;
			GameScr.splashY[num] = y;
			flag2 = true;
		}
		return flag2;
	}

	// Token: 0x060004D4 RID: 1236 RVA: 0x000607EC File Offset: 0x0005E9EC
	public static void updateSplash()
	{
		for (int i = 0; i < 2; i++)
		{
			bool flag = GameScr.splashState[i] != -1;
			if (flag)
			{
				GameScr.splashState[i]++;
				GameScr.splashX[i] += GameScr.splashDir[i] << 2;
				GameScr.splashY[i]--;
				bool flag2 = GameScr.splashState[i] >= 6;
				if (flag2)
				{
					GameScr.splashState[i] = -1;
				}
				else
				{
					GameScr.splashF[i] = (GameScr.splashState[i] >> 1) % 3;
				}
			}
		}
	}

	// Token: 0x060004D5 RID: 1237 RVA: 0x00060890 File Offset: 0x0005EA90
	public static void paintSplash(mGraphics g)
	{
		for (int i = 0; i < 2; i++)
		{
			bool flag = GameScr.splashState[i] != -1;
			if (flag)
			{
				bool flag2 = GameScr.splashDir[i] == 1;
				if (flag2)
				{
					g.drawImage(GameScr.imgSplash[GameScr.splashF[i]], GameScr.splashX[i], GameScr.splashY[i], 3);
				}
				else
				{
					g.drawRegion(GameScr.imgSplash[GameScr.splashF[i]], 0, 0, mGraphics.getImageWidth(GameScr.imgSplash[GameScr.splashF[i]]), mGraphics.getImageHeight(GameScr.imgSplash[GameScr.splashF[i]]), 2, GameScr.splashX[i], GameScr.splashY[i], 3);
				}
			}
		}
	}

	// Token: 0x060004D6 RID: 1238 RVA: 0x00004D9A File Offset: 0x00002F9A
	private void loadInforBar()
	{
		this.imgScrW = 84;
		GameScr.hpBarW = 66L;
		GameScr.mpBarW = 59;
		GameScr.hpBarX = 52;
		GameScr.hpBarY = 10;
		GameScr.spBarW = 61;
		GameScr.expBarW = GameScr.gW - 61;
	}

	// Token: 0x060004D7 RID: 1239 RVA: 0x00060950 File Offset: 0x0005EB50
	public void updateSS()
	{
		bool flag = GameScr.indexMenu != -1;
		if (flag)
		{
			bool flag2 = GameScr.cmySK != GameScr.cmtoYSK;
			if (flag2)
			{
				GameScr.cmvySK = GameScr.cmtoYSK - GameScr.cmySK << 2;
				GameScr.cmdySK += GameScr.cmvySK;
				GameScr.cmySK += GameScr.cmdySK >> 4;
				GameScr.cmdySK &= 15;
			}
			bool flag3 = global::Math.abs(GameScr.cmtoYSK - GameScr.cmySK) < 15 && GameScr.cmySK < 0;
			if (flag3)
			{
				GameScr.cmtoYSK = 0;
			}
			bool flag4 = global::Math.abs(GameScr.cmtoYSK - GameScr.cmySK) < 15 && GameScr.cmySK > GameScr.cmyLimSK;
			if (flag4)
			{
				GameScr.cmtoYSK = GameScr.cmyLimSK;
			}
		}
	}

	// Token: 0x060004D8 RID: 1240 RVA: 0x00060A28 File Offset: 0x0005EC28
	public void updateKeyAlert()
	{
		bool flag = !GameScr.isPaintAlert || GameCanvas.currentDialog != null;
		if (!flag)
		{
			bool flag2 = false;
			bool flag3 = GameCanvas.keyPressed[Key.NUM8];
			if (flag3)
			{
				GameScr.indexRow++;
				bool flag4 = GameScr.indexRow >= this.texts.size();
				if (flag4)
				{
					GameScr.indexRow = 0;
				}
				flag2 = true;
			}
			else
			{
				bool flag5 = GameCanvas.keyPressed[Key.NUM2];
				if (flag5)
				{
					GameScr.indexRow--;
					bool flag6 = GameScr.indexRow < 0;
					if (flag6)
					{
						GameScr.indexRow = this.texts.size() - 1;
					}
					flag2 = true;
				}
			}
			bool flag7 = flag2;
			if (flag7)
			{
				GameScr.scrMain.moveTo(GameScr.indexRow * GameScr.scrMain.ITEM_SIZE);
				GameCanvas.clearKeyHold();
				GameCanvas.clearKeyPressed();
			}
			bool isTouch = GameCanvas.isTouch;
			if (isTouch)
			{
				ScrollResult scrollResult = GameScr.scrMain.updateKey();
				bool flag8 = scrollResult.isDowning || scrollResult.isFinish;
				if (flag8)
				{
					GameScr.indexRow = scrollResult.selected;
					flag2 = true;
				}
			}
			bool flag9 = !flag2 || GameScr.indexRow < 0 || GameScr.indexRow >= this.texts.size();
			if (!flag9)
			{
				string text = (string)this.texts.elementAt(GameScr.indexRow);
				this.fnick = null;
				this.alertURL = null;
				this.center = null;
				ChatTextField.gI().center = null;
				int num;
				bool flag10 = (num = text.IndexOf("http://")) >= 0;
				if (flag10)
				{
					Cout.println("currentLine: " + text);
					this.alertURL = text.Substring(num);
					this.center = new Command(mResources.open_link, 12000);
					bool flag11 = !GameCanvas.isTouch;
					if (flag11)
					{
						ChatTextField.gI().center = new Command(mResources.open_link, null, 12000, null);
					}
				}
				else
				{
					bool flag12 = text.IndexOf("@") < 0;
					if (!flag12)
					{
						string text2 = text.Substring(2);
						text2 = text2.Trim();
						num = text2.IndexOf("@");
						string text3 = text2.Substring(num);
						int num2 = text3.IndexOf(" ");
						num2 = ((num2 > 0) ? (num2 + num) : (num + text3.Length));
						this.fnick = text2.Substring(num + 1, num2);
						bool flag13 = !this.fnick.Equals(string.Empty) && !this.fnick.Equals(global::Char.myCharz().cName);
						if (flag13)
						{
							this.center = new Command(mResources.SELECT, 12009, this.fnick);
							bool flag14 = !GameCanvas.isTouch;
							if (flag14)
							{
								ChatTextField.gI().center = new Command(mResources.SELECT, null, 12009, this.fnick);
							}
						}
						else
						{
							this.fnick = null;
							this.center = null;
						}
					}
				}
			}
		}
	}

	// Token: 0x060004D9 RID: 1241 RVA: 0x00060D48 File Offset: 0x0005EF48
	public bool isPaintPopup()
	{
		return GameScr.isPaintItemInfo || GameScr.isPaintInfoMe || GameScr.isPaintStore || GameScr.isPaintWeapon || GameScr.isPaintNonNam || GameScr.isPaintNonNu || GameScr.isPaintAoNam || GameScr.isPaintAoNu || GameScr.isPaintGangTayNam || GameScr.isPaintGangTayNu || GameScr.isPaintQuanNam || GameScr.isPaintQuanNu || GameScr.isPaintGiayNam || GameScr.isPaintGiayNu || GameScr.isPaintLien || GameScr.isPaintNhan || GameScr.isPaintNgocBoi || GameScr.isPaintPhu || GameScr.isPaintStack || GameScr.isPaintStackLock || GameScr.isPaintGrocery || GameScr.isPaintGroceryLock || GameScr.isPaintUpGrade || GameScr.isPaintConvert || GameScr.isPaintSplit || GameScr.isPaintUpPearl || GameScr.isPaintBox || GameScr.isPaintTrade || GameScr.isPaintAlert || GameScr.isPaintZone || GameScr.isPaintTeam || GameScr.isPaintClan || GameScr.isPaintFindTeam || GameScr.isPaintTask || GameScr.isPaintFriend || GameScr.isPaintEnemies || GameScr.isPaintCharInMap || GameScr.isPaintMessage;
	}

	// Token: 0x060004DA RID: 1242 RVA: 0x00060EA8 File Offset: 0x0005F0A8
	public bool isNotPaintTouchControl()
	{
		bool flag = !GameCanvas.isTouchControl && GameCanvas.currentScreen == GameScr.gI();
		bool flag2;
		if (flag)
		{
			flag2 = true;
		}
		else
		{
			bool flag3 = !GameCanvas.isTouch;
			if (flag3)
			{
				flag2 = true;
			}
			else
			{
				bool isShow = ChatTextField.gI().isShow;
				if (isShow)
				{
					flag2 = true;
				}
				else
				{
					bool isShow2 = InfoDlg.isShow;
					if (isShow2)
					{
						flag2 = true;
					}
					else
					{
						bool flag4 = GameCanvas.currentDialog != null || ChatPopup.currChatPopup != null || GameCanvas.menu.showMenu || GameCanvas.panel.isShow || this.isPaintPopup();
						flag2 = flag4;
					}
				}
			}
		}
		return flag2;
	}

	// Token: 0x060004DB RID: 1243 RVA: 0x00060F48 File Offset: 0x0005F148
	public bool isPaintUI()
	{
		return GameScr.isPaintStore || GameScr.isPaintWeapon || GameScr.isPaintNonNam || GameScr.isPaintNonNu || GameScr.isPaintAoNam || GameScr.isPaintAoNu || GameScr.isPaintGangTayNam || GameScr.isPaintGangTayNu || GameScr.isPaintQuanNam || GameScr.isPaintQuanNu || GameScr.isPaintGiayNam || GameScr.isPaintGiayNu || GameScr.isPaintLien || GameScr.isPaintNhan || GameScr.isPaintNgocBoi || GameScr.isPaintPhu || GameScr.isPaintStack || GameScr.isPaintStackLock || GameScr.isPaintGrocery || GameScr.isPaintGroceryLock || GameScr.isPaintUpGrade || GameScr.isPaintConvert || GameScr.isPaintSplit || GameScr.isPaintUpPearl || GameScr.isPaintBox || GameScr.isPaintTrade;
	}

	// Token: 0x060004DC RID: 1244 RVA: 0x00061030 File Offset: 0x0005F230
	public bool isOpenUI()
	{
		return GameScr.isPaintItemInfo || GameScr.isPaintInfoMe || GameScr.isPaintStore || GameScr.isPaintNonNam || GameScr.isPaintNonNu || GameScr.isPaintAoNam || GameScr.isPaintAoNu || GameScr.isPaintGangTayNam || GameScr.isPaintGangTayNu || GameScr.isPaintQuanNam || GameScr.isPaintQuanNu || GameScr.isPaintGiayNam || GameScr.isPaintGiayNu || GameScr.isPaintLien || GameScr.isPaintNhan || GameScr.isPaintNgocBoi || GameScr.isPaintPhu || GameScr.isPaintWeapon || GameScr.isPaintStack || GameScr.isPaintStackLock || GameScr.isPaintGrocery || GameScr.isPaintGroceryLock || GameScr.isPaintUpGrade || GameScr.isPaintConvert || GameScr.isPaintUpPearl || GameScr.isPaintBox || GameScr.isPaintSplit || GameScr.isPaintTrade;
	}

	// Token: 0x060004DD RID: 1245 RVA: 0x0006112C File Offset: 0x0005F32C
	public static void setPopupSize(int w, int h)
	{
		bool flag = GameCanvas.w == 128 || GameCanvas.h <= 208;
		if (flag)
		{
			w = 126;
			h = 160;
		}
		GameScr.indexTitle = 0;
		GameScr.popupW = w;
		GameScr.popupH = h;
		GameScr.popupX = GameScr.gW2 - w / 2;
		GameScr.popupY = GameScr.gH2 - h / 2;
		bool flag2 = GameCanvas.isTouch && !GameScr.isPaintZone && !GameScr.isPaintTeam && !GameScr.isPaintClan && !GameScr.isPaintCharInMap && !GameScr.isPaintFindTeam && !GameScr.isPaintFriend && !GameScr.isPaintEnemies && !GameScr.isPaintTask && !GameScr.isPaintMessage;
		if (flag2)
		{
			bool flag3 = GameCanvas.h <= 240;
			if (flag3)
			{
				GameScr.popupY -= 10;
			}
			bool flag4 = GameCanvas.isTouch && !GameCanvas.isTouchControlSmallScreen && GameCanvas.currentScreen is GameScr;
			if (flag4)
			{
				GameScr.popupW = 310;
				GameScr.popupX = GameScr.gW / 2 - GameScr.popupW / 2;
				bool flag5 = GameScr.isPaintInfoMe && GameScr.indexMenu > 0;
				if (flag5)
				{
					GameScr.popupW = w;
					GameScr.popupX = GameScr.gW2 - w / 2;
				}
			}
		}
		bool flag6 = GameScr.popupY < -10;
		if (flag6)
		{
			GameScr.popupY = -10;
		}
		bool flag7 = GameCanvas.h > 208 && GameScr.popupY < 0;
		if (flag7)
		{
			GameScr.popupY = 0;
		}
		bool flag8 = GameCanvas.h == 208 && GameScr.popupY < 10;
		if (flag8)
		{
			GameScr.popupY = 10;
		}
	}

	// Token: 0x060004DE RID: 1246 RVA: 0x00004DD6 File Offset: 0x00002FD6
	public static void loadImg()
	{
		TileMap.loadTileImage();
	}

	// Token: 0x060004DF RID: 1247 RVA: 0x000612E0 File Offset: 0x0005F4E0
	public void paintTitle(mGraphics g, string title, bool arrow)
	{
		int num = GameScr.gW / 2;
		g.setColor(Paint.COLORDARK);
		g.fillRoundRect(num - mFont.tahoma_8b.getWidth(title) / 2 - 12, GameScr.popupY + 4, mFont.tahoma_8b.getWidth(title) + 22, 24, 6, 6);
		bool flag = (GameScr.indexTitle == 0 || GameCanvas.isTouch) && arrow;
		if (flag)
		{
			SmallImage.drawSmallImage(g, 989, num - mFont.tahoma_8b.getWidth(title) / 2 - 15 - 7 - ((GameCanvas.gameTick % 8 <= 3) ? 2 : 0), GameScr.popupY + 16, 2, StaticObj.VCENTER_HCENTER);
			SmallImage.drawSmallImage(g, 989, num + mFont.tahoma_8b.getWidth(title) / 2 + 15 + 5 + ((GameCanvas.gameTick % 8 <= 3) ? 2 : 0), GameScr.popupY + 16, 0, StaticObj.VCENTER_HCENTER);
		}
		bool flag2 = GameScr.indexTitle == 0;
		if (flag2)
		{
			g.setColor(Paint.COLORFOCUS);
		}
		else
		{
			g.setColor(Paint.COLORBORDER);
		}
		g.drawRoundRect(num - mFont.tahoma_8b.getWidth(title) / 2 - 12, GameScr.popupY + 4, mFont.tahoma_8b.getWidth(title) + 22, 24, 6, 6);
		mFont.tahoma_8b.drawString(g, title, num, GameScr.popupY + 9, 2);
	}

	// Token: 0x060004E0 RID: 1248 RVA: 0x0006143C File Offset: 0x0005F63C
	public static int getTaskMapId()
	{
		bool flag = global::Char.myCharz().taskMaint == null;
		int num;
		if (flag)
		{
			num = -1;
		}
		else
		{
			num = GameScr.mapTasks[global::Char.myCharz().taskMaint.index];
		}
		return num;
	}

	// Token: 0x060004E1 RID: 1249 RVA: 0x0006147C File Offset: 0x0005F67C
	public static sbyte getTaskNpcId()
	{
		sbyte b = 0;
		bool flag = global::Char.myCharz().taskMaint == null;
		if (flag)
		{
			b = -1;
		}
		else
		{
			bool flag2 = global::Char.myCharz().taskMaint.index <= GameScr.tasks.Length - 1;
			if (flag2)
			{
				b = (sbyte)GameScr.tasks[global::Char.myCharz().taskMaint.index];
			}
		}
		return b;
	}

	// Token: 0x060004E2 RID: 1250 RVA: 0x00003E4C File Offset: 0x0000204C
	public void refreshTeam()
	{
	}

	// Token: 0x060004E3 RID: 1251 RVA: 0x000614E4 File Offset: 0x0005F6E4
	public void onChatFromMe(string text, string to)
	{
		Res.outz("CHAT");
		bool flag = !GameScr.isPaintMessage || GameCanvas.isTouch;
		if (flag)
		{
			ChatTextField.gI().isShow = false;
		}
		bool flag2 = to.Equals(mResources.chat_player);
		if (flag2)
		{
			bool flag3 = GameScr.info2.playerID != global::Char.myCharz().charID;
			if (flag3)
			{
				Service.gI().chatPlayer(text, GameScr.info2.playerID);
			}
		}
		else
		{
			bool flag4 = !text.Equals(string.Empty);
			if (flag4)
			{
				Service.gI().chat(text);
			}
		}
	}

	// Token: 0x060004E4 RID: 1252 RVA: 0x00061588 File Offset: 0x0005F788
	public void onCancelChat()
	{
		bool flag = GameScr.isPaintMessage;
		if (flag)
		{
			GameScr.isPaintMessage = false;
			ChatTextField.gI().center = null;
		}
	}

	// Token: 0x060004E5 RID: 1253 RVA: 0x000615B4 File Offset: 0x0005F7B4
	public void openWeb(string strLeft, string strRight, string url, string title, string str)
	{
		GameScr.isPaintAlert = true;
		this.isLockKey = true;
		GameScr.indexRow = 0;
		GameScr.setPopupSize(175, 200);
		this.textsTitle = title;
		this.texts = mFont.tahoma_7.splitFontVector(str, GameScr.popupW - 30);
		this.center = null;
		this.left = new Command(strLeft, 11068, url);
		this.right = new Command(strRight, 11069);
	}

	// Token: 0x060004E6 RID: 1254 RVA: 0x00061634 File Offset: 0x0005F834
	public void sendSms(string strLeft, string strRight, short port, string syntax, string title, string str)
	{
		GameScr.isPaintAlert = true;
		this.isLockKey = true;
		GameScr.indexRow = 0;
		GameScr.setPopupSize(175, 200);
		this.textsTitle = title;
		this.texts = mFont.tahoma_7.splitFontVector(str, GameScr.popupW - 30);
		this.center = null;
		MyVector myVector = new MyVector();
		myVector.addElement(string.Empty + port.ToString());
		myVector.addElement(syntax);
		this.left = new Command(strLeft, 11074);
		this.right = new Command(strRight, 11075);
	}

	// Token: 0x060004E7 RID: 1255 RVA: 0x00004DDF File Offset: 0x00002FDF
	public void actMenu()
	{
		GameCanvas.panel.setTypeMain();
		GameCanvas.panel.show();
	}

	// Token: 0x060004E8 RID: 1256 RVA: 0x000616D8 File Offset: 0x0005F8D8
	public void openUIZone(Message message)
	{
		InfoDlg.hide();
		try
		{
			this.zones = new int[(int)message.reader().readByte()];
			this.pts = new int[this.zones.Length];
			this.numPlayer = new int[this.zones.Length];
			this.maxPlayer = new int[this.zones.Length];
			this.rank1 = new int[this.zones.Length];
			this.rankName1 = new string[this.zones.Length];
			this.rank2 = new int[this.zones.Length];
			this.rankName2 = new string[this.zones.Length];
			for (int i = 0; i < this.zones.Length; i++)
			{
				this.zones[i] = (int)message.reader().readByte();
				this.pts[i] = (int)message.reader().readByte();
				this.numPlayer[i] = (int)message.reader().readByte();
				this.maxPlayer[i] = (int)message.reader().readByte();
				sbyte b = message.reader().readByte();
				bool flag = b == 1;
				if (flag)
				{
					this.rankName1[i] = message.reader().readUTF();
					this.rank1[i] = message.reader().readInt();
					this.rankName2[i] = message.reader().readUTF();
					this.rank2[i] = message.reader().readInt();
				}
			}
		}
		catch (Exception ex)
		{
			Cout.LogError("Loi ham OPEN UIZONE " + ex.ToString());
		}
	}

	// Token: 0x060004E9 RID: 1257 RVA: 0x00061894 File Offset: 0x0005FA94
	public void openUIZoneDemo(Message message)
	{
		InfoDlg.hide();
		try
		{
			this.zones = new int[(int)message.reader().readByte()];
			this.pts = new int[this.zones.Length];
			this.numPlayer = new int[this.zones.Length];
			this.maxPlayer = new int[this.zones.Length];
			this.rank1 = new int[this.zones.Length];
			this.rankName1 = new string[this.zones.Length];
			this.rank2 = new int[this.zones.Length];
			this.rankName2 = new string[this.zones.Length];
			for (int i = 0; i < this.zones.Length; i++)
			{
				this.zones[i] = (int)message.reader().readByte();
				this.pts[i] = (int)message.reader().readByte();
				this.numPlayer[i] = (int)message.reader().readByte();
				this.maxPlayer[i] = (int)message.reader().readByte();
				sbyte b = message.reader().readByte();
				bool flag = b == 1;
				if (flag)
				{
					this.rankName1[i] = message.reader().readUTF();
					this.rank1[i] = message.reader().readInt();
					this.rankName2[i] = message.reader().readUTF();
					this.rank2[i] = message.reader().readInt();
				}
			}
		}
		catch (Exception ex)
		{
			Cout.LogError("Loi ham OPEN UIZONE " + ex.ToString());
		}
		GameCanvas.panel.setTypeZone();
		GameCanvas.panel.show();
	}

	// Token: 0x060004EA RID: 1258 RVA: 0x00004DF8 File Offset: 0x00002FF8
	public void showViewInfo()
	{
		GameScr.indexMenu = 3;
		GameScr.isPaintInfoMe = true;
		GameScr.setPopupSize(175, 200);
	}

	// Token: 0x060004EB RID: 1259 RVA: 0x00061A64 File Offset: 0x0005FC64
	private void actDead()
	{
		MyVector myVector = new MyVector();
		myVector.addElement(new Command(mResources.DIES[1], 110381));
		myVector.addElement(new Command(mResources.DIES[2], 110382));
		myVector.addElement(new Command(mResources.DIES[3], 110383));
		GameCanvas.menu.startAt(myVector, 3);
	}

	// Token: 0x060004EC RID: 1260 RVA: 0x00004E17 File Offset: 0x00003017
	public void startYesNoPopUp(string info, Command cmdYes, Command cmdNo)
	{
		this.popUpYesNo = new PopUpYesNo();
		this.popUpYesNo.setPopUp(info, cmdYes, cmdNo);
	}

	// Token: 0x060004ED RID: 1261 RVA: 0x00061AD0 File Offset: 0x0005FCD0
	public void player_vs_player(int playerId, int xu, string info, sbyte typePK)
	{
		global::Char @char = GameScr.findCharInMap(playerId);
		bool flag = @char != null;
		if (flag)
		{
			bool flag2 = typePK == 3;
			if (flag2)
			{
				this.startYesNoPopUp(info, new Command(mResources.OK, 2000, @char), new Command(mResources.CLOSE, 2009, @char));
			}
			bool flag3 = typePK == 4;
			if (flag3)
			{
				this.startYesNoPopUp(info, new Command(mResources.OK, 2005, @char), new Command(mResources.CLOSE, 2009, @char));
			}
		}
	}

	// Token: 0x060004EE RID: 1262 RVA: 0x00061B58 File Offset: 0x0005FD58
	public void giaodich(int playerID)
	{
		global::Char @char = GameScr.findCharInMap(playerID);
		bool flag = @char != null;
		if (flag)
		{
			this.startYesNoPopUp(@char.cName + mResources.want_to_trade, new Command(mResources.YES, 11114, @char), new Command(mResources.NO, 2009, @char));
		}
	}

	// Token: 0x060004EF RID: 1263 RVA: 0x00061BB0 File Offset: 0x0005FDB0
	public void getFlagImage(int charID, sbyte cflag)
	{
		bool flag = GameScr.vFlag.size() == 0;
		if (flag)
		{
			Service.gI().getFlag(2, cflag);
			Res.outz("getFlag1");
		}
		else
		{
			bool flag2 = charID == global::Char.myCharz().charID;
			if (flag2)
			{
				Res.outz("my cflag: isme");
				bool flag3 = global::Char.myCharz().isGetFlagImage(cflag);
				if (flag3)
				{
					Res.outz("my cflag: true");
					for (int i = 0; i < GameScr.vFlag.size(); i++)
					{
						PKFlag pkflag = (PKFlag)GameScr.vFlag.elementAt(i);
						bool flag4 = pkflag != null && pkflag.cflag == cflag;
						if (flag4)
						{
							Res.outz("my cflag: cflag==");
							global::Char.myCharz().flagImage = pkflag.IDimageFlag;
						}
					}
				}
				else
				{
					bool flag5 = !global::Char.myCharz().isGetFlagImage(cflag);
					if (flag5)
					{
						Res.outz("my cflag: false");
						Service.gI().getFlag(2, cflag);
					}
				}
			}
			else
			{
				Res.outz("my cflag: not me");
				bool flag6 = GameScr.findCharInMap(charID) == null;
				if (!flag6)
				{
					bool flag7 = GameScr.findCharInMap(charID).isGetFlagImage(cflag);
					if (flag7)
					{
						Res.outz("my cflag: true");
						for (int j = 0; j < GameScr.vFlag.size(); j++)
						{
							PKFlag pkflag2 = (PKFlag)GameScr.vFlag.elementAt(j);
							bool flag8 = pkflag2 != null && pkflag2.cflag == cflag;
							if (flag8)
							{
								Res.outz("my cflag: cflag==");
								GameScr.findCharInMap(charID).flagImage = pkflag2.IDimageFlag;
							}
						}
					}
					else
					{
						bool flag9 = !GameScr.findCharInMap(charID).isGetFlagImage(cflag);
						if (flag9)
						{
							Res.outz("my cflag: false");
							Service.gI().getFlag(2, cflag);
						}
					}
				}
			}
		}
	}

	// Token: 0x060004F0 RID: 1264 RVA: 0x00061D9C File Offset: 0x0005FF9C
	public void actionPerform(int idAction, object p)
	{
		Cout.println("PERFORM WITH ID = " + idAction.ToString());
		bool flag = idAction == 999901;
		if (flag)
		{
			new Thread(new ThreadStart(AddSetDo.MacSet1)).Start();
		}
		bool flag2 = idAction == 999902;
		if (flag2)
		{
			new Thread(new ThreadStart(AddSetDo.MacSet2)).Start();
		}
		int num = idAction;
		int num2 = num;
		if (num2 <= 11059)
		{
			if (num2 <= 8002)
			{
				if (num2 <= 2)
				{
					if (num2 != 1)
					{
						if (num2 == 2)
						{
							GameCanvas.menu.showMenu = false;
						}
					}
					else
					{
						GameCanvas.endDlg();
					}
				}
				else
				{
					switch (num2)
					{
					case 2000:
					{
						this.popUpYesNo = null;
						GameCanvas.endDlg();
						bool flag3 = (global::Char)p == null;
						if (flag3)
						{
							Service.gI().player_vs_player(1, 3, -1);
						}
						else
						{
							Service.gI().player_vs_player(1, 3, ((global::Char)p).charID);
							Service.gI().charMove();
						}
						break;
					}
					case 2001:
						GameCanvas.endDlg();
						break;
					case 2002:
					case 2008:
						break;
					case 2003:
						GameCanvas.endDlg();
						InfoDlg.showWait();
						Service.gI().player_vs_player(0, 3, global::Char.myCharz().charFocus.charID);
						break;
					case 2004:
						GameCanvas.endDlg();
						Service.gI().player_vs_player(0, 4, global::Char.myCharz().charFocus.charID);
						break;
					case 2005:
					{
						GameCanvas.endDlg();
						this.popUpYesNo = null;
						bool flag4 = (global::Char)p == null;
						if (flag4)
						{
							Service.gI().player_vs_player(1, 4, -1);
						}
						else
						{
							Service.gI().player_vs_player(1, 4, ((global::Char)p).charID);
						}
						break;
					}
					case 2006:
						GameCanvas.endDlg();
						Service.gI().player_vs_player(2, 4, global::Char.myCharz().charFocus.charID);
						break;
					case 2007:
						GameCanvas.endDlg();
						GameMidlet.instance.exit();
						break;
					case 2009:
						this.popUpYesNo = null;
						break;
					default:
						if (num2 == 8002)
						{
							this.doFire(false, true);
							GameCanvas.clearKeyHold();
							GameCanvas.clearKeyPressed();
						}
						break;
					}
				}
			}
			else if (num2 <= 11038)
			{
				switch (num2)
				{
				case 11000:
					this.actMenu();
					break;
				case 11001:
					global::Char.myCharz().findNextFocusByKey();
					break;
				case 11002:
					GameCanvas.panel.hide();
					break;
				default:
					if (num2 == 11038)
					{
						this.actDead();
					}
					break;
				}
			}
			else if (num2 != 11057)
			{
				if (num2 == 11059)
				{
					Skill skill = GameScr.onScreenSkill[this.selectedIndexSkill];
					this.doUseSkill(skill, false);
					this.center = null;
				}
			}
			else
			{
				Effect2.vEffect2Outside.removeAllElements();
				Effect2.vEffect2.removeAllElements();
				Npc npc = (Npc)p;
				bool flag5 = npc.idItem == 0;
				if (flag5)
				{
					Service.gI().confirmMenu((short)npc.template.npcTemplateId, (sbyte)GameCanvas.menu.menuSelectedItem);
				}
				else
				{
					bool flag6 = GameCanvas.menu.menuSelectedItem == 0;
					if (flag6)
					{
						Service.gI().pickItem(npc.idItem);
					}
				}
			}
		}
		else if (num2 <= 110001)
		{
			if (num2 <= 11121)
			{
				if (num2 != 11067)
				{
					switch (num2)
					{
					case 11111:
					{
						bool flag7 = global::Char.myCharz().charFocus != null;
						if (flag7)
						{
							InfoDlg.showWait();
							bool flag8 = GameCanvas.panel.vPlayerMenu.size() <= 0;
							if (flag8)
							{
								this.playerMenu(global::Char.myCharz().charFocus);
							}
							GameCanvas.panel.setTypePlayerMenu(global::Char.myCharz().charFocus);
							GameCanvas.panel.show();
							Service.gI().getPlayerMenu(global::Char.myCharz().charFocus.charID);
							Service.gI().messagePlayerMenu(global::Char.myCharz().charFocus.charID);
						}
						break;
					}
					case 11112:
					{
						global::Char @char = (global::Char)p;
						Service.gI().friend(1, @char.charID);
						break;
					}
					case 11113:
					{
						global::Char char2 = (global::Char)p;
						bool flag9 = char2 != null;
						if (flag9)
						{
							Service.gI().giaodich(0, char2.charID, -1, -1);
						}
						break;
					}
					case 11114:
					{
						this.popUpYesNo = null;
						GameCanvas.endDlg();
						global::Char char3 = (global::Char)p;
						bool flag10 = char3 != null;
						if (flag10)
						{
							Service.gI().giaodich(1, char3.charID, -1, -1);
						}
						break;
					}
					case 11115:
					{
						bool flag11 = global::Char.myCharz().charFocus != null;
						if (flag11)
						{
							InfoDlg.showWait();
							Service.gI().playerMenuAction(global::Char.myCharz().charFocus.charID, (short)global::Char.myCharz().charFocus.menuSelect);
						}
						break;
					}
					case 11120:
					{
						object[] array = (object[])p;
						Skill skill2 = (Skill)array[0];
						int num3 = int.Parse((string)array[1]);
						for (int i = 0; i < GameScr.onScreenSkill.Length; i++)
						{
							bool flag12 = GameScr.onScreenSkill[i] == skill2;
							if (flag12)
							{
								GameScr.onScreenSkill[i] = null;
							}
						}
						GameScr.onScreenSkill[num3] = skill2;
						this.saveonScreenSkillToRMS();
						break;
					}
					case 11121:
					{
						object[] array2 = (object[])p;
						Skill skill3 = (Skill)array2[0];
						int num4 = int.Parse((string)array2[1]);
						for (int j = 0; j < GameScr.keySkill.Length; j++)
						{
							bool flag13 = GameScr.keySkill[j] == skill3;
							if (flag13)
							{
								GameScr.keySkill[j] = null;
							}
						}
						GameScr.keySkill[num4] = skill3;
						this.saveKeySkillToRMS();
						break;
					}
					}
				}
				else
				{
					bool flag14 = TileMap.zoneID != GameScr.indexSelect;
					if (flag14)
					{
						Service.gI().requestChangeZone(GameScr.indexSelect, this.indexItemUse);
						InfoDlg.showWait();
					}
					else
					{
						GameScr.info1.addInfo(mResources.ZONE_HERE, 0);
					}
				}
			}
			else
			{
				switch (num2)
				{
				case 12000:
					Service.gI().getClan(1, -1, null);
					break;
				case 12001:
					GameCanvas.endDlg();
					break;
				case 12002:
				{
					GameCanvas.endDlg();
					ClanObject clanObject = (ClanObject)p;
					Service.gI().clanInvite(1, -1, clanObject.clanID, clanObject.code);
					this.popUpYesNo = null;
					break;
				}
				case 12003:
				{
					ClanObject clanObject2 = (ClanObject)p;
					GameCanvas.endDlg();
					Service.gI().clanInvite(2, -1, clanObject2.clanID, clanObject2.code);
					this.popUpYesNo = null;
					break;
				}
				case 12004:
				{
					Skill skill4 = (Skill)p;
					this.doUseSkill(skill4, true);
					global::Char.myCharz().saveLoadPreviousSkill();
					break;
				}
				case 12005:
				{
					bool flag15 = GameCanvas.serverScr == null;
					if (flag15)
					{
						GameCanvas.serverScr = new ServerScr();
					}
					GameCanvas.serverScr.switchToMe();
					GameCanvas.endDlg();
					break;
				}
				case 12006:
					GameMidlet.instance.exit();
					break;
				default:
					if (num2 == 110001)
					{
						GameCanvas.panel.setTypeMain();
						GameCanvas.panel.show();
					}
					break;
				}
			}
		}
		else if (num2 <= 110382)
		{
			if (num2 != 110004)
			{
				if (num2 == 110382)
				{
					Service.gI().returnTownFromDead();
				}
			}
			else
			{
				GameCanvas.menu.showMenu = false;
			}
		}
		else if (num2 != 110383)
		{
			if (num2 != 110391)
			{
				if (num2 == 888351)
				{
					Service.gI().petStatus(5);
					GameCanvas.endDlg();
				}
			}
			else
			{
				Service.gI().clanInvite(0, global::Char.myCharz().charFocus.charID, -1, -1);
			}
		}
		else
		{
			Service.gI().wakeUpFromDead();
		}
	}

	// Token: 0x060004F1 RID: 1265 RVA: 0x0006262C File Offset: 0x0006082C
	private static void setTouchBtn()
	{
		bool flag = GameScr.isAnalog != 0;
		if (flag)
		{
			GameScr.xTG = (GameScr.xF = GameCanvas.w - 45);
			bool isLargeGamePad = GameScr.gamePad.isLargeGamePad;
			if (isLargeGamePad)
			{
				GameScr.xSkill = GameScr.gamePad.wZone + 20;
				GameScr.wSkill = 35;
				GameScr.xHP = GameScr.xF - 45;
			}
			else
			{
				bool isMediumGamePad = GameScr.gamePad.isMediumGamePad;
				if (isMediumGamePad)
				{
					GameScr.xHP = GameScr.xF - 45;
				}
			}
			GameScr.yF = GameCanvas.h - 45;
			GameScr.yTG = GameScr.yF - 45;
		}
	}

	// Token: 0x060004F2 RID: 1266 RVA: 0x000626D0 File Offset: 0x000608D0
	private void updateGamePad()
	{
		bool flag = GameScr.isAnalog == 0 || global::Char.myCharz().statusMe == 14;
		if (!flag)
		{
			bool flag2 = GameCanvas.isPointerHoldIn(GameScr.xF, GameScr.yF, 40, 40);
			if (flag2)
			{
				mScreen.keyTouch = 5;
				bool isPointerJustRelease = GameCanvas.isPointerJustRelease;
				if (isPointerJustRelease)
				{
					GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] = true;
					GameCanvas.isPointerClick = (GameCanvas.isPointerJustDown = (GameCanvas.isPointerJustRelease = false));
				}
			}
			GameScr.gamePad.update();
			bool flag3 = GameCanvas.isPointerHoldIn(GameScr.xTG, GameScr.yTG, 34, 34);
			if (flag3)
			{
				mScreen.keyTouch = 13;
				GameCanvas.isPointerJustDown = false;
				this.isPointerDowning = false;
				bool flag4 = GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease;
				if (flag4)
				{
					global::Char.myCharz().findNextFocusByKey();
					GameCanvas.isPointerClick = (GameCanvas.isPointerJustDown = (GameCanvas.isPointerJustRelease = false));
				}
			}
		}
	}

	// Token: 0x060004F3 RID: 1267 RVA: 0x000627BC File Offset: 0x000609BC
	private void paintGamePad(mGraphics g)
	{
		bool flag = GameScr.isAnalog != 0 && global::Char.myCharz().statusMe != 14;
		if (flag)
		{
			g.drawImage((mScreen.keyTouch != 5 && mScreen.keyMouse != 5) ? GameScr.imgFire0 : GameScr.imgFire1, GameScr.xF + 20, GameScr.yF + 20, mGraphics.HCENTER | mGraphics.VCENTER);
			GameScr.gamePad.paint(g);
			g.drawImage((mScreen.keyTouch != 13) ? GameScr.imgFocus : GameScr.imgFocus2, GameScr.xTG + 20, GameScr.yTG + 20, mGraphics.HCENTER | mGraphics.VCENTER);
		}
	}

	// Token: 0x060004F4 RID: 1268 RVA: 0x00062870 File Offset: 0x00060A70
	public void showWinNumber(string num, string finish)
	{
		this.winnumber = new int[num.Length];
		this.randomNumber = new int[num.Length];
		this.tMove = new int[num.Length];
		this.moveCount = new int[num.Length];
		this.delayMove = new int[num.Length];
		try
		{
			for (int i = 0; i < num.Length; i++)
			{
				this.winnumber[i] = (int)short.Parse(num[i].ToString());
				this.randomNumber[i] = Res.random(0, 11);
				this.tMove[i] = 1;
				this.delayMove[i] = 0;
			}
		}
		catch (Exception)
		{
		}
		this.tShow = 100;
		this.moveIndex = 0;
		this.strFinish = finish;
		GameScr.lastXS = (GameScr.currXS = mSystem.currentTimeMillis());
	}

	// Token: 0x060004F5 RID: 1269 RVA: 0x00062968 File Offset: 0x00060B68
	public void chatVip(string chatVip)
	{
		DovaBaoKhu.TbBoss = chatVip;
		bool flag = !this.startChat;
		if (flag)
		{
			this.currChatWidth = mFont.tahoma_7b_yellowSmall.getWidth(chatVip);
			this.xChatVip = GameCanvas.w;
			this.startChat = true;
		}
		bool flag2 = chatVip.StartsWith("!");
		if (flag2)
		{
			chatVip = chatVip.Substring(1, chatVip.Length);
			this.isFireWorks = true;
		}
		GameScr.vChatVip.addElement(chatVip);
	}

	// Token: 0x060004F6 RID: 1270 RVA: 0x00004E34 File Offset: 0x00003034
	public void clearChatVip()
	{
		GameScr.vChatVip.removeAllElements();
		this.xChatVip = GameCanvas.w;
		this.startChat = false;
	}

	// Token: 0x060004F7 RID: 1271 RVA: 0x000629E4 File Offset: 0x00060BE4
	public void paintChatVip(mGraphics g)
	{
		bool flag = GameScr.vChatVip.size() != 0 && GameScr.isPaintChatVip;
		if (flag)
		{
			g.setClip(0, GameCanvas.h - 13, GameCanvas.w, 15);
			g.fillRect(0, GameCanvas.h - 13, GameCanvas.w, 15, 0, 90);
			string text = (string)GameScr.vChatVip.elementAt(0);
			mFont.tahoma_7b_yellow.drawString(g, text, this.xChatVip, GameCanvas.h - 13, 0, mFont.tahoma_7b_dark);
		}
	}

	// Token: 0x060004F8 RID: 1272 RVA: 0x00062A70 File Offset: 0x00060C70
	public void updateChatVip()
	{
		bool flag = !this.startChat;
		if (!flag)
		{
			this.xChatVip -= 2;
			bool flag2 = this.xChatVip < -this.currChatWidth;
			if (flag2)
			{
				this.xChatVip = GameCanvas.w;
				GameScr.vChatVip.removeElementAt(0);
				bool flag3 = GameScr.vChatVip.size() == 0;
				if (flag3)
				{
					this.isFireWorks = false;
					this.startChat = false;
				}
				else
				{
					this.currChatWidth = mFont.tahoma_7b_white.getWidth((string)GameScr.vChatVip.elementAt(0));
				}
			}
		}
	}

	// Token: 0x060004F9 RID: 1273 RVA: 0x00004E54 File Offset: 0x00003054
	public void showYourNumber(string strNum)
	{
		this.yourNumber = strNum;
		this.strPaint = mFont.tahoma_7.splitFontArray(this.yourNumber, 500);
	}

	// Token: 0x060004FA RID: 1274 RVA: 0x00004E79 File Offset: 0x00003079
	public static void checkRemoveImage()
	{
		ImgByName.checkDelHash(ImgByName.hashImagePath, 10, false);
	}

	// Token: 0x060004FB RID: 1275 RVA: 0x00062B10 File Offset: 0x00060D10
	public static void StartServerPopUp(string strMsg)
	{
		GameCanvas.endDlg();
		int num = 1139;
		ChatPopup.addBigMessage(strMsg, 100000, new Npc(-1, 0, 0, 0, 0, 0)
		{
			avatar = num
		});
		ChatPopup.serverChatPopUp.cmdMsg1 = new Command(mResources.CLOSE, ChatPopup.serverChatPopUp, 1001, null);
		ChatPopup.serverChatPopUp.cmdMsg1.x = GameCanvas.w / 2 - 35;
		ChatPopup.serverChatPopUp.cmdMsg1.y = GameCanvas.h - 35;
	}

	// Token: 0x060004FC RID: 1276 RVA: 0x00062B9C File Offset: 0x00060D9C
	public static bool ispaintPhubangBar()
	{
		return TileMap.mapPhuBang() && GameScr.phuban_Info.type_PB == 0;
	}

	// Token: 0x060004FD RID: 1277 RVA: 0x00062BD0 File Offset: 0x00060DD0
	public void paintPhuBanBar(mGraphics g, int x, int y, int w)
	{
		bool flag = GameScr.phuban_Info == null || GameScr.isPaintOther || GameScr.isPaintRada != 1 || GameCanvas.panel.isShow || !GameScr.ispaintPhubangBar();
		if (!flag)
		{
			bool flag2 = w < GameScr.fra_PVE_Bar_1.frameWidth + GameScr.fra_PVE_Bar_0.frameWidth * 4;
			if (flag2)
			{
				w = GameScr.fra_PVE_Bar_1.frameWidth + GameScr.fra_PVE_Bar_0.frameWidth * 4;
			}
			bool flag3 = x > GameCanvas.w - w / 2;
			if (flag3)
			{
				x = GameCanvas.w - w / 2;
			}
			bool flag4 = x < mGraphics.getImageWidth(GameScr.imgKhung) + w / 2 + 10;
			if (flag4)
			{
				x = mGraphics.getImageWidth(GameScr.imgKhung) + w / 2 + 10;
			}
			int frameHeight = GameScr.fra_PVE_Bar_0.frameHeight;
			int num = y + frameHeight + mGraphics.getImageHeight(GameScr.imgBall) / 2 + 2;
			int frameWidth = GameScr.fra_PVE_Bar_1.frameWidth;
			int num2 = w / 2 - frameWidth / 2;
			int num3 = x - w / 2;
			int num4 = x + frameWidth / 2;
			int num5 = y + 3;
			int num6 = num2 - GameScr.fra_PVE_Bar_0.frameWidth;
			int num7 = num6 / GameScr.fra_PVE_Bar_0.frameWidth;
			bool flag5 = num6 % GameScr.fra_PVE_Bar_0.frameWidth > 0;
			if (flag5)
			{
				num7++;
			}
			for (int i = 0; i < num7; i++)
			{
				bool flag6 = i < num7 - 1;
				if (flag6)
				{
					GameScr.fra_PVE_Bar_0.drawFrame(1, num3 + GameScr.fra_PVE_Bar_0.frameWidth + i * GameScr.fra_PVE_Bar_0.frameWidth, num5, 0, 0, g);
				}
				else
				{
					GameScr.fra_PVE_Bar_0.drawFrame(1, num3 + num6, num5, 0, 0, g);
				}
				bool flag7 = i < num7 - 1;
				if (flag7)
				{
					GameScr.fra_PVE_Bar_0.drawFrame(1, num4 + i * GameScr.fra_PVE_Bar_0.frameWidth, num5, 0, 0, g);
				}
				else
				{
					GameScr.fra_PVE_Bar_0.drawFrame(1, num4 + num6 - GameScr.fra_PVE_Bar_0.frameWidth, num5, 0, 0, g);
				}
			}
			GameScr.fra_PVE_Bar_0.drawFrame(0, num3, num5, 2, 0, g);
			GameScr.fra_PVE_Bar_0.drawFrame(0, num4 + num6, num5, 0, 0, g);
			bool flag8 = GameScr.phuban_Info.pointTeam1 > 0;
			if (flag8)
			{
				int num8 = 2;
				int num9 = 3;
				bool flag9 = GameScr.phuban_Info.color_1 == 4;
				if (flag9)
				{
					num8 = 4;
					num9 = 5;
				}
				int num10 = GameScr.phuban_Info.pointTeam1 * num2 / GameScr.phuban_Info.maxPoint;
				bool flag10 = num10 < 0;
				if (flag10)
				{
					num10 = 0;
				}
				bool flag11 = num10 > num2;
				if (flag11)
				{
					num10 = num2;
				}
				g.setClip(num3 + num2 - num10, num5, num10, frameHeight);
				for (int j = 0; j < num7; j++)
				{
					bool flag12 = j < num7 - 1;
					if (flag12)
					{
						GameScr.fra_PVE_Bar_0.drawFrame(num9, num3 + GameScr.fra_PVE_Bar_0.frameWidth + j * GameScr.fra_PVE_Bar_0.frameWidth, num5, 0, 0, g);
					}
					else
					{
						GameScr.fra_PVE_Bar_0.drawFrame(num9, num3 + num6, num5, 0, 0, g);
					}
				}
				GameScr.fra_PVE_Bar_0.drawFrame(num8, num3, num5, 2, 0, g);
				GameCanvas.resetTrans(g);
			}
			bool flag13 = GameScr.phuban_Info.pointTeam2 > 0;
			if (flag13)
			{
				int num11 = 2;
				int num12 = 3;
				bool flag14 = GameScr.phuban_Info.color_2 == 4;
				if (flag14)
				{
					num11 = 4;
					num12 = 5;
				}
				int num13 = GameScr.phuban_Info.pointTeam2 * num2 / GameScr.phuban_Info.maxPoint;
				bool flag15 = num13 < 0;
				if (flag15)
				{
					num13 = 0;
				}
				bool flag16 = num13 > num2;
				if (flag16)
				{
					num13 = num2;
				}
				g.setClip(num4, num5, num13, frameHeight);
				for (int k = 0; k < num7; k++)
				{
					bool flag17 = k < num7 - 1;
					if (flag17)
					{
						GameScr.fra_PVE_Bar_0.drawFrame(num12, num4 + k * GameScr.fra_PVE_Bar_0.frameWidth, num5, 0, 0, g);
					}
					else
					{
						GameScr.fra_PVE_Bar_0.drawFrame(num12, num4 + num6 - GameScr.fra_PVE_Bar_0.frameWidth, num5, 0, 0, g);
					}
				}
				GameScr.fra_PVE_Bar_0.drawFrame(num11, num4 + num6, num5, 0, 0, g);
				GameCanvas.resetTrans(g);
			}
			GameScr.fra_PVE_Bar_1.drawFrame(0, x - frameWidth / 2, y, 0, 0, g);
			string timeCountDown = mSystem.getTimeCountDown(GameScr.phuban_Info.timeStart, (int)GameScr.phuban_Info.timeSecond, true, false);
			mFont.tahoma_7b_yellow.drawString(g, timeCountDown, x + 1, y + GameScr.fra_PVE_Bar_1.frameHeight / 2 - mFont.tahoma_7b_green2.getHeight() / 2, 2);
			Panel.setTextColor(GameScr.phuban_Info.color_1, 1).drawString(g, GameScr.phuban_Info.nameTeam1, x - 5, num + 5, 1);
			Panel.setTextColor(GameScr.phuban_Info.color_2, 1).drawString(g, GameScr.phuban_Info.nameTeam2, x + 5, num + 5, 0);
			bool flag18 = GameScr.phuban_Info.type_PB != 0;
			if (flag18)
			{
				int num14 = y + frameHeight / 2 - 2;
				mFont.bigNumber_While.drawString(g, string.Empty + GameScr.phuban_Info.pointTeam1.ToString(), num3 + num2 / 2, num14, 2);
				mFont.bigNumber_While.drawString(g, string.Empty + GameScr.phuban_Info.pointTeam2.ToString(), num4 + num2 / 2, num14, 2);
			}
			g.drawImage(GameScr.imgVS, x, y + GameScr.fra_PVE_Bar_1.frameHeight + 2, 3);
			bool flag19 = GameScr.phuban_Info.type_PB == 0;
			if (flag19)
			{
				GameScr.paintChienTruong_Life(g, GameScr.phuban_Info.maxLife, GameScr.phuban_Info.color_1, GameScr.phuban_Info.lifeTeam1, x - 13, GameScr.phuban_Info.color_2, GameScr.phuban_Info.lifeTeam2, x + 13, num);
			}
		}
	}

	// Token: 0x060004FE RID: 1278 RVA: 0x000631D8 File Offset: 0x000613D8
	public static void paintChienTruong_Life(mGraphics g, int maxLife, int cl1, int lifeTeam1, int x1, int cl2, int lifeTeam2, int x2, int y)
	{
		bool flag = GameScr.imgBall == null;
		if (!flag)
		{
			int num = mGraphics.getImageHeight(GameScr.imgBall) / 2;
			for (int i = 0; i < maxLife; i++)
			{
				int num2 = 0;
				bool flag2 = i < lifeTeam1;
				if (flag2)
				{
					num2 = 1;
				}
				g.drawRegion(GameScr.imgBall, 0, num2 * num, mGraphics.getImageWidth(GameScr.imgBall), num, 0, x1 - i * (num + 1), y, mGraphics.VCENTER | mGraphics.HCENTER);
			}
			for (int j = 0; j < maxLife; j++)
			{
				int num3 = 0;
				bool flag3 = j < lifeTeam2;
				if (flag3)
				{
					num3 = 1;
				}
				g.drawRegion(GameScr.imgBall, 0, num3 * num, mGraphics.getImageWidth(GameScr.imgBall), num, 0, x2 + j * (num + 1), y, mGraphics.VCENTER | mGraphics.HCENTER);
			}
		}
	}

	// Token: 0x060004FF RID: 1279 RVA: 0x000632BC File Offset: 0x000614BC
	public static void paintHPBar_NEW(mGraphics g, int x, int y, global::Char c)
	{
		g.drawImage(GameScr.imgKhung, x, y, 0);
		int num = x + 3;
		int num2 = y + 19;
		int width = GameScr.imgHP_NEW.getWidth();
		int num3 = GameScr.imgHP_NEW.getHeight() / 2;
		int num4 = (int)(c.cHP * (long)width / c.cHPFull);
		bool flag = num4 <= 0;
		if (flag)
		{
			num4 = 1;
		}
		else
		{
			bool flag2 = num4 > width;
			if (flag2)
			{
				num4 = width;
			}
		}
		g.drawRegion(GameScr.imgHP_NEW, 0, num3, num4, num3, 0, num, num2, 0);
		int num5 = (int)(c.cMP * (long)width / c.cMPFull);
		bool flag3 = num5 <= 0;
		if (flag3)
		{
			num5 = 1;
		}
		else
		{
			bool flag4 = num5 > width;
			if (flag4)
			{
				num5 = width;
			}
		}
		g.drawRegion(GameScr.imgHP_NEW, 0, 0, num5, num3, 0, num, num2 + 6, 0);
		int num6 = x + GameScr.imgKhung.getWidth() / 2 + 1;
		int num7 = num2 + 13;
		mFont.tahoma_7_green2.drawString(g, c.cName, num6, y + 4, 2);
		bool flag5 = c.mobFocus != null;
		if (flag5)
		{
			bool flag6 = c.mobFocus.getTemplate() != null;
			if (flag6)
			{
				mFont.tahoma_7_green2.drawString(g, c.mobFocus.getTemplate().name, num6, num7, 2);
			}
		}
		else
		{
			bool flag7 = c.npcFocus != null;
			if (flag7)
			{
				mFont.tahoma_7_green2.drawString(g, c.npcFocus.template.name, num6, num7, 2);
			}
			else
			{
				bool flag8 = c.charFocus != null;
				if (flag8)
				{
					mFont.tahoma_7_green2.drawString(g, c.charFocus.cName, num6, num7, 2);
				}
			}
		}
	}

	// Token: 0x06000500 RID: 1280 RVA: 0x00063474 File Offset: 0x00061674
	public static void addEffectEnd(int type, int subtype, int typePaint, int x, int y, int levelPaint, int dir, short timeRemove, Point[] listObj)
	{
		Effect_End effect_End = new Effect_End(type, subtype, typePaint, x, y, levelPaint, dir, timeRemove, listObj);
		GameScr.addEffect2Vector(effect_End);
	}

	// Token: 0x06000501 RID: 1281 RVA: 0x000634A0 File Offset: 0x000616A0
	public static void addEffectEnd_Target(int type, int subtype, int typePaint, global::Char charUse, Point target, int levelPaint, short timeRemove, short range)
	{
		Effect_End effect_End = new Effect_End(type, subtype, typePaint, charUse.clone(), target, levelPaint, timeRemove, range);
		GameScr.addEffect2Vector(effect_End);
	}

	// Token: 0x06000502 RID: 1282 RVA: 0x000634CC File Offset: 0x000616CC
	public static void addEffect2Vector(Effect_End eff)
	{
		bool flag = eff.levelPaint == 0;
		if (flag)
		{
			EffectManager.addHiEffect(eff);
		}
		else
		{
			bool flag2 = eff.levelPaint == 1;
			if (flag2)
			{
				EffectManager.addMidEffects(eff);
			}
			else
			{
				bool flag3 = eff.levelPaint == 2;
				if (flag3)
				{
					EffectManager.addMid_2Effects(eff);
				}
				else
				{
					EffectManager.addLowEffect(eff);
				}
			}
		}
	}

	// Token: 0x06000503 RID: 1283 RVA: 0x0006352C File Offset: 0x0006172C
	public static bool setIsInScreen(int x, int y, int wOne, int hOne)
	{
		bool flag = x < GameScr.cmx - wOne || x > GameScr.cmx + GameCanvas.w + wOne || y < GameScr.cmy - hOne || y > GameScr.cmy + GameCanvas.h + hOne * 3 / 2;
		return !flag;
	}

	// Token: 0x06000504 RID: 1284 RVA: 0x00063584 File Offset: 0x00061784
	public static bool isSmallScr()
	{
		return GameCanvas.w <= 320;
	}

	// Token: 0x06000505 RID: 1285 RVA: 0x000635B0 File Offset: 0x000617B0
	private void paint_xp_bar(mGraphics g)
	{
		g.setColor(8421504);
		g.fillRect(0, GameCanvas.h - 2, GameCanvas.w, 2);
		int num = (int)(global::Char.myCharz().cLevelPercent * (long)GameCanvas.w / 10000L);
		g.setColor(16777215);
		g.fillRect(0, GameCanvas.h - 2, num, 2);
		g.setColor(0);
		num = GameCanvas.w / 10;
		for (int i = 1; i < 10; i++)
		{
			g.fillRect(i * num, GameCanvas.h - 2, 1, 2);
		}
	}

	// Token: 0x06000506 RID: 1286 RVA: 0x00063650 File Offset: 0x00061850
	private void paint_ios_bg(mGraphics g)
	{
		bool flag = mSystem.clientType == 5;
		if (flag)
		{
			bool flag2 = GameScr.imgBgIOS != null;
			if (flag2)
			{
				g.setColor(16777215);
				g.fillRect(0, 0, GameCanvas.w, GameCanvas.h);
				g.drawImage(GameScr.imgBgIOS, GameCanvas.w / 2, GameCanvas.h / 2, mGraphics.VCENTER | mGraphics.HCENTER);
			}
			else
			{
				GameScr.imgBgIOS = GameCanvas.loadImage("/bg/bg_ios_" + ((TileMap.bgID % 2 != 0) ? 1 : 2).ToString() + ".png");
			}
		}
	}

	// Token: 0x06000507 RID: 1287 RVA: 0x000636F4 File Offset: 0x000618F4
	public void paint_CT(mGraphics g, int x, int y, int w)
	{
		w = 194;
		w = 182;
		w = 170;
		int num = 66;
		int num2 = 11;
		bool flag = x > GameCanvas.w - w / 2;
		if (flag)
		{
			x = GameCanvas.w - w / 2;
		}
		bool flag2 = x < mGraphics.getImageWidth(GameScr.imgKhung) + w / 2 + 10;
		if (flag2)
		{
			x = mGraphics.getImageWidth(GameScr.imgKhung) + w / 2 + 10;
		}
		int frameHeight = GameScr.fra_PVE_Bar_0.frameHeight;
		int num3 = y + frameHeight + mGraphics.getImageHeight(GameScr.imgBall) / 2 + 2;
		int frameWidth = GameScr.fra_PVE_Bar_1.frameWidth;
		int num4 = w / 2 - frameWidth / 2;
		int num5 = x - w / 2 + 3;
		int num6 = x + frameWidth / 2;
		int num7 = y + 3;
		int num8 = num4 - GameScr.fra_PVE_Bar_0.frameWidth;
		int num9 = num8 / GameScr.fra_PVE_Bar_0.frameWidth;
		bool flag3 = num8 % GameScr.fra_PVE_Bar_0.frameWidth > 0;
		if (flag3)
		{
			num9++;
		}
		for (int i = 0; i < num9; i++)
		{
			bool flag4 = i < num9 - 1;
			if (flag4)
			{
				g.drawRegion(GameScr.img_ct_bar_0, 0, 15, mGraphics.getImageWidth(GameScr.img_ct_bar_0), 15, 2, num5 + GameScr.fra_PVE_Bar_0.frameWidth + i * GameScr.fra_PVE_Bar_0.frameWidth, num7, mGraphics.TOP | mGraphics.LEFT, true);
			}
			else
			{
				g.drawRegion(GameScr.img_ct_bar_0, 0, 15, mGraphics.getImageWidth(GameScr.img_ct_bar_0), 15, 2, num5 + num8, num7, mGraphics.TOP | mGraphics.LEFT, true);
			}
			bool flag5 = i < num9 - 1;
			if (flag5)
			{
				g.drawRegion(GameScr.img_ct_bar_0, 0, 15, mGraphics.getImageWidth(GameScr.img_ct_bar_0), 15, 2, num6 + i * GameScr.fra_PVE_Bar_0.frameWidth, num7, mGraphics.TOP | mGraphics.LEFT, true);
			}
			else
			{
				g.drawRegion(GameScr.img_ct_bar_0, 0, 15, mGraphics.getImageWidth(GameScr.img_ct_bar_0), 15, 2, num6 + num8 - GameScr.fra_PVE_Bar_0.frameWidth, num7, mGraphics.TOP | mGraphics.LEFT, true);
			}
		}
		GameScr.fra_PVE_Bar_0.drawFrame(0, num5, num7, 2, 0, g);
		GameScr.fra_PVE_Bar_0.drawFrame(0, num6 + num8, num7, 0, 0, g);
		int num10 = GameScr.nCT_TeamA * 100 / (GameScr.nCT_nBoyBaller / 2) * num / 100;
		bool flag6 = num10 > 0;
		if (flag6)
		{
			bool flag7 = num10 < 6;
			if (flag7)
			{
				num10 = 6;
			}
			g.setClip(num5, num7, num10, 15);
		}
		bool flag8 = GameScr.nCT_TeamA > 0;
		if (flag8)
		{
			for (int j = 0; j < num2; j++)
			{
				bool flag9 = j == 0;
				if (flag9)
				{
					g.drawRegion(GameScr.img_ct_bar_0, 0, 60, mGraphics.getImageWidth(GameScr.img_ct_bar_0), 15, 2, num5, num7, mGraphics.TOP | mGraphics.LEFT, true);
				}
				else
				{
					g.drawRegion(GameScr.img_ct_bar_0, 0, 75, mGraphics.getImageWidth(GameScr.img_ct_bar_0), 15, 2, num5 + j * 6, num7, mGraphics.TOP | mGraphics.LEFT, true);
				}
			}
		}
		GameCanvas.resetTrans(g);
		int num11 = GameScr.nCT_TeamB * 100 / (GameScr.nCT_nBoyBaller / 2) * num / 100;
		bool flag10 = num - (num - num11) > 0;
		if (flag10)
		{
			bool flag11 = num11 < 6;
			if (flag11)
			{
				num11 = 6;
			}
			g.setClip(num6 + num - num11, num7, num - (num - num11), 15);
		}
		bool flag12 = GameScr.nCT_TeamB > 0;
		if (flag12)
		{
			for (int k = 0; k < num2; k++)
			{
				bool flag13 = k == 0;
				if (flag13)
				{
					g.drawRegion(GameScr.img_ct_bar_0, 0, 30, mGraphics.getImageWidth(GameScr.img_ct_bar_0), 15, 0, num6 + num8, num7, mGraphics.TOP | mGraphics.LEFT, true);
				}
				else
				{
					g.drawRegion(GameScr.img_ct_bar_0, 0, 45, mGraphics.getImageWidth(GameScr.img_ct_bar_0), 15, 0, num6 + num8 - k * 6, num7, mGraphics.TOP | mGraphics.LEFT, true);
				}
			}
		}
		GameCanvas.resetTrans(g);
		GameScr.fra_PVE_Bar_1.drawFrame(0, x - frameWidth / 2 + 1, y, 0, 0, g);
		string text = NinjaUtil.getTime((int)((GameScr.nCT_timeBallte - mSystem.currentTimeMillis()) / 1000L)) + string.Empty;
		mFont.tahoma_7b_yellow.drawString(g, text, num5 + w / 2 - 2, y + 5, 2);
		mFont.tahoma_7_grey.drawString(g, "Tầng " + GameScr.nCT_floor.ToString(), num5 + w / 2 - 3, y + GameScr.fra_PVE_Bar_1.frameHeight, mFont.CENTER);
		int num12 = mFont.tahoma_7b_red.getWidth(GameScr.nCT_TeamA.ToString() + string.Empty);
		mFont.tahoma_7b_blue.drawString(g, GameScr.nCT_TeamA.ToString() + string.Empty, x - frameWidth / 2 - num12, num7 + GameScr.fra_PVE_Bar_1.frameHeight, 0);
		SmallImage.drawSmallImage(g, 2325, x - frameWidth / 2 - num12 - 15, num7 + GameScr.fra_PVE_Bar_1.frameHeight, 2, mGraphics.TOP | mGraphics.LEFT);
		num12 = mFont.tahoma_7b_red.getWidth(GameScr.nCT_TeamB.ToString() + string.Empty);
		mFont.tahoma_7b_red.drawString(g, GameScr.nCT_TeamB.ToString() + string.Empty, x + frameWidth / 2, num7 + GameScr.fra_PVE_Bar_1.frameHeight, 0);
		SmallImage.drawSmallImage(g, 2323, x + frameWidth / 2 + num12 + 3, num7 + GameScr.fra_PVE_Bar_1.frameHeight, 0, mGraphics.TOP | mGraphics.LEFT);
		this.paint_board_CT(g, GameCanvas.w - mFont.tahoma_7b_dark.getWidth("#01 AAAAAAAAAA"), 40);
		GameCanvas.resetTrans(g);
	}

	// Token: 0x06000508 RID: 1288 RVA: 0x00063CF4 File Offset: 0x00061EF4
	private void paint_board_CT(mGraphics g, int x, int y)
	{
		bool flag = !GameScr.is_Paint_boardCT_Expand;
		if (flag)
		{
			string text = "#01 nnnnnnnnnnnn";
			int width = mFont.tahoma_7.getWidth(text);
			int num = GameCanvas.w - width - 20;
			for (int i = 0; i < GameScr.nTop; i++)
			{
				mFont mFont = mFont.tahoma_7_white;
				switch (i)
				{
				case 0:
					mFont = mFont.tahoma_7_red;
					break;
				case 1:
					mFont = mFont.tahoma_7_yellow;
					break;
				case 2:
					mFont = mFont.tahoma_7_blue;
					break;
				}
				bool flag2 = i == GameScr.nTop - 1;
				if (flag2)
				{
					mFont = mFont.tahoma_7_green;
				}
				string[] array = Res.split((string)GameScr.res_CT.elementAt(i), "|", 0);
				int[] array2 = new int[] { 0, 18 };
				for (int j = 0; j < 2; j++)
				{
					mFont.drawString(g, array[j], num + array2[j], y + i * mFont.tahoma_7.getHeight(), 0, mFont.tahoma_7);
				}
			}
			GameCanvas.resetTrans(g);
			GameScr.xRect = num;
			GameScr.yRect = y;
			GameScr.wRect = width + 10;
			GameScr.hRect = mFont.tahoma_7b_dark.getHeight() * 6;
		}
		else
		{
			string text2 = "#01 namec1000000 0001   00000";
			int[] array3 = new int[] { 0, 18, 80, 101 };
			int width2 = mFont.tahoma_7.getWidth(text2);
			int num2 = GameCanvas.w - width2 - 20;
			for (int k = 0; k < GameScr.nTop; k++)
			{
				string[] array4 = Res.split((string)GameScr.res_CT.elementAt(k), "|", 0);
				mFont mFont2 = mFont.tahoma_7_white;
				switch (k)
				{
				case 0:
					mFont2 = mFont.tahoma_7_red;
					break;
				case 1:
					mFont2 = mFont.tahoma_7_yellow;
					break;
				case 2:
					mFont2 = mFont.tahoma_7_blue;
					break;
				}
				bool flag3 = k == GameScr.nTop - 1;
				if (flag3)
				{
					mFont2 = mFont.tahoma_7_green;
				}
				int num3 = k * mFont.tahoma_7_white.getHeight() + y;
				for (int l = 0; l < array3.Length; l++)
				{
					mFont2.drawString(g, array4[l], num2 + array3[l], num3, 0, mFont.tahoma_7);
				}
			}
			GameScr.xRect = num2;
			GameScr.yRect = y;
			GameScr.wRect = width2 + 10;
			GameScr.hRect = mFont.tahoma_7b_dark.getHeight() * 6;
		}
		GameCanvas.resetTrans(g);
	}

	// Token: 0x06000509 RID: 1289 RVA: 0x00063F98 File Offset: 0x00062198
	private void paintHPCT(mGraphics g, int x, int y, global::Char c)
	{
		g.drawImage(GameScr.imgKhung, x, y, 0);
		int num = x + 3;
		int num2 = y + 19;
		int width = GameScr.imgHP_NEW.getWidth();
		int num3 = GameScr.imgHP_NEW.getHeight() / 2;
		int num4 = (int)(c.cHP * (long)width / c.cHPFull);
		bool flag = num4 <= 0;
		if (!flag)
		{
			bool flag2 = num4 > width;
			if (flag2)
			{
			}
		}
		g.drawRegion(GameScr.imgHP_NEW, 0, num3, 80, num3, 0, num, num2, 0);
		int num5 = (int)(c.cMP * (long)width / c.cMPFull);
		bool flag3 = num5 <= 0;
		if (!flag3)
		{
			bool flag4 = num5 > width;
			if (flag4)
			{
			}
		}
		g.drawRegion(GameScr.imgHP_NEW, 0, 0, 80, num3, 0, num, num2 + 6, 0);
	}

	// Token: 0x04000775 RID: 1909
	public bool isWaitingDoubleClick;

	// Token: 0x04000776 RID: 1910
	public long timeStartDblClick;

	// Token: 0x04000777 RID: 1911
	public long timeEndDblClick;

	// Token: 0x04000778 RID: 1912
	public static bool isPaintOther = false;

	// Token: 0x04000779 RID: 1913
	public static MyVector textTime = new MyVector(string.Empty);

	// Token: 0x0400077A RID: 1914
	public static bool isLoadAllData = false;

	// Token: 0x0400077B RID: 1915
	public static GameScr instance;

	// Token: 0x0400077C RID: 1916
	public static int gW;

	// Token: 0x0400077D RID: 1917
	public static int gH;

	// Token: 0x0400077E RID: 1918
	public static int gW2;

	// Token: 0x0400077F RID: 1919
	public static int gssw;

	// Token: 0x04000780 RID: 1920
	public static int gssh;

	// Token: 0x04000781 RID: 1921
	public static int gH34;

	// Token: 0x04000782 RID: 1922
	public static int gW3;

	// Token: 0x04000783 RID: 1923
	public static int gH3;

	// Token: 0x04000784 RID: 1924
	public static int gH23;

	// Token: 0x04000785 RID: 1925
	public static int gW23;

	// Token: 0x04000786 RID: 1926
	public static int gH2;

	// Token: 0x04000787 RID: 1927
	public static int csPadMaxH;

	// Token: 0x04000788 RID: 1928
	public static int cmdBarH;

	// Token: 0x04000789 RID: 1929
	public static int gW34;

	// Token: 0x0400078A RID: 1930
	public static int gW6;

	// Token: 0x0400078B RID: 1931
	public static int gH6;

	// Token: 0x0400078C RID: 1932
	public static int cmx;

	// Token: 0x0400078D RID: 1933
	public static int cmy;

	// Token: 0x0400078E RID: 1934
	public static int cmdx;

	// Token: 0x0400078F RID: 1935
	public static int cmdy;

	// Token: 0x04000790 RID: 1936
	public static int cmvx;

	// Token: 0x04000791 RID: 1937
	public static int cmvy;

	// Token: 0x04000792 RID: 1938
	public static int cmtoX;

	// Token: 0x04000793 RID: 1939
	public static int cmtoY;

	// Token: 0x04000794 RID: 1940
	public static int cmxLim;

	// Token: 0x04000795 RID: 1941
	public static int cmyLim;

	// Token: 0x04000796 RID: 1942
	public static int gssx;

	// Token: 0x04000797 RID: 1943
	public static int gssy;

	// Token: 0x04000798 RID: 1944
	public static int gssxe;

	// Token: 0x04000799 RID: 1945
	public static int gssye;

	// Token: 0x0400079A RID: 1946
	public Command cmdback;

	// Token: 0x0400079B RID: 1947
	public Command cmdBag;

	// Token: 0x0400079C RID: 1948
	public Command cmdSkill;

	// Token: 0x0400079D RID: 1949
	public Command cmdTiemnang;

	// Token: 0x0400079E RID: 1950
	public Command cmdtrangbi;

	// Token: 0x0400079F RID: 1951
	public Command cmdInfo;

	// Token: 0x040007A0 RID: 1952
	public Command cmdFocus;

	// Token: 0x040007A1 RID: 1953
	public Command cmdFire;

	// Token: 0x040007A2 RID: 1954
	public static int d;

	// Token: 0x040007A3 RID: 1955
	public static int hpPotion;

	// Token: 0x040007A4 RID: 1956
	public static SkillPaint[] sks;

	// Token: 0x040007A5 RID: 1957
	public static Arrowpaint[] arrs;

	// Token: 0x040007A6 RID: 1958
	public static DartInfo[] darts;

	// Token: 0x040007A7 RID: 1959
	public static Part[] parts;

	// Token: 0x040007A8 RID: 1960
	public static EffectCharPaint[] efs;

	// Token: 0x040007A9 RID: 1961
	public static int lockTick;

	// Token: 0x040007AA RID: 1962
	private int moveUp;

	// Token: 0x040007AB RID: 1963
	private int moveDow;

	// Token: 0x040007AC RID: 1964
	private int idTypeTask;

	// Token: 0x040007AD RID: 1965
	private bool isstarOpen;

	// Token: 0x040007AE RID: 1966
	private bool isChangeSkill;

	// Token: 0x040007AF RID: 1967
	public static MyVector vClan = new MyVector();

	// Token: 0x040007B0 RID: 1968
	public static MyVector vPtMap = new MyVector();

	// Token: 0x040007B1 RID: 1969
	public static MyVector vFriend = new MyVector();

	// Token: 0x040007B2 RID: 1970
	public static MyVector vEnemies = new MyVector();

	// Token: 0x040007B3 RID: 1971
	public static MyVector vCharInMap = new MyVector();

	// Token: 0x040007B4 RID: 1972
	public static MyVector vItemMap = new MyVector();

	// Token: 0x040007B5 RID: 1973
	public static MyVector vMobAttack = new MyVector();

	// Token: 0x040007B6 RID: 1974
	public static MyVector vSet = new MyVector();

	// Token: 0x040007B7 RID: 1975
	public static MyVector vMob = new MyVector();

	// Token: 0x040007B8 RID: 1976
	public static MyVector vNpc = new MyVector();

	// Token: 0x040007B9 RID: 1977
	public static MyVector vFlag = new MyVector();

	// Token: 0x040007BA RID: 1978
	public static NClass[] nClasss;

	// Token: 0x040007BB RID: 1979
	public static int indexSize = 28;

	// Token: 0x040007BC RID: 1980
	public static int indexTitle = 0;

	// Token: 0x040007BD RID: 1981
	public static int indexSelect = 0;

	// Token: 0x040007BE RID: 1982
	public static int indexRow = -1;

	// Token: 0x040007BF RID: 1983
	public static int indexRowMax;

	// Token: 0x040007C0 RID: 1984
	public static int indexMenu = 0;

	// Token: 0x040007C1 RID: 1985
	public Item itemFocus;

	// Token: 0x040007C2 RID: 1986
	public ItemOptionTemplate[] iOptionTemplates;

	// Token: 0x040007C3 RID: 1987
	public SkillOptionTemplate[] sOptionTemplates;

	// Token: 0x040007C4 RID: 1988
	private static Scroll scrInfo = new Scroll();

	// Token: 0x040007C5 RID: 1989
	public static Scroll scrMain = new Scroll();

	// Token: 0x040007C6 RID: 1990
	public static MyVector vItemUpGrade = new MyVector();

	// Token: 0x040007C7 RID: 1991
	public static bool isTypeXu;

	// Token: 0x040007C8 RID: 1992
	public static bool isViewNext;

	// Token: 0x040007C9 RID: 1993
	public static bool isViewClanMemOnline = false;

	// Token: 0x040007CA RID: 1994
	public static bool isViewClanInvite = true;

	// Token: 0x040007CB RID: 1995
	public static bool isChop;

	// Token: 0x040007CC RID: 1996
	public static string titleInputText = string.Empty;

	// Token: 0x040007CD RID: 1997
	public static int tickMove;

	// Token: 0x040007CE RID: 1998
	public static bool isPaintAlert = false;

	// Token: 0x040007CF RID: 1999
	public static bool isPaintTask = false;

	// Token: 0x040007D0 RID: 2000
	public static bool isPaintTeam = false;

	// Token: 0x040007D1 RID: 2001
	public static bool isPaintFindTeam = false;

	// Token: 0x040007D2 RID: 2002
	public static bool isPaintFriend = false;

	// Token: 0x040007D3 RID: 2003
	public static bool isPaintEnemies = false;

	// Token: 0x040007D4 RID: 2004
	public static bool isPaintItemInfo = false;

	// Token: 0x040007D5 RID: 2005
	public static bool isHaveSelectSkill = false;

	// Token: 0x040007D6 RID: 2006
	public static bool isPaintSkill = false;

	// Token: 0x040007D7 RID: 2007
	public static bool isPaintInfoMe = false;

	// Token: 0x040007D8 RID: 2008
	public static bool isPaintStore = false;

	// Token: 0x040007D9 RID: 2009
	public static bool isPaintNonNam = false;

	// Token: 0x040007DA RID: 2010
	public static bool isPaintNonNu = false;

	// Token: 0x040007DB RID: 2011
	public static bool isPaintAoNam = false;

	// Token: 0x040007DC RID: 2012
	public static bool isPaintAoNu = false;

	// Token: 0x040007DD RID: 2013
	public static bool isPaintGangTayNam = false;

	// Token: 0x040007DE RID: 2014
	public static bool isPaintGangTayNu = false;

	// Token: 0x040007DF RID: 2015
	public static bool isPaintQuanNam = false;

	// Token: 0x040007E0 RID: 2016
	public static bool isPaintQuanNu = false;

	// Token: 0x040007E1 RID: 2017
	public static bool isPaintGiayNam = false;

	// Token: 0x040007E2 RID: 2018
	public static bool isPaintGiayNu = false;

	// Token: 0x040007E3 RID: 2019
	public static bool isPaintLien = false;

	// Token: 0x040007E4 RID: 2020
	public static bool isPaintNhan = false;

	// Token: 0x040007E5 RID: 2021
	public static bool isPaintNgocBoi = false;

	// Token: 0x040007E6 RID: 2022
	public static bool isPaintPhu = false;

	// Token: 0x040007E7 RID: 2023
	public static bool isPaintWeapon = false;

	// Token: 0x040007E8 RID: 2024
	public static bool isPaintStack = false;

	// Token: 0x040007E9 RID: 2025
	public static bool isPaintStackLock = false;

	// Token: 0x040007EA RID: 2026
	public static bool isPaintGrocery = false;

	// Token: 0x040007EB RID: 2027
	public static bool isPaintGroceryLock = false;

	// Token: 0x040007EC RID: 2028
	public static bool isPaintUpGrade = false;

	// Token: 0x040007ED RID: 2029
	public static bool isPaintConvert = false;

	// Token: 0x040007EE RID: 2030
	public static bool isPaintUpGradeGold = false;

	// Token: 0x040007EF RID: 2031
	public static bool isPaintUpPearl = false;

	// Token: 0x040007F0 RID: 2032
	public static bool isPaintBox = false;

	// Token: 0x040007F1 RID: 2033
	public static bool isPaintSplit = false;

	// Token: 0x040007F2 RID: 2034
	public static bool isPaintCharInMap = false;

	// Token: 0x040007F3 RID: 2035
	public static bool isPaintTrade = false;

	// Token: 0x040007F4 RID: 2036
	public static bool isPaintZone = false;

	// Token: 0x040007F5 RID: 2037
	public static bool isPaintMessage = false;

	// Token: 0x040007F6 RID: 2038
	public static bool isPaintClan = false;

	// Token: 0x040007F7 RID: 2039
	public static bool isRequestMember = false;

	// Token: 0x040007F8 RID: 2040
	public static global::Char currentCharViewInfo;

	// Token: 0x040007F9 RID: 2041
	public static long[] exps;

	// Token: 0x040007FA RID: 2042
	public static int[] crystals;

	// Token: 0x040007FB RID: 2043
	public static int[] upClothe;

	// Token: 0x040007FC RID: 2044
	public static int[] upAdorn;

	// Token: 0x040007FD RID: 2045
	public static int[] upWeapon;

	// Token: 0x040007FE RID: 2046
	public static int[] coinUpCrystals;

	// Token: 0x040007FF RID: 2047
	public static int[] coinUpClothes;

	// Token: 0x04000800 RID: 2048
	public static int[] coinUpAdorns;

	// Token: 0x04000801 RID: 2049
	public static int[] coinUpWeapons;

	// Token: 0x04000802 RID: 2050
	public static int[] maxPercents;

	// Token: 0x04000803 RID: 2051
	public static int[] goldUps;

	// Token: 0x04000804 RID: 2052
	public int tMenuDelay;

	// Token: 0x04000805 RID: 2053
	public int zoneCol = 6;

	// Token: 0x04000806 RID: 2054
	public int[] zones;

	// Token: 0x04000807 RID: 2055
	public int[] pts;

	// Token: 0x04000808 RID: 2056
	public int[] numPlayer;

	// Token: 0x04000809 RID: 2057
	public int[] maxPlayer;

	// Token: 0x0400080A RID: 2058
	public int[] rank1;

	// Token: 0x0400080B RID: 2059
	public int[] rank2;

	// Token: 0x0400080C RID: 2060
	public string[] rankName1;

	// Token: 0x0400080D RID: 2061
	public string[] rankName2;

	// Token: 0x0400080E RID: 2062
	public int typeTrade;

	// Token: 0x0400080F RID: 2063
	public int typeTradeOrder;

	// Token: 0x04000810 RID: 2064
	public int coinTrade;

	// Token: 0x04000811 RID: 2065
	public int coinTradeOrder;

	// Token: 0x04000812 RID: 2066
	public int timeTrade;

	// Token: 0x04000813 RID: 2067
	public int indexItemUse = -1;

	// Token: 0x04000814 RID: 2068
	public int cLastFocusID = -1;

	// Token: 0x04000815 RID: 2069
	public int cPreFocusID = -1;

	// Token: 0x04000816 RID: 2070
	public bool isLockKey;

	// Token: 0x04000817 RID: 2071
	public static int[] tasks;

	// Token: 0x04000818 RID: 2072
	public static int[] mapTasks;

	// Token: 0x04000819 RID: 2073
	public static Image imgRoomStat;

	// Token: 0x0400081A RID: 2074
	public static Image frBarPow0;

	// Token: 0x0400081B RID: 2075
	public static Image frBarPow1;

	// Token: 0x0400081C RID: 2076
	public static Image frBarPow2;

	// Token: 0x0400081D RID: 2077
	public static Image frBarPow20;

	// Token: 0x0400081E RID: 2078
	public static Image frBarPow21;

	// Token: 0x0400081F RID: 2079
	public static Image frBarPow22;

	// Token: 0x04000820 RID: 2080
	public MyVector texts;

	// Token: 0x04000821 RID: 2081
	public string textsTitle;

	// Token: 0x04000822 RID: 2082
	public static sbyte vcData;

	// Token: 0x04000823 RID: 2083
	public static sbyte vcMap;

	// Token: 0x04000824 RID: 2084
	public static sbyte vcSkill;

	// Token: 0x04000825 RID: 2085
	public static sbyte vcItem;

	// Token: 0x04000826 RID: 2086
	public static sbyte vsData;

	// Token: 0x04000827 RID: 2087
	public static sbyte vsMap;

	// Token: 0x04000828 RID: 2088
	public static sbyte vsSkill;

	// Token: 0x04000829 RID: 2089
	public static sbyte vsItem;

	// Token: 0x0400082A RID: 2090
	public static sbyte vcTask;

	// Token: 0x0400082B RID: 2091
	public static Image imgArrow;

	// Token: 0x0400082C RID: 2092
	public static Image imgArrow2;

	// Token: 0x0400082D RID: 2093
	public static Image imgChat;

	// Token: 0x0400082E RID: 2094
	public static Image imgChat2;

	// Token: 0x0400082F RID: 2095
	public static Image imgMenu;

	// Token: 0x04000830 RID: 2096
	public static Image imgFocus;

	// Token: 0x04000831 RID: 2097
	public static Image imgFocus2;

	// Token: 0x04000832 RID: 2098
	public static Image imgSkill;

	// Token: 0x04000833 RID: 2099
	public static Image imgSkill2;

	// Token: 0x04000834 RID: 2100
	public static Image imgHP1;

	// Token: 0x04000835 RID: 2101
	public static Image imgHP2;

	// Token: 0x04000836 RID: 2102
	public static Image imgHP3;

	// Token: 0x04000837 RID: 2103
	public static Image imgHP4;

	// Token: 0x04000838 RID: 2104
	public static Image imgFire0;

	// Token: 0x04000839 RID: 2105
	public static Image imgFire1;

	// Token: 0x0400083A RID: 2106
	public static Image imgNR1;

	// Token: 0x0400083B RID: 2107
	public static Image imgNR2;

	// Token: 0x0400083C RID: 2108
	public static Image imgNR3;

	// Token: 0x0400083D RID: 2109
	public static Image imgNR4;

	// Token: 0x0400083E RID: 2110
	public static Image imgLbtn;

	// Token: 0x0400083F RID: 2111
	public static Image imgLbtnFocus;

	// Token: 0x04000840 RID: 2112
	public static Image imgLbtn2;

	// Token: 0x04000841 RID: 2113
	public static Image imgLbtnFocus2;

	// Token: 0x04000842 RID: 2114
	public static Image imgAnalog1;

	// Token: 0x04000843 RID: 2115
	public static Image imgAnalog2;

	// Token: 0x04000844 RID: 2116
	public string tradeName = string.Empty;

	// Token: 0x04000845 RID: 2117
	public string tradeItemName = string.Empty;

	// Token: 0x04000846 RID: 2118
	public int timeLengthMap;

	// Token: 0x04000847 RID: 2119
	public int timeStartMap;

	// Token: 0x04000848 RID: 2120
	public static sbyte typeViewInfo = 0;

	// Token: 0x04000849 RID: 2121
	public static sbyte typeActive = 0;

	// Token: 0x0400084A RID: 2122
	public static InfoMe info1 = new InfoMe();

	// Token: 0x0400084B RID: 2123
	public static InfoMe info2 = new InfoMe();

	// Token: 0x0400084C RID: 2124
	public static Image imgPanel;

	// Token: 0x0400084D RID: 2125
	public static Image imgPanel2;

	// Token: 0x0400084E RID: 2126
	public static Image imgHP;

	// Token: 0x0400084F RID: 2127
	public static Image imgMP;

	// Token: 0x04000850 RID: 2128
	public static Image imgSP;

	// Token: 0x04000851 RID: 2129
	public static Image imgHPLost;

	// Token: 0x04000852 RID: 2130
	public static Image imgMPLost;

	// Token: 0x04000853 RID: 2131
	public static Image imgHP_tm_do;

	// Token: 0x04000854 RID: 2132
	public static Image imgHP_tm_vang;

	// Token: 0x04000855 RID: 2133
	public static Image imgHP_tm_xam;

	// Token: 0x04000856 RID: 2134
	public static Image imgHP_tm_xanh;

	// Token: 0x04000857 RID: 2135
	public static Image imgHP_tm_xanhnuocbien;

	// Token: 0x04000858 RID: 2136
	public Mob mobCapcha;

	// Token: 0x04000859 RID: 2137
	public MagicTree magicTree;

	// Token: 0x0400085A RID: 2138
	private short l;

	// Token: 0x0400085B RID: 2139
	public static int countEff;

	// Token: 0x0400085C RID: 2140
	public static GamePad gamePad = new GamePad();

	// Token: 0x0400085D RID: 2141
	public static Image imgChatPC;

	// Token: 0x0400085E RID: 2142
	public static Image imgChatsPC2;

	// Token: 0x0400085F RID: 2143
	public static int isAnalog = 0;

	// Token: 0x04000860 RID: 2144
	public static Image img_ct_bar_0 = mSystem.loadImage("/mainImage/i_pve_bar_0.png");

	// Token: 0x04000861 RID: 2145
	public static Image img_ct_bar_1 = mSystem.loadImage("/mainImage/i_pve_bar_1.png");

	// Token: 0x04000862 RID: 2146
	public static bool isUseTouch;

	// Token: 0x04000863 RID: 2147
	public Command cmdDoiCo;

	// Token: 0x04000864 RID: 2148
	public Command cmdLogOut;

	// Token: 0x04000865 RID: 2149
	public Command cmdChatTheGioi;

	// Token: 0x04000866 RID: 2150
	public Command cmdshowInfo;

	// Token: 0x04000867 RID: 2151
	private static Command[] cmdTestLogin = null;

	// Token: 0x04000868 RID: 2152
	public const int numSkill = 10;

	// Token: 0x04000869 RID: 2153
	public const int numSkill_2 = 5;

	// Token: 0x0400086A RID: 2154
	public static Skill[] keySkill = new Skill[10];

	// Token: 0x0400086B RID: 2155
	public static Skill[] onScreenSkill = new Skill[10];

	// Token: 0x0400086C RID: 2156
	public Command cmdMenu;

	// Token: 0x0400086D RID: 2157
	public static int firstY;

	// Token: 0x0400086E RID: 2158
	public static int wSkill;

	// Token: 0x0400086F RID: 2159
	public static long deltaTime;

	// Token: 0x04000870 RID: 2160
	public bool isPointerDowning;

	// Token: 0x04000871 RID: 2161
	public bool isChangingCameraMode;

	// Token: 0x04000872 RID: 2162
	private int ptLastDownX;

	// Token: 0x04000873 RID: 2163
	private int ptLastDownY;

	// Token: 0x04000874 RID: 2164
	private int ptFirstDownX;

	// Token: 0x04000875 RID: 2165
	private int ptFirstDownY;

	// Token: 0x04000876 RID: 2166
	private int ptDownTime;

	// Token: 0x04000877 RID: 2167
	private bool disableSingleClick;

	// Token: 0x04000878 RID: 2168
	public long lastSingleClick;

	// Token: 0x04000879 RID: 2169
	public bool clickMoving;

	// Token: 0x0400087A RID: 2170
	public bool clickOnTileTop;

	// Token: 0x0400087B RID: 2171
	public bool clickMovingRed;

	// Token: 0x0400087C RID: 2172
	private int clickToX;

	// Token: 0x0400087D RID: 2173
	private int clickToY;

	// Token: 0x0400087E RID: 2174
	private int lastClickCMX;

	// Token: 0x0400087F RID: 2175
	private int lastClickCMY;

	// Token: 0x04000880 RID: 2176
	private int clickMovingP1;

	// Token: 0x04000881 RID: 2177
	private int clickMovingTimeOut;

	// Token: 0x04000882 RID: 2178
	private long lastMove;

	// Token: 0x04000883 RID: 2179
	public static bool isNewClanMessage;

	// Token: 0x04000884 RID: 2180
	private long lastFire;

	// Token: 0x04000885 RID: 2181
	private long lastUsePotion;

	// Token: 0x04000886 RID: 2182
	public int auto;

	// Token: 0x04000887 RID: 2183
	public int dem;

	// Token: 0x04000888 RID: 2184
	private string strTam = string.Empty;

	// Token: 0x04000889 RID: 2185
	private int a;

	// Token: 0x0400088A RID: 2186
	public bool isFreez;

	// Token: 0x0400088B RID: 2187
	public bool isUseFreez;

	// Token: 0x0400088C RID: 2188
	public static Image imgTrans;

	// Token: 0x0400088D RID: 2189
	public bool isRongThanXuatHien;

	// Token: 0x0400088E RID: 2190
	public bool isRongNamek;

	// Token: 0x0400088F RID: 2191
	public bool isSuperPower;

	// Token: 0x04000890 RID: 2192
	public int tPower;

	// Token: 0x04000891 RID: 2193
	public int xPower;

	// Token: 0x04000892 RID: 2194
	public int yPower;

	// Token: 0x04000893 RID: 2195
	public int dxPower;

	// Token: 0x04000894 RID: 2196
	public bool activeRongThan;

	// Token: 0x04000895 RID: 2197
	public bool isMeCallRongThan;

	// Token: 0x04000896 RID: 2198
	public int mautroi;

	// Token: 0x04000897 RID: 2199
	public int mapRID;

	// Token: 0x04000898 RID: 2200
	public int zoneRID;

	// Token: 0x04000899 RID: 2201
	public int bgRID = -1;

	// Token: 0x0400089A RID: 2202
	public static int tam = 0;

	// Token: 0x0400089B RID: 2203
	public static bool isAutoPlay;

	// Token: 0x0400089C RID: 2204
	public static bool canAutoPlay;

	// Token: 0x0400089D RID: 2205
	public static bool isChangeZone;

	// Token: 0x0400089E RID: 2206
	private int timeSkill;

	// Token: 0x0400089F RID: 2207
	private int nSkill;

	// Token: 0x040008A0 RID: 2208
	private int selectedIndexSkill = -1;

	// Token: 0x040008A1 RID: 2209
	private Skill lastSkill;

	// Token: 0x040008A2 RID: 2210
	private bool doSeleckSkillFlag;

	// Token: 0x040008A3 RID: 2211
	public string strCapcha;

	// Token: 0x040008A4 RID: 2212
	private long longPress;

	// Token: 0x040008A5 RID: 2213
	private int move;

	// Token: 0x040008A6 RID: 2214
	public bool flareFindFocus;

	// Token: 0x040008A7 RID: 2215
	private int flareTime;

	// Token: 0x040008A8 RID: 2216
	public int keyTouchSkill = -1;

	// Token: 0x040008A9 RID: 2217
	private long lastSendUpdatePostion;

	// Token: 0x040008AA RID: 2218
	public static long lastTick;

	// Token: 0x040008AB RID: 2219
	public static long currTick;

	// Token: 0x040008AC RID: 2220
	private int timeAuto;

	// Token: 0x040008AD RID: 2221
	public static long lastXS;

	// Token: 0x040008AE RID: 2222
	public static long currXS;

	// Token: 0x040008AF RID: 2223
	public static int secondXS;

	// Token: 0x040008B0 RID: 2224
	public int runArrow;

	// Token: 0x040008B1 RID: 2225
	public static int isPaintRada;

	// Token: 0x040008B2 RID: 2226
	public static Image imgNut;

	// Token: 0x040008B3 RID: 2227
	public static Image imgNutF;

	// Token: 0x040008B4 RID: 2228
	public int[] keyCapcha;

	// Token: 0x040008B5 RID: 2229
	public static Image imgCapcha;

	// Token: 0x040008B6 RID: 2230
	public string keyInput;

	// Token: 0x040008B7 RID: 2231
	public static int disXC;

	// Token: 0x040008B8 RID: 2232
	public static bool isPaint = true;

	// Token: 0x040008B9 RID: 2233
	public static int shock_scr;

	// Token: 0x040008BA RID: 2234
	private static int[] shock_x = new int[] { 1, -1, 1, -1 };

	// Token: 0x040008BB RID: 2235
	private static int[] shock_y = new int[] { 1, -1, -1, 1 };

	// Token: 0x040008BC RID: 2236
	private int tDoubleDelay;

	// Token: 0x040008BD RID: 2237
	public static Image arrow;

	// Token: 0x040008BE RID: 2238
	private static int yTouchBar;

	// Token: 0x040008BF RID: 2239
	private static int xC;

	// Token: 0x040008C0 RID: 2240
	private static int yC;

	// Token: 0x040008C1 RID: 2241
	private static int xL;

	// Token: 0x040008C2 RID: 2242
	private static int yL;

	// Token: 0x040008C3 RID: 2243
	public int xR;

	// Token: 0x040008C4 RID: 2244
	public int yR;

	// Token: 0x040008C5 RID: 2245
	private static int xU;

	// Token: 0x040008C6 RID: 2246
	private static int yU;

	// Token: 0x040008C7 RID: 2247
	private static int xF;

	// Token: 0x040008C8 RID: 2248
	private static int yF;

	// Token: 0x040008C9 RID: 2249
	public static int xHP;

	// Token: 0x040008CA RID: 2250
	public static int yHP;

	// Token: 0x040008CB RID: 2251
	private static int xTG;

	// Token: 0x040008CC RID: 2252
	private static int yTG;

	// Token: 0x040008CD RID: 2253
	public static int[] xS;

	// Token: 0x040008CE RID: 2254
	public static int[] yS;

	// Token: 0x040008CF RID: 2255
	public static int xSkill;

	// Token: 0x040008D0 RID: 2256
	public static int ySkill;

	// Token: 0x040008D1 RID: 2257
	public static int padSkill;

	// Token: 0x040008D2 RID: 2258
	public long dMP;

	// Token: 0x040008D3 RID: 2259
	public long twMp;

	// Token: 0x040008D4 RID: 2260
	public bool isInjureMp;

	// Token: 0x040008D5 RID: 2261
	public long dHP;

	// Token: 0x040008D6 RID: 2262
	public long twHp;

	// Token: 0x040008D7 RID: 2263
	public bool isInjureHp;

	// Token: 0x040008D8 RID: 2264
	private long curr;

	// Token: 0x040008D9 RID: 2265
	private long last;

	// Token: 0x040008DA RID: 2266
	private int secondVS;

	// Token: 0x040008DB RID: 2267
	private int[] idVS = new int[] { -1, -1 };

	// Token: 0x040008DC RID: 2268
	public static string[] flyTextString;

	// Token: 0x040008DD RID: 2269
	public static int[] flyTextX;

	// Token: 0x040008DE RID: 2270
	public static int[] flyTextY;

	// Token: 0x040008DF RID: 2271
	public static int[] flyTextYTo;

	// Token: 0x040008E0 RID: 2272
	public static int[] flyTextDx;

	// Token: 0x040008E1 RID: 2273
	public static int[] flyTextDy;

	// Token: 0x040008E2 RID: 2274
	public static int[] flyTextState;

	// Token: 0x040008E3 RID: 2275
	public static int[] flyTextColor;

	// Token: 0x040008E4 RID: 2276
	public static int[] flyTime;

	// Token: 0x040008E5 RID: 2277
	public static int[] splashX;

	// Token: 0x040008E6 RID: 2278
	public static int[] splashY;

	// Token: 0x040008E7 RID: 2279
	public static int[] splashState;

	// Token: 0x040008E8 RID: 2280
	public static int[] splashF;

	// Token: 0x040008E9 RID: 2281
	public static int[] splashDir;

	// Token: 0x040008EA RID: 2282
	public static Image[] imgSplash;

	// Token: 0x040008EB RID: 2283
	public static int cmdBarX;

	// Token: 0x040008EC RID: 2284
	public static int cmdBarY;

	// Token: 0x040008ED RID: 2285
	public static int cmdBarW;

	// Token: 0x040008EE RID: 2286
	public static int cmdBarLeftW;

	// Token: 0x040008EF RID: 2287
	public static int cmdBarRightW;

	// Token: 0x040008F0 RID: 2288
	public static int cmdBarCenterW;

	// Token: 0x040008F1 RID: 2289
	public static int hpBarX;

	// Token: 0x040008F2 RID: 2290
	public static int hpBarY;

	// Token: 0x040008F3 RID: 2291
	public static int spBarW;

	// Token: 0x040008F4 RID: 2292
	public static int mpBarW;

	// Token: 0x040008F5 RID: 2293
	public static int expBarW;

	// Token: 0x040008F6 RID: 2294
	public static int lvPosX;

	// Token: 0x040008F7 RID: 2295
	public static int moneyPosX;

	// Token: 0x040008F8 RID: 2296
	public static int hpBarH;

	// Token: 0x040008F9 RID: 2297
	public static int girlHPBarY;

	// Token: 0x040008FA RID: 2298
	public static long hpBarW;

	// Token: 0x040008FB RID: 2299
	public static Image[] imgCmdBar;

	// Token: 0x040008FC RID: 2300
	private int imgScrW;

	// Token: 0x040008FD RID: 2301
	public static int popupY;

	// Token: 0x040008FE RID: 2302
	public static int popupX;

	// Token: 0x040008FF RID: 2303
	public static int isborderIndex;

	// Token: 0x04000900 RID: 2304
	public static int isselectedRow;

	// Token: 0x04000901 RID: 2305
	private static Image imgNolearn;

	// Token: 0x04000902 RID: 2306
	public int cmxp;

	// Token: 0x04000903 RID: 2307
	public int cmvxp;

	// Token: 0x04000904 RID: 2308
	public int cmdxp;

	// Token: 0x04000905 RID: 2309
	public int cmxLimp;

	// Token: 0x04000906 RID: 2310
	public int cmyLimp;

	// Token: 0x04000907 RID: 2311
	public int cmyp;

	// Token: 0x04000908 RID: 2312
	public int cmvyp;

	// Token: 0x04000909 RID: 2313
	public int cmdyp;

	// Token: 0x0400090A RID: 2314
	private int indexTiemNang;

	// Token: 0x0400090B RID: 2315
	private string alertURL;

	// Token: 0x0400090C RID: 2316
	private string fnick;

	// Token: 0x0400090D RID: 2317
	public static int xstart;

	// Token: 0x0400090E RID: 2318
	public static int ystart;

	// Token: 0x0400090F RID: 2319
	public static int popupW = 140;

	// Token: 0x04000910 RID: 2320
	public static int popupH = 160;

	// Token: 0x04000911 RID: 2321
	public static int cmySK;

	// Token: 0x04000912 RID: 2322
	public static int cmtoYSK;

	// Token: 0x04000913 RID: 2323
	public static int cmdySK;

	// Token: 0x04000914 RID: 2324
	public static int cmvySK;

	// Token: 0x04000915 RID: 2325
	public static int cmyLimSK;

	// Token: 0x04000916 RID: 2326
	public static int columns = 6;

	// Token: 0x04000917 RID: 2327
	public static int rows;

	// Token: 0x04000918 RID: 2328
	private int totalRowInfo;

	// Token: 0x04000919 RID: 2329
	private int ypaintKill;

	// Token: 0x0400091A RID: 2330
	private int ylimUp;

	// Token: 0x0400091B RID: 2331
	private int ylimDow;

	// Token: 0x0400091C RID: 2332
	private int yPaint;

	// Token: 0x0400091D RID: 2333
	public static int indexEff = 0;

	// Token: 0x0400091E RID: 2334
	public static EffectCharPaint effUpok;

	// Token: 0x0400091F RID: 2335
	public static int inforX;

	// Token: 0x04000920 RID: 2336
	public static int inforY;

	// Token: 0x04000921 RID: 2337
	public static int inforW;

	// Token: 0x04000922 RID: 2338
	public static int inforH;

	// Token: 0x04000923 RID: 2339
	public Command cmdDead;

	// Token: 0x04000924 RID: 2340
	public static bool notPaint = false;

	// Token: 0x04000925 RID: 2341
	public static bool isPing = false;

	// Token: 0x04000926 RID: 2342
	public static int INFO = 0;

	// Token: 0x04000927 RID: 2343
	public static int STORE = 1;

	// Token: 0x04000928 RID: 2344
	public static int ZONE = 2;

	// Token: 0x04000929 RID: 2345
	public static int UPGRADE = 3;

	// Token: 0x0400092A RID: 2346
	private int Hitem = 30;

	// Token: 0x0400092B RID: 2347
	private int maxSizeRow = 5;

	// Token: 0x0400092C RID: 2348
	private int isTranKyNang;

	// Token: 0x0400092D RID: 2349
	private bool isTran;

	// Token: 0x0400092E RID: 2350
	private int cmY_Old;

	// Token: 0x0400092F RID: 2351
	private int cmX_Old;

	// Token: 0x04000930 RID: 2352
	public PopUpYesNo popUpYesNo;

	// Token: 0x04000931 RID: 2353
	public static MyVector vChatVip = new MyVector();

	// Token: 0x04000932 RID: 2354
	public static int vBig;

	// Token: 0x04000933 RID: 2355
	public bool isFireWorks;

	// Token: 0x04000934 RID: 2356
	public int[] winnumber;

	// Token: 0x04000935 RID: 2357
	public int[] randomNumber;

	// Token: 0x04000936 RID: 2358
	public int[] tMove;

	// Token: 0x04000937 RID: 2359
	public int[] moveCount;

	// Token: 0x04000938 RID: 2360
	public int[] delayMove;

	// Token: 0x04000939 RID: 2361
	public int moveIndex;

	// Token: 0x0400093A RID: 2362
	private bool isWin;

	// Token: 0x0400093B RID: 2363
	private string strFinish;

	// Token: 0x0400093C RID: 2364
	private int tShow;

	// Token: 0x0400093D RID: 2365
	private int xChatVip;

	// Token: 0x0400093E RID: 2366
	private int currChatWidth;

	// Token: 0x0400093F RID: 2367
	private bool startChat;

	// Token: 0x04000940 RID: 2368
	public sbyte percentMabu;

	// Token: 0x04000941 RID: 2369
	public bool mabuEff;

	// Token: 0x04000942 RID: 2370
	public int tMabuEff;

	// Token: 0x04000943 RID: 2371
	public static bool isPaintChatVip;

	// Token: 0x04000944 RID: 2372
	public static sbyte mabuPercent;

	// Token: 0x04000945 RID: 2373
	public static sbyte isNewMember;

	// Token: 0x04000946 RID: 2374
	private string yourNumber = string.Empty;

	// Token: 0x04000947 RID: 2375
	private string[] strPaint;

	// Token: 0x04000948 RID: 2376
	public static Image imgHP_NEW;

	// Token: 0x04000949 RID: 2377
	public static InfoPhuBan phuban_Info;

	// Token: 0x0400094A RID: 2378
	public static FrameImage fra_PVE_Bar_0;

	// Token: 0x0400094B RID: 2379
	public static FrameImage fra_PVE_Bar_1;

	// Token: 0x0400094C RID: 2380
	public static Image imgVS;

	// Token: 0x0400094D RID: 2381
	public static Image imgBall;

	// Token: 0x0400094E RID: 2382
	public static Image imgKhung;

	// Token: 0x0400094F RID: 2383
	public int countFrameSkill;

	// Token: 0x04000950 RID: 2384
	public static Image imgBgIOS;

	// Token: 0x04000951 RID: 2385
	public static int nCT_TeamB = 50;

	// Token: 0x04000952 RID: 2386
	public static int nCT_TeamA = 50;

	// Token: 0x04000953 RID: 2387
	public static long nCT_timeBallte;

	// Token: 0x04000954 RID: 2388
	public static string nCT_team;

	// Token: 0x04000955 RID: 2389
	public static int nCT_nBoyBaller = 100;

	// Token: 0x04000956 RID: 2390
	public static bool isPaint_CT;

	// Token: 0x04000957 RID: 2391
	public static sbyte nCT_floor;

	// Token: 0x04000958 RID: 2392
	public static bool is_Paint_boardCT_Expand;

	// Token: 0x04000959 RID: 2393
	private static int xRect;

	// Token: 0x0400095A RID: 2394
	private static int yRect;

	// Token: 0x0400095B RID: 2395
	private static int wRect;

	// Token: 0x0400095C RID: 2396
	private static int hRect;

	// Token: 0x0400095D RID: 2397
	public static MyVector res_CT = new MyVector();

	// Token: 0x0400095E RID: 2398
	public static int nTop = 1;

	// Token: 0x0400095F RID: 2399
	public static bool isPickNgocRong = false;

	// Token: 0x04000960 RID: 2400
	public static int nUSER_CT;

	// Token: 0x04000961 RID: 2401
	public static int nUSER_MAX_CT;

	// Token: 0x04000962 RID: 2402
	public static bool isudungCapsun;

	// Token: 0x04000963 RID: 2403
	public static bool isudungCapsun4;

	// Token: 0x04000964 RID: 2404
	public static bool isudungCapsun3;

	// Token: 0x04000965 RID: 2405
	public static long timehoichieubuff;

	// Token: 0x04000966 RID: 2406
	public static long timehoikhien;

	// Token: 0x04000967 RID: 2407
	public static long timehoiskill3;

	// Token: 0x04000968 RID: 2408
	public static long timehoiskill9;

	// Token: 0x04000969 RID: 2409
	public static long timehoibom;

	// Token: 0x0400096A RID: 2410
	public static long timehoithoimien;

	// Token: 0x0400096B RID: 2411
	public static long ccc1;

	// Token: 0x0400096C RID: 2412
	public static long ccc3;

	// Token: 0x0400096D RID: 2413
	public static long ccc5;
}
using System;
using AssemblyCSharp.Mod.PickMob;
using AssemblyCSharp.Mod.Xmap;
using Mod.DungPham.KoiOctiiu957;

// Token: 0x02000016 RID: 22
internal class KsSupper
{
	// Token: 0x06000080 RID: 128 RVA: 0x00009E24 File Offset: 0x00008024
	public static bool IsBoss()
	{
		for (int i = 0; i < GameScr.vCharInMap.size(); i++)
		{
			global::Char @char = (global::Char)GameScr.vCharInMap.elementAt(i);
			bool flag = @char != null && @char.cName.Contains("Broly") && @char.cName.Contains("Super") && @char.cHPFull >= 16070777L;
			if (flag)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000081 RID: 129 RVA: 0x0000A5C0 File Offset: 0x000087C0
	public static void DoKhuNgauNhien()
	{
		bool flag = KsSupper.IsBoss() || Pk9rXmap.IsXmapRunning;
		if (!flag)
		{
			Service.gI().requestChangeZone(-1, -1);
		}
	}

	// Token: 0x06000082 RID: 130 RVA: 0x0000A5F4 File Offset: 0x000087F4
	public static void FocusSuperBroly()
	{
		for (int i = 0; i < GameScr.vCharInMap.size(); i++)
		{
			global::Char @char = (global::Char)GameScr.vCharInMap.elementAt(i);
			bool flag = @char != null && @char.cName.Contains("Broly") && @char.cName.Contains("Super") && @char.cHP > 0L;
			if (flag)
			{
				bool flag2 = global::Char.myCharz().charFocus != @char;
				if (flag2)
				{
					global::Char.myCharz().npcFocus = null;
					global::Char.myCharz().mobFocus = null;
					global::Char.myCharz().charFocus = @char;
					break;
				}
				bool flag3 = Res.distance(@char.cx, @char.cy, global::Char.myCharz().cx, global::Char.myCharz().cy) > 200 && @char.cx > 500 && @char.cHP > 2L;
				if (flag3)
				{
					KsSupper.Move(@char.cx - 100, @char.cy);
					break;
				}
				bool flag4 = Res.distance(@char.cx, @char.cy, global::Char.myCharz().cx, global::Char.myCharz().cy) > 200 && @char.cx < 500 && @char.cHP > 2L;
				if (flag4)
				{
					KsSupper.Move(@char.cx + 100, @char.cy);
					break;
				}
				bool flag5 = Res.distance(@char.cx, @char.cy, global::Char.myCharz().cx, global::Char.myCharz().cy) < 80 && @char.cx < 500 && @char.cHP > 2L;
				if (flag5)
				{
					KsSupper.Move(@char.cx + 100, @char.cy);
					break;
				}
				bool flag6 = Res.distance(@char.cx, @char.cy, global::Char.myCharz().cx, global::Char.myCharz().cy) < 80 && @char.cx > 500 && @char.cHP > 2L;
				if (flag6)
				{
					KsSupper.Move(@char.cx - 100, @char.cy);
					break;
				}
			}
		}
	}

	// Token: 0x06000083 RID: 131 RVA: 0x0000A840 File Offset: 0x00008A40
	private static void Move(int x, int y)
	{
		global::Char @char = global::Char.myCharz();
		bool flag = !Pk9rPickMob.IsVuotDiaHinh;
		if (flag)
		{
			@char.currentMovePoint = new MovePoint(x, y);
		}
		else
		{
			int[] pointYsdMax = KsSupper.GetPointYsdMax(@char.cx, x);
			bool flag2 = pointYsdMax[1] >= y || (pointYsdMax[1] >= @char.cy && (@char.statusMe == 2 || @char.statusMe == 1));
			if (flag2)
			{
				pointYsdMax[0] = x;
				pointYsdMax[1] = y;
			}
			@char.currentMovePoint = new MovePoint(pointYsdMax[0], pointYsdMax[1]);
		}
	}

	// Token: 0x06000084 RID: 132 RVA: 0x0000A8C8 File Offset: 0x00008AC8
	private static int GetYsd(int xsd)
	{
		global::Char @char = global::Char.myCharz();
		int num = TileMap.pxh;
		int num2 = -1;
		for (int i = 24; i < TileMap.pxh; i += 24)
		{
			bool flag = TileMap.tileTypeAt(xsd, i, 2);
			if (flag)
			{
				int num3 = Res.abs(i - @char.cy);
				bool flag2 = num3 < num;
				if (flag2)
				{
					num = num3;
					num2 = i;
				}
			}
		}
		return num2;
	}

	// Token: 0x06000085 RID: 133 RVA: 0x0000A938 File Offset: 0x00008B38
	private static int[] GetPointYsdMax(int xStart, int xEnd)
	{
		int num = TileMap.pxh;
		int num2 = -1;
		bool flag = xStart > xEnd;
		if (flag)
		{
			for (int i = xEnd; i < xStart; i += 24)
			{
				int ysd = KsSupper.GetYsd(i);
				bool flag2 = ysd < num;
				if (flag2)
				{
					num = ysd;
					num2 = i;
				}
			}
		}
		else
		{
			for (int j = xEnd; j > xStart; j -= 24)
			{
				int ysd2 = KsSupper.GetYsd(j);
				bool flag3 = ysd2 < num;
				if (flag3)
				{
					num = ysd2;
					num2 = j;
				}
			}
		}
		return new int[] { num2, num };
	}

	// Token: 0x06000086 RID: 134 RVA: 0x0000A9D4 File Offset: 0x00008BD4
	public static void TelePortTo(int x, int y)
	{
		global::Char.myCharz().cx = x;
		global::Char.myCharz().cy = y;
		Service.gI().charMove();
		bool flag = ItemTime.isExistItem(4387);
		if (!flag)
		{
			global::Char.myCharz().cx = x;
			global::Char.myCharz().cy = y + 1;
			Service.gI().charMove();
			global::Char.myCharz().cx = x;
			global::Char.myCharz().cy = y;
			Service.gI().charMove();
		}
	}

	// Token: 0x06000087 RID: 135 RVA: 0x0000AA58 File Offset: 0x00008C58
	public static void Ks()
	{
		global::Char charFocus = global::Char.myCharz().charFocus;
		bool flag = Res.distance(charFocus.cx, charFocus.cy, global::Char.myCharz().cx, global::Char.myCharz().cy) > 100 && charFocus.cHP < 2L && charFocus.cHP > 0L;
		if (flag)
		{
			KsSupper.TelePortTo(charFocus.cx, charFocus.cy);
		}
		bool flag2 = Res.distance(charFocus.cx, charFocus.cy, global::Char.myCharz().cx, global::Char.myCharz().cy) < 100 && charFocus.cHP < 2L && charFocus.cHP > 0L;
		if (flag2)
		{
			AutoSkill.AutoSendAttack();
		}
	}

	// Token: 0x06000088 RID: 136 RVA: 0x0000AB18 File Offset: 0x00008D18
	public static void autoitem()
	{
		global::Char @char = global::Char.myCharz();
		for (int i = 0; i < GameScr.vItemMap.size(); i++)
		{
			ItemMap itemMap = (ItemMap)GameScr.vItemMap.elementAt(i);
			bool flag = itemMap != null;
			if (flag)
			{
				bool flag2 = itemMap.playerId == @char.charID;
				if (flag2)
				{
					bool flag3 = Res.abs(itemMap.xEnd - @char.cx) > 25;
					if (flag3)
					{
						KsSupper.Move(itemMap.xEnd, itemMap.yEnd);
					}
					Service.gI().pickItem(itemMap.itemMapID);
					return;
				}
			}
		}
		for (int j = 0; j < GameScr.vItemMap.size(); j++)
		{
			ItemMap itemMap2 = (ItemMap)GameScr.vItemMap.elementAt(j);
			bool flag4 = itemMap2 != null && itemMap2.playerId == -1;
			if (flag4)
			{
				bool flag5 = Res.abs(itemMap2.xEnd - @char.cx) > 25;
				if (flag5)
				{
					KsSupper.Move(itemMap2.xEnd, itemMap2.yEnd);
				}
				Service.gI().pickItem(itemMap2.itemMapID);
				break;
			}
		}
	}

	// Token: 0x06000089 RID: 137 RVA: 0x0000AC58 File Offset: 0x00008E58
	public static void Update()
	{
		bool flag = KsSupper.IsBoss();
		if (flag)
		{
			KsSupper.FocusSuperBroly();
			bool flag2 = DataAccount.Type == 3;
			if (flag2)
			{
				KsSupper.Ks();
			}
		}
	}
}
using System;
using System.Collections.Generic;

namespace AssemblyCSharp.Mod.PickMob
{
	// Token: 0x02000108 RID: 264
	public class Pk9rPickMob
	{
		// Token: 0x06000CDE RID: 3294 RVA: 0x00006EEE File Offset: 0x000050EE
		public static void Update()
		{
			PickMobController.Update();
		}

		// Token: 0x06000CDF RID: 3295 RVA: 0x000C50D8 File Offset: 0x000C32D8
		public static void MobStartDie(object obj)
		{
			Mob mob = (Mob)obj;
			bool flag = mob.status != 1 && mob.status != 0;
			if (flag)
			{
				mob.timeLastDie = mSystem.currentTimeMillis();
				mob.countDie++;
				bool flag2 = mob.countDie > 10;
				if (flag2)
				{
					mob.countDie = 0;
				}
			}
		}

		// Token: 0x06000CE0 RID: 3296 RVA: 0x000C5138 File Offset: 0x000C3338
		public static void UpdateCountDieMob(Mob mob)
		{
			bool flag = mob.levelBoss != 0;
			if (flag)
			{
				mob.countDie = 0;
			}
		}

		// Token: 0x040015ED RID: 5613
		private static readonly sbyte[] IdSkillsBase = new sbyte[] { 0, 2, 17, 4, 12 };

		// Token: 0x040015EE RID: 5614
		private static readonly short[] IdItemBlockBase = new short[]
		{
			18, 19, 20, 226, 1460, 1758, 930, 931, 1459, 901,
			702, 703, 704, 705, 706, 707, 708, 441, 442, 443,
			444, 445, 446, 447, 220, 221, 222, 223, 224, 225,
			353, 354, 355, 356, 357, 358, 359, 360, 362
		};

		// Token: 0x040015EF RID: 5615
		public static bool IsTanSat = false;

		// Token: 0x040015F0 RID: 5616
		public static bool IsNeSieuQuai = true;

		// Token: 0x040015F1 RID: 5617
		public static bool IsVuotDiaHinh = true;

		// Token: 0x040015F2 RID: 5618
		private static readonly int[] CanhDongTuyet = new int[] { 0, 1, 2, 3 };

		// Token: 0x040015F3 RID: 5619
		private static readonly int[] RungTuyet = new int[] { 0, 1, 2, 3, 4, 5 };

		// Token: 0x040015F4 RID: 5620
		private static readonly int[] RungBang = new int[] { 0, 4, 5, 6, 7, 8, 9, 10 };

		// Token: 0x040015F5 RID: 5621
		private static readonly int[] DongSongBang = new int[] { 4, 5, 6, 7, 8 };

		// Token: 0x040015F6 RID: 5622
		private static readonly int[] NuiTuyet = new int[] { 6, 7, 8, 9 };

		// Token: 0x040015F7 RID: 5623
		private static readonly int[] HangBang = new int[] { 1, 2, 10 };

		// Token: 0x040015F8 RID: 5624
		private static readonly int[] Lmao = new int[] { 82, 83, 84 };

		// Token: 0x040015F9 RID: 5625
		public static List<int> IdMobsTanSat = new List<int>();

		// Token: 0x040015FA RID: 5626
		public static List<int> TypeMobsTanSat = new List<int>(Pk9rPickMob.Lmao);

		// Token: 0x040015FB RID: 5627
		public static List<sbyte> IdSkillsTanSat = new List<sbyte>(Pk9rPickMob.IdSkillsBase);

		// Token: 0x040015FC RID: 5628
		public static bool IsAutoPickItems = true;

		// Token: 0x040015FD RID: 5629
		public static bool IsItemMe = true;

		// Token: 0x040015FE RID: 5630
		public static bool IsLimitTimesPickItem = true;

		// Token: 0x040015FF RID: 5631
		public static int TimesAutoPickItemMax = 7;

		// Token: 0x04001600 RID: 5632
		public static List<short> IdItemPicks = new List<short>();

		// Token: 0x04001601 RID: 5633
		public static List<short> IdItemBlocks = new List<short>(Pk9rPickMob.IdItemBlockBase);

		// Token: 0x04001602 RID: 5634
		public static List<sbyte> TypeItemPicks = new List<sbyte>();

		// Token: 0x04001603 RID: 5635
		public static List<sbyte> TypeItemBlock = new List<sbyte>();

		// Token: 0x04001604 RID: 5636
		public static int HpBuff = 10;

		// Token: 0x04001605 RID: 5637
		public static int MpBuff = 10;

		// Token: 0x04001606 RID: 5638
		public static List<sbyte> IdskillUses = new List<sbyte>();
	}
}
using System;
using System.Collections.Generic;

namespace AssemblyCSharp.Mod.Xmap
{
	// Token: 0x020000FE RID: 254
	public struct GroupMap
	{
		// Token: 0x06000C77 RID: 3191 RVA: 0x00006D3E File Offset: 0x00004F3E
		public GroupMap(string nameGroup, List<int> idMaps)
		{
			this.NameGroup = nameGroup;
			this.IdMaps = idMaps;
		}

		// Token: 0x040015AC RID: 5548
		public string NameGroup;

		// Token: 0x040015AD RID: 5549
		public List<int> IdMaps;
	}
}
using System;

namespace AssemblyCSharp.Mod.Xmap
{
	// Token: 0x020000FF RID: 255
	public struct MapNext
	{
		// Token: 0x06000C78 RID: 3192 RVA: 0x00006D4F File Offset: 0x00004F4F
		public MapNext(int mapID, TypeMapNext type, int[] info)
		{
			this.MapID = mapID;
			this.Type = type;
			this.Info = info;
		}

		// Token: 0x040015AE RID: 5550
		public int MapID;

		// Token: 0x040015AF RID: 5551
		public TypeMapNext Type;

		// Token: 0x040015B0 RID: 5552
		public int[] Info;
	}
}
using System;

namespace AssemblyCSharp.Mod.Xmap
{
	// Token: 0x02000100 RID: 256
	public class Pk9rXmap
	{
		// Token: 0x06000C79 RID: 3193 RVA: 0x000C2EA4 File Offset: 0x000C10A4
		public static bool Chat(string text)
		{
			bool flag = text == "xmp";
			if (flag)
			{
				bool isXmapRunning = Pk9rXmap.IsXmapRunning;
				if (isXmapRunning)
				{
					XmapController.FinishXmap();
					GameScr.info1.addInfo("Đã huỷ Xmap", 0);
				}
				else
				{
					XmapController.ShowXmapMenu();
				}
			}
			else
			{
				bool flag2 = Pk9rXmap.IsGetInfoChat<int>(text, "xmp");
				if (flag2)
				{
					bool isXmapRunning2 = Pk9rXmap.IsXmapRunning;
					if (isXmapRunning2)
					{
						XmapController.FinishXmap();
						GameScr.info1.addInfo("Đã huỷ Xmap", 0);
					}
					else
					{
						XmapController.StartRunToMapId(Pk9rXmap.GetInfoChat<int>(text, "xmp"));
					}
				}
				else
				{
					bool flag3 = text == "csb";
					if (flag3)
					{
						Pk9rXmap.IsUseCapsuleNormal = !Pk9rXmap.IsUseCapsuleNormal;
						GameScr.info1.addInfo("Sử dụng capsule thường Xmap: " + (Pk9rXmap.IsUseCapsuleNormal ? "Bật" : "Tắt"), 0);
					}
					else
					{
						bool flag4 = !(text == "csdb");
						if (flag4)
						{
							return false;
						}
						Pk9rXmap.IsUseCapsuleVip = !Pk9rXmap.IsUseCapsuleVip;
						GameScr.info1.addInfo("Sử dụng capsule đặc biệt Xmap: " + (Pk9rXmap.IsUseCapsuleVip ? "Bật" : "Tắt"), 0);
					}
				}
			}
			return true;
		}

		// Token: 0x06000C7A RID: 3194 RVA: 0x000C2FE8 File Offset: 0x000C11E8
		public static bool HotKeys()
		{
			int keyAsciiPress = GameCanvas.keyAsciiPress;
			int num = keyAsciiPress;
			if (num != 99)
			{
				if (num != 120)
				{
					return false;
				}
				Pk9rXmap.Chat("xmp");
			}
			else
			{
				Pk9rXmap.Chat("csb");
			}
			return true;
		}

		// Token: 0x06000C7B RID: 3195 RVA: 0x000C3030 File Offset: 0x000C1230
		public static void Update()
		{
			bool isLoading = XmapData.Instance().IsLoading;
			if (isLoading)
			{
				XmapData.Instance().Update();
			}
			bool isXmapRunning = Pk9rXmap.IsXmapRunning;
			if (isXmapRunning)
			{
				XmapController.Update();
			}
		}

		// Token: 0x06000C7C RID: 3196 RVA: 0x000C306C File Offset: 0x000C126C
		public static void Info(string text)
		{
			bool flag = text.Equals("Bạn chưa thể đến khu vực này");
			if (flag)
			{
				XmapController.FinishXmap();
			}
			bool flag2 = text.Equals("Vui lòng chờ một lát nữa");
			if (flag2)
			{
				XmapController.FinishXmap();
			}
		}

		// Token: 0x06000C7D RID: 3197 RVA: 0x000C30A8 File Offset: 0x000C12A8
		public static bool XoaTauBay(object obj)
		{
			Teleport teleport = (Teleport)obj;
			bool isMe = teleport.isMe;
			bool flag2;
			if (isMe)
			{
				global::Char.myCharz().isTeleport = false;
				bool flag = teleport.type == 0;
				if (flag)
				{
					Controller.isStopReadMessage = false;
					global::Char.ischangingMap = true;
				}
				Teleport.vTeleport.removeElement(teleport);
				flag2 = true;
			}
			else
			{
				flag2 = false;
			}
			return flag2;
		}

		// Token: 0x06000C7E RID: 3198 RVA: 0x000C3104 File Offset: 0x000C1304
		public static void SelectMapTrans(int selected)
		{
			bool isMapTransAsXmap = Pk9rXmap.IsMapTransAsXmap;
			if (isMapTransAsXmap)
			{
				XmapController.HideInfoDlg();
				XmapController.StartRunToMapId(XmapData.GetIdMapFromPanelXmap(GameCanvas.panel.mapNames[selected]));
			}
			else
			{
				XmapController.SaveIdMapCapsuleReturn();
				Service.gI().requestMapSelect(selected);
			}
		}

		// Token: 0x06000C7F RID: 3199 RVA: 0x000C3150 File Offset: 0x000C1350
		public static void ShowPanelMapTrans()
		{
			Pk9rXmap.IsMapTransAsXmap = false;
			bool isShowPanelMapTrans = Pk9rXmap.IsShowPanelMapTrans;
			if (isShowPanelMapTrans)
			{
				GameCanvas.panel.setTypeMapTrans();
				GameCanvas.panel.show();
			}
			else
			{
				Pk9rXmap.IsShowPanelMapTrans = true;
			}
		}

		// Token: 0x06000C80 RID: 3200 RVA: 0x00006D67 File Offset: 0x00004F67
		public static void FixBlackScreen()
		{
			Controller.gI().loadCurrMap(0);
			Service.gI().finishLoadMap();
			global::Char.isLoadingMap = false;
		}

		// Token: 0x06000C81 RID: 3201 RVA: 0x000C3190 File Offset: 0x000C1390
		private static bool IsGetInfoChat<T>(string text, string s)
		{
			bool flag = text.StartsWith(s);
			bool flag2;
			if (flag)
			{
				try
				{
					Convert.ChangeType(text.Substring(s.Length), typeof(T));
				}
				catch
				{
					return false;
				}
				flag2 = true;
			}
			else
			{
				flag2 = false;
			}
			return flag2;
		}

		// Token: 0x06000C82 RID: 3202 RVA: 0x000C31EC File Offset: 0x000C13EC
		private static T GetInfoChat<T>(string text, string s)
		{
			return (T)((object)Convert.ChangeType(text.Substring(s.Length), typeof(T)));
		}

		// Token: 0x040015B1 RID: 5553
		public static bool IsXmapRunning = false;

		// Token: 0x040015B2 RID: 5554
		public static bool IsMapTransAsXmap = false;

		// Token: 0x040015B3 RID: 5555
		public static bool IsShowPanelMapTrans = true;

		// Token: 0x040015B4 RID: 5556
		public static bool IsUseCapsuleNormal = true;

		// Token: 0x040015B5 RID: 5557
		public static bool IsUseCapsuleVip = true;

		// Token: 0x040015B6 RID: 5558
		public static int IdMapCapsuleReturn = -1;
	}
}
using System;
using System.Collections.Generic;

namespace AssemblyCSharp.Mod.Xmap
{
	// Token: 0x02000102 RID: 258
	public class XmapAlgorithm
	{
		// Token: 0x06000C85 RID: 3205 RVA: 0x000C3220 File Offset: 0x000C1420
		public static List<int> FindWay(int idMapStart, int idMapEnd)
		{
			List<int> wayPassedStart = XmapAlgorithm.GetWayPassedStart(idMapStart);
			return XmapAlgorithm.FindWay(idMapEnd, wayPassedStart);
		}

		// Token: 0x06000C86 RID: 3206 RVA: 0x000C3240 File Offset: 0x000C1440
		private static List<int> FindWay(int idMapEnd, List<int> wayPassed)
		{
			int num = wayPassed[wayPassed.Count - 1];
			bool flag = num == idMapEnd;
			List<int> list;
			if (flag)
			{
				list = wayPassed;
			}
			else
			{
				bool flag2 = !XmapData.Instance().CanGetMapNexts(num);
				if (flag2)
				{
					list = null;
				}
				else
				{
					List<List<int>> list2 = new List<List<int>>();
					foreach (MapNext mapNext in XmapData.Instance().GetMapNexts(num))
					{
						List<int> list3 = null;
						bool flag3 = !wayPassed.Contains(mapNext.MapID);
						if (flag3)
						{
							List<int> wayPassedNext = XmapAlgorithm.GetWayPassedNext(wayPassed, mapNext.MapID);
							list3 = XmapAlgorithm.FindWay(idMapEnd, wayPassedNext);
						}
						bool flag4 = list3 != null;
						if (flag4)
						{
							list2.Add(list3);
						}
					}
					list = XmapAlgorithm.GetBestWay(list2);
				}
			}
			return list;
		}

		// Token: 0x06000C87 RID: 3207 RVA: 0x000C3330 File Offset: 0x000C1530
		private static List<int> GetBestWay(List<List<int>> ways)
		{
			bool flag = ways.Count == 0;
			List<int> list;
			if (flag)
			{
				list = null;
			}
			else
			{
				List<int> list2 = ways[0];
				for (int i = 1; i < ways.Count; i++)
				{
					bool flag2 = XmapAlgorithm.IsWayBetter(ways[i], list2);
					if (flag2)
					{
						list2 = ways[i];
					}
				}
				list = list2;
			}
			return list;
		}

		// Token: 0x06000C88 RID: 3208 RVA: 0x000C3394 File Offset: 0x000C1594
		private static List<int> GetWayPassedStart(int idMapStart)
		{
			return new List<int> { idMapStart };
		}

		// Token: 0x06000C89 RID: 3209 RVA: 0x000C33B4 File Offset: 0x000C15B4
		private static List<int> GetWayPassedNext(List<int> wayPassed, int idMapNext)
		{
			return new List<int>(wayPassed) { idMapNext };
		}

		// Token: 0x06000C8A RID: 3210 RVA: 0x000C33D4 File Offset: 0x000C15D4
		private static bool IsWayBetter(List<int> way1, List<int> way2)
		{
			bool flag = XmapAlgorithm.IsBadWay(way1);
			bool flag2 = XmapAlgorithm.IsBadWay(way2);
			bool flag3 = !flag || flag2;
			bool flag5;
			if (flag3)
			{
				bool flag4 = flag || !flag2;
				flag5 = !flag4 || way1.Count < way2.Count;
			}
			else
			{
				flag5 = false;
			}
			return flag5;
		}

		// Token: 0x06000C8B RID: 3211 RVA: 0x000C3428 File Offset: 0x000C1628
		private static bool IsBadWay(List<int> way)
		{
			return XmapAlgorithm.IsWayGoFutureAndBack(way);
		}

		// Token: 0x06000C8C RID: 3212 RVA: 0x000C3440 File Offset: 0x000C1640
		private static bool IsWayGoFutureAndBack(List<int> way)
		{
			List<int> list = new List<int> { 27, 28, 29 };
			for (int i = 1; i < way.Count - 1; i++)
			{
				bool flag = way[i] == 102 && way[i + 1] == 24 && list.Contains(way[i - 1]);
				if (flag)
				{
					return true;
				}
			}
			return false;
		}
	}
}
using System;
using System.Collections.Generic;

namespace AssemblyCSharp.Mod.Xmap
{
	// Token: 0x02000103 RID: 259
	public class XmapController : IActionListener
	{
		// Token: 0x06000C8E RID: 3214 RVA: 0x000C34C4 File Offset: 0x000C16C4
		public static void Update()
		{
			bool flag = XmapController.IsWaiting() || XmapData.Instance().IsLoading;
			if (!flag)
			{
				bool isWaitNextMap = XmapController.IsWaitNextMap;
				if (isWaitNextMap)
				{
					XmapController.Wait(500);
					XmapController.IsWaitNextMap = false;
				}
				else
				{
					bool isNextMapFailed = XmapController.IsNextMapFailed;
					if (isNextMapFailed)
					{
						XmapData.Instance().MyLinkMaps = null;
						XmapController.WayXmap = null;
						XmapController.IsNextMapFailed = false;
					}
					else
					{
						bool flag2 = XmapController.WayXmap == null;
						if (flag2)
						{
							bool flag3 = XmapData.Instance().MyLinkMaps == null;
							if (flag3)
							{
								XmapData.Instance().LoadLinkMaps();
								return;
							}
							XmapController.WayXmap = XmapAlgorithm.FindWay(TileMap.mapID, XmapController.IdMapEnd);
							XmapController.IndexWay = 0;
							bool flag4 = XmapController.WayXmap == null;
							if (flag4)
							{
								GameScr.info1.addInfo("Không thể tìm thấy đường đi", 0);
								XmapController.FinishXmap();
								return;
							}
						}
						bool flag5 = TileMap.mapID == XmapController.WayXmap[XmapController.WayXmap.Count - 1] && !XmapData.IsMyCharDie();
						if (flag5)
						{
							GameScr.info1.addInfo("Đã đến: " + TileMap.mapNames[TileMap.mapID], 0);
							XmapController.FinishXmap();
						}
						else
						{
							bool flag6 = TileMap.mapID == XmapController.WayXmap[XmapController.IndexWay];
							if (flag6)
							{
								bool flag7 = XmapData.IsMyCharDie();
								if (flag7)
								{
									Service.gI().returnTownFromDead();
									XmapController.IsWaitNextMap = (XmapController.IsNextMapFailed = true);
								}
								else
								{
									bool flag8 = XmapData.CanNextMap();
									if (flag8)
									{
										XmapController.NextMap(XmapController.WayXmap[XmapController.IndexWay + 1]);
										XmapController.IsWaitNextMap = true;
									}
								}
								XmapController.Wait(1000);
							}
							else
							{
								bool flag9 = TileMap.mapID == XmapController.WayXmap[XmapController.IndexWay + 1];
								if (flag9)
								{
									XmapController.IndexWay++;
								}
								else
								{
									XmapController.IsNextMapFailed = true;
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000C8F RID: 3215 RVA: 0x000C36BC File Offset: 0x000C18BC
		public void perform(int idAction, object p)
		{
			bool flag = idAction == 1;
			if (flag)
			{
				XmapController.ShowPanelXmap((List<int>)p);
			}
		}

		// Token: 0x06000C90 RID: 3216 RVA: 0x00006DAD File Offset: 0x00004FAD
		private static void Wait(int time)
		{
			XmapController.IsWait = true;
			XmapController.TimeStartWait = mSystem.currentTimeMillis();
			XmapController.TimeWait = (long)time;
		}

		// Token: 0x06000C91 RID: 3217 RVA: 0x000C36E0 File Offset: 0x000C18E0
		private static bool IsWaiting()
		{
			bool flag = XmapController.IsWait && mSystem.currentTimeMillis() - XmapController.TimeStartWait >= XmapController.TimeWait;
			if (flag)
			{
				XmapController.IsWait = false;
			}
			return XmapController.IsWait;
		}

		// Token: 0x06000C92 RID: 3218 RVA: 0x000C3724 File Offset: 0x000C1924
		public static void ShowXmapMenu()
		{
			XmapData.Instance().LoadGroupMapsFromFile("Nro_244_Data\\Resources\\TextData\\GroupMapsXmap.txt");
			MyVector myVector = new MyVector();
			foreach (GroupMap groupMap in XmapData.Instance().GroupMaps)
			{
				myVector.addElement(new Command(groupMap.NameGroup, XmapController._Instance, 1, groupMap.IdMaps));
			}
			GameCanvas.menu.startAt(myVector, 3);
		}

		// Token: 0x06000C93 RID: 3219 RVA: 0x000C37BC File Offset: 0x000C19BC
		public static void ShowPanelXmap(List<int> idMaps)
		{
			Pk9rXmap.IsMapTransAsXmap = true;
			int count = idMaps.Count;
			GameCanvas.panel.mapNames = new string[count];
			GameCanvas.panel.planetNames = new string[count];
			for (int i = 0; i < count; i++)
			{
				string text = TileMap.mapNames[idMaps[i]];
				GameCanvas.panel.mapNames[i] = idMaps[i].ToString() + ": " + text;
				GameCanvas.panel.planetNames[i] = "";
			}
			GameCanvas.panel.setTypeMapTrans();
			GameCanvas.panel.show();
		}

		// Token: 0x06000C94 RID: 3220 RVA: 0x00006DC7 File Offset: 0x00004FC7
		public static void StartRunToMapId(int idMap)
		{
			XmapController.IdMapEnd = idMap;
			Pk9rXmap.IsXmapRunning = true;
		}

		// Token: 0x06000C95 RID: 3221 RVA: 0x00006DD6 File Offset: 0x00004FD6
		public static void FinishXmap()
		{
			Pk9rXmap.IsXmapRunning = false;
			XmapController.IsNextMapFailed = false;
			XmapData.Instance().MyLinkMaps = null;
			XmapController.WayXmap = null;
		}

		// Token: 0x06000C96 RID: 3222 RVA: 0x00006DF6 File Offset: 0x00004FF6
		public static void SaveIdMapCapsuleReturn()
		{
			Pk9rXmap.IdMapCapsuleReturn = TileMap.mapID;
		}

		// Token: 0x06000C97 RID: 3223 RVA: 0x000C3868 File Offset: 0x000C1A68
		private static void NextMap(int idMapNext)
		{
			List<MapNext> mapNexts = XmapData.Instance().GetMapNexts(TileMap.mapID);
			bool flag = mapNexts != null;
			if (flag)
			{
				foreach (MapNext mapNext in mapNexts)
				{
					bool flag2 = mapNext.MapID == idMapNext;
					if (flag2)
					{
						XmapController.NextMap(mapNext);
						return;
					}
				}
			}
			GameScr.info1.addInfo("Lỗi tại dữ liệu", 0);
		}

		// Token: 0x06000C98 RID: 3224 RVA: 0x000C38F8 File Offset: 0x000C1AF8
		private static void NextMap(MapNext mapNext)
		{
			switch (mapNext.Type)
			{
			case TypeMapNext.AutoWaypoint:
				XmapController.NextMapAutoWaypoint(mapNext);
				break;
			case TypeMapNext.NpcMenu:
				XmapController.NextMapNpcMenu(mapNext);
				break;
			case TypeMapNext.NpcPanel:
				XmapController.NextMapNpcPanel(mapNext);
				break;
			case TypeMapNext.Position:
				XmapController.NextMapPosition(mapNext);
				break;
			case TypeMapNext.Capsule:
				XmapController.NextMapCapsule(mapNext);
				break;
			}
		}

		// Token: 0x06000C99 RID: 3225 RVA: 0x000C3958 File Offset: 0x000C1B58
		private static void NextMapAutoWaypoint(MapNext mapNext)
		{
			Waypoint waypoint = XmapData.FindWaypoint(mapNext.MapID);
			bool flag = waypoint != null && TileMap.mapID == 16;
			if (flag)
			{
				waypoint.popup.doClick(1);
			}
			bool flag2 = waypoint != null && TileMap.mapID != 16;
			if (flag2)
			{
				int posWaypointX = XmapData.GetPosWaypointX(waypoint);
				int posWaypointY = XmapData.GetPosWaypointY(waypoint);
				XmapController.MoveMyChar(posWaypointX, posWaypointY);
				XmapController.RequestChangeMap(waypoint);
			}
		}

		// Token: 0x06000C9A RID: 3226 RVA: 0x000C39CC File Offset: 0x000C1BCC
		public static void findCalich()
		{
			bool flag = TileMap.mapID == 27;
			if (flag)
			{
				XmapController.NextMap(28);
				XmapController.IsWaitNextMap = true;
				XmapController.step = 0;
			}
			else
			{
				bool flag2 = TileMap.mapID == 29;
				if (flag2)
				{
					XmapController.NextMap(28);
					XmapController.IsWaitNextMap = true;
					XmapController.step = 1;
				}
				else
				{
					bool flag3 = XmapController.step == 0;
					if (flag3)
					{
						XmapController.NextMap(29);
						XmapController.IsWaitNextMap = true;
					}
					else
					{
						bool flag4 = XmapController.step == 1;
						if (flag4)
						{
							XmapController.NextMap(27);
							XmapController.IsWaitNextMap = true;
						}
					}
				}
			}
		}

		// Token: 0x06000C9B RID: 3227 RVA: 0x000C3A60 File Offset: 0x000C1C60
		private static void NextMapNpcMenu(MapNext mapNext)
		{
			int num = mapNext.Info[0];
			bool flag = GameScr.findNPCInMap((short)num) == null;
			if (flag)
			{
				XmapController.findCalich();
			}
			else
			{
				Service.gI().openMenu(num);
				for (int i = 1; i < mapNext.Info.Length; i++)
				{
					int num2 = mapNext.Info[i];
					Service.gI().confirmMenu((short)num, (sbyte)num2);
				}
			}
		}

		// Token: 0x06000C9C RID: 3228 RVA: 0x000C3AD0 File Offset: 0x000C1CD0
		private static void NextMapNpcPanel(MapNext mapNext)
		{
			int num = mapNext.Info[0];
			int num2 = mapNext.Info[1];
			int num3 = mapNext.Info[2];
			Service.gI().openMenu(num);
			Service.gI().confirmMenu((short)num, (sbyte)num2);
			Service.gI().requestMapSelect(num3);
		}

		// Token: 0x06000C9D RID: 3229 RVA: 0x000C3B20 File Offset: 0x000C1D20
		private static void NextMapPosition(MapNext mapNext)
		{
			int num = mapNext.Info[0];
			int num2 = mapNext.Info[1];
			XmapController.MoveMyChar(num, num2);
			Service.gI().requestChangeMap();
			Service.gI().getMapOffline();
		}

		// Token: 0x06000C9E RID: 3230 RVA: 0x000C3B60 File Offset: 0x000C1D60
		private static void NextMapCapsule(MapNext mapNext)
		{
			XmapController.SaveIdMapCapsuleReturn();
			int num = mapNext.Info[0];
			Service.gI().requestMapSelect(num);
		}

		// Token: 0x06000C9F RID: 3231 RVA: 0x00006E03 File Offset: 0x00005003
		public static void UseCapsuleNormal()
		{
			Pk9rXmap.IsShowPanelMapTrans = false;
			Service.gI().useItem(0, 1, -1, 193);
		}

		// Token: 0x06000CA0 RID: 3232 RVA: 0x00006E1F File Offset: 0x0000501F
		public static void UseCapsuleVip()
		{
			Pk9rXmap.IsShowPanelMapTrans = false;
			Service.gI().useItem(0, 1, -1, 194);
		}

		// Token: 0x06000CA1 RID: 3233 RVA: 0x00006E3B File Offset: 0x0000503B
		public static void HideInfoDlg()
		{
			InfoDlg.hide();
		}

		// Token: 0x06000CA2 RID: 3234 RVA: 0x000C3B8C File Offset: 0x000C1D8C
		private static void MoveMyChar(int x, int y)
		{
			global::Char.myCharz().cx = x;
			global::Char.myCharz().cy = y;
			Service.gI().charMove();
			bool flag = !ItemTime.isExistItem(4387);
			if (flag)
			{
				global::Char.myCharz().cx = x;
				global::Char.myCharz().cy = y + 1;
				Service.gI().charMove();
				global::Char.myCharz().cx = x;
				global::Char.myCharz().cy = y;
				Service.gI().charMove();
			}
		}

		// Token: 0x06000CA3 RID: 3235 RVA: 0x000C3C14 File Offset: 0x000C1E14
		private static void RequestChangeMap(Waypoint waypoint)
		{
			bool isOffline = waypoint.isOffline;
			if (isOffline)
			{
				Service.gI().getMapOffline();
			}
			else
			{
				Service.gI().requestChangeMap();
			}
		}

		// Token: 0x040015BD RID: 5565
		private static int step;

		// Token: 0x040015BE RID: 5566
		private const int TIME_DELAY_NEXTMAP = 200;

		// Token: 0x040015BF RID: 5567
		private const int TIME_DELAY_RENEXTMAP = 500;

		// Token: 0x040015C0 RID: 5568
		private const int ID_ITEM_CAPSULE_VIP = 194;

		// Token: 0x040015C1 RID: 5569
		private const int ID_ITEM_CAPSULE = 193;

		// Token: 0x040015C2 RID: 5570
		private const int ID_ICON_ITEM_TDLT = 4387;

		// Token: 0x040015C3 RID: 5571
		private static readonly XmapController _Instance = new XmapController();

		// Token: 0x040015C4 RID: 5572
		private static int IdMapEnd;

		// Token: 0x040015C5 RID: 5573
		private static List<int> WayXmap;

		// Token: 0x040015C6 RID: 5574
		private static int IndexWay;

		// Token: 0x040015C7 RID: 5575
		private static bool IsNextMapFailed;

		// Token: 0x040015C8 RID: 5576
		private static bool IsWait;

		// Token: 0x040015C9 RID: 5577
		private static long TimeStartWait;

		// Token: 0x040015CA RID: 5578
		private static long TimeWait;

		// Token: 0x040015CB RID: 5579
		private static bool IsWaitNextMap;
	}
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AssemblyCSharp.Mod.Xmap
{
	// Token: 0x02000104 RID: 260
	public class XmapData
	{
		// Token: 0x06000CA6 RID: 3238 RVA: 0x00006E51 File Offset: 0x00005051
		private XmapData()
		{
			this.GroupMaps = new List<GroupMap>();
			this.MyLinkMaps = null;
			this.IsLoading = false;
			this.IsLoadingCapsule = false;
		}

		// Token: 0x06000CA7 RID: 3239 RVA: 0x000C3C48 File Offset: 0x000C1E48
		public static XmapData Instance()
		{
			bool flag = XmapData._Instance == null;
			if (flag)
			{
				XmapData._Instance = new XmapData();
			}
			return XmapData._Instance;
		}

		// Token: 0x06000CA8 RID: 3240 RVA: 0x00006E7B File Offset: 0x0000507B
		public void LoadLinkMaps()
		{
			this.IsLoading = true;
		}

		// Token: 0x06000CA9 RID: 3241 RVA: 0x000C3C78 File Offset: 0x000C1E78
		public void Update()
		{
			bool isLoadingCapsule = this.IsLoadingCapsule;
			if (isLoadingCapsule)
			{
				bool flag = !this.IsWaitInfoMapTrans();
				if (flag)
				{
					this.LoadLinkMapCapsule();
					this.IsLoadingCapsule = false;
					this.IsLoading = false;
				}
			}
			else
			{
				this.LoadLinkMapBase();
				bool flag2 = XmapData.CanUseCapsuleVip();
				if (flag2)
				{
					XmapController.UseCapsuleVip();
					this.IsLoadingCapsule = true;
				}
				else
				{
					bool flag3 = XmapData.CanUseCapsuleNormal();
					if (flag3)
					{
						XmapController.UseCapsuleNormal();
						this.IsLoadingCapsule = true;
					}
					else
					{
						this.IsLoading = false;
					}
				}
			}
		}

		// Token: 0x06000CAA RID: 3242 RVA: 0x000C3CFC File Offset: 0x000C1EFC
		public void LoadGroupMapsFromFile(string path)
		{
			this.GroupMaps.Clear();
			try
			{
				StreamReader streamReader = new StreamReader(path);
				string text;
				while ((text = streamReader.ReadLine()) != null)
				{
					text = text.Trim();
					bool flag = !text.StartsWith("#") && !text.Equals("");
					if (flag)
					{
						List<int> list = Array.ConvertAll<string, int>(streamReader.ReadLine().Trim().Split(new char[] { ' ' }), (string s) => int.Parse(s)).ToList<int>();
						this.GroupMaps.Add(new GroupMap(text, list));
					}
				}
			}
			catch (Exception ex)
			{
				GameScr.info1.addInfo(ex.Message, 0);
			}
			this.RemoveMapsHomeInGroupMaps();
		}

		// Token: 0x06000CAB RID: 3243 RVA: 0x000C3DF0 File Offset: 0x000C1FF0
		private void RemoveMapsHomeInGroupMaps()
		{
			int cgender = global::Char.myCharz().cgender;
			foreach (GroupMap groupMap in this.GroupMaps)
			{
				int num = cgender;
				int num2 = num;
				if (num2 != 0)
				{
					if (num2 != 1)
					{
						groupMap.IdMaps.Remove(21);
						groupMap.IdMaps.Remove(22);
					}
					else
					{
						groupMap.IdMaps.Remove(21);
						groupMap.IdMaps.Remove(23);
					}
				}
				else
				{
					groupMap.IdMaps.Remove(22);
					groupMap.IdMaps.Remove(23);
				}
			}
		}

		// Token: 0x06000CAC RID: 3244 RVA: 0x000C3EB8 File Offset: 0x000C20B8
		private void LoadLinkMapCapsule()
		{
			this.AddKeyLinkMaps(TileMap.mapID);
			string[] mapNames = GameCanvas.panel.mapNames;
			for (int i = 0; i < mapNames.Length; i++)
			{
				int idMapFromName = XmapData.GetIdMapFromName(mapNames[i]);
				bool flag = idMapFromName != -1;
				if (flag)
				{
					int[] array = new int[] { i };
					this.MyLinkMaps[TileMap.mapID].Add(new MapNext(idMapFromName, TypeMapNext.Capsule, array));
				}
			}
		}

		// Token: 0x06000CAD RID: 3245 RVA: 0x00006E85 File Offset: 0x00005085
		private void LoadLinkMapBase()
		{
			this.MyLinkMaps = new Dictionary<int, List<MapNext>>();
			this.LoadLinkMapsFromFile("Nro_244_Data\\Resources\\TextData\\LinkMapsXmap.txt");
			this.LoadLinkMapsAutoWaypointFromFile("Nro_244_Data\\Resources\\TextData\\AutoLinkMapsWaypoint.txt");
			this.LoadLinkMapsHome();
			this.LoadLinkMapSieuThi();
			this.LoadLinkMapToCold();
		}

		// Token: 0x06000CAE RID: 3246 RVA: 0x000C3F34 File Offset: 0x000C2134
		private void LoadLinkMapsFromFile(string path)
		{
			try
			{
				StreamReader streamReader = new StreamReader(path);
				string text;
				while ((text = streamReader.ReadLine()) != null)
				{
					text = text.Trim();
					bool flag = !text.StartsWith("#") && !text.Equals("");
					if (flag)
					{
						int[] array = Array.ConvertAll<string, int>(text.Split(new char[] { ' ' }), (string s) => int.Parse(s));
						int num = array.Length - 3;
						int[] array2 = new int[num];
						Array.Copy(array, 3, array2, 0, num);
						this.LoadLinkMap(array[0], array[1], (TypeMapNext)array[2], array2);
					}
				}
			}
			catch (Exception ex)
			{
				GameScr.info1.addInfo(ex.Message, 0);
			}
		}

		// Token: 0x06000CAF RID: 3247 RVA: 0x000C4024 File Offset: 0x000C2224
		private void LoadLinkMapsAutoWaypointFromFile(string path)
		{
			try
			{
				StreamReader streamReader = new StreamReader(path);
				string text;
				while ((text = streamReader.ReadLine()) != null)
				{
					text = text.Trim();
					bool flag = text.StartsWith("#") || text.Equals("");
					if (!flag)
					{
						int[] array = Array.ConvertAll<string, int>(text.Split(new char[] { ' ' }), (string s) => int.Parse(s));
						for (int i = 0; i < array.Length; i++)
						{
							bool flag2 = i != 0;
							if (flag2)
							{
								this.LoadLinkMap(array[i], array[i - 1], TypeMapNext.AutoWaypoint, null);
							}
							bool flag3 = i != array.Length - 1;
							if (flag3)
							{
								this.LoadLinkMap(array[i], array[i + 1], TypeMapNext.AutoWaypoint, null);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				GameScr.info1.addInfo(ex.Message, 0);
			}
		}

		// Token: 0x06000CB0 RID: 3248 RVA: 0x000C4144 File Offset: 0x000C2344
		private void LoadLinkMapsHome()
		{
			int cgender = global::Char.myCharz().cgender;
			int num = 21 + cgender;
			int num2 = 7 * cgender;
			this.LoadLinkMap(num2, num, TypeMapNext.AutoWaypoint, null);
			this.LoadLinkMap(num, num2, TypeMapNext.AutoWaypoint, null);
		}

		// Token: 0x06000CB1 RID: 3249 RVA: 0x000C417C File Offset: 0x000C237C
		private void LoadLinkMapSieuThi()
		{
			int cgender = global::Char.myCharz().cgender;
			int num = 24 + cgender;
			int[] array = new int[2];
			array[0] = 10;
			int[] array2 = array;
			this.LoadLinkMap(84, num, TypeMapNext.NpcMenu, array2);
		}

		// Token: 0x06000CB2 RID: 3250 RVA: 0x000C41B4 File Offset: 0x000C23B4
		private void LoadLinkMapToCold()
		{
			bool flag = global::Char.myCharz().taskMaint.taskId > 30;
			if (flag)
			{
				int[] array = new int[2];
				array[0] = 12;
				int[] array2 = array;
				this.LoadLinkMap(19, 109, TypeMapNext.NpcMenu, array2);
			}
		}

		// Token: 0x06000CB3 RID: 3251 RVA: 0x000C41F4 File Offset: 0x000C23F4
		public List<MapNext> GetMapNexts(int idMap)
		{
			bool flag = this.CanGetMapNexts(idMap);
			List<MapNext> list;
			if (flag)
			{
				list = this.MyLinkMaps[idMap];
			}
			else
			{
				list = null;
			}
			return list;
		}

		// Token: 0x06000CB4 RID: 3252 RVA: 0x000C4224 File Offset: 0x000C2424
		public bool CanGetMapNexts(int idMap)
		{
			return this.MyLinkMaps.ContainsKey(idMap);
		}

		// Token: 0x06000CB5 RID: 3253 RVA: 0x000C4244 File Offset: 0x000C2444
		private void LoadLinkMap(int idMapStart, int idMapNext, TypeMapNext type, int[] info)
		{
			this.AddKeyLinkMaps(idMapStart);
			MapNext mapNext = new MapNext(idMapNext, type, info);
			this.MyLinkMaps[idMapStart].Add(mapNext);
		}

		// Token: 0x06000CB6 RID: 3254 RVA: 0x000C4278 File Offset: 0x000C2478
		private void AddKeyLinkMaps(int idMap)
		{
			bool flag = !this.MyLinkMaps.ContainsKey(idMap);
			if (flag)
			{
				this.MyLinkMaps.Add(idMap, new List<MapNext>());
			}
		}

		// Token: 0x06000CB7 RID: 3255 RVA: 0x000C42B0 File Offset: 0x000C24B0
		private bool IsWaitInfoMapTrans()
		{
			return !Pk9rXmap.IsShowPanelMapTrans;
		}

		// Token: 0x06000CB8 RID: 3256 RVA: 0x000C42CC File Offset: 0x000C24CC
		public static int GetIdMapFromPanelXmap(string mapName)
		{
			return int.Parse(mapName.Split(new char[] { ':' })[0]);
		}

		// Token: 0x06000CB9 RID: 3257 RVA: 0x000C42F8 File Offset: 0x000C24F8
		public static Waypoint FindWaypoint(int idMap)
		{
			for (int i = 0; i < TileMap.vGo.size(); i++)
			{
				Waypoint waypoint = (Waypoint)TileMap.vGo.elementAt(i);
				bool flag = XmapData.GetTextPopup(waypoint.popup).Equals(TileMap.mapNames[idMap]);
				if (flag)
				{
					return waypoint;
				}
			}
			return null;
		}

		// Token: 0x06000CBA RID: 3258 RVA: 0x000C435C File Offset: 0x000C255C
		public static int GetPosWaypointX(Waypoint waypoint)
		{
			bool flag = waypoint.maxX < 60;
			int num;
			if (flag)
			{
				num = 15;
			}
			else
			{
				bool flag2 = (int)waypoint.minX > TileMap.pxw - 60;
				if (flag2)
				{
					num = TileMap.pxw - 15;
				}
				else
				{
					num = (int)(waypoint.minX + 30);
				}
			}
			return num;
		}

		// Token: 0x06000CBB RID: 3259 RVA: 0x000C43AC File Offset: 0x000C25AC
		public static int GetPosWaypointY(Waypoint waypoint)
		{
			return (int)waypoint.maxY;
		}

		// Token: 0x06000CBC RID: 3260 RVA: 0x000C43C4 File Offset: 0x000C25C4
		public static bool IsMyCharDie()
		{
			bool flag = global::Char.myCharz().statusMe != 14;
			return !flag || global::Char.myCharz().cHP <= 0L;
		}

		// Token: 0x06000CBD RID: 3261 RVA: 0x000C4404 File Offset: 0x000C2604
		public static bool CanNextMap()
		{
			bool flag = !global::Char.isLoadingMap && !global::Char.ischangingMap;
			return flag && !Controller.isStopReadMessage;
		}

		// Token: 0x06000CBE RID: 3262 RVA: 0x000C443C File Offset: 0x000C263C
		private static int GetIdMapFromName(string mapName)
		{
			int cgender = global::Char.myCharz().cgender;
			bool flag = mapName.Equals("Về nhà");
			int num;
			if (flag)
			{
				num = 21 + cgender;
			}
			else
			{
				bool flag2 = mapName.Equals("Trạm tàu vũ trụ");
				if (flag2)
				{
					num = 24 + cgender;
				}
				else
				{
					bool flag3 = mapName.Contains("Về chỗ cũ: ");
					if (flag3)
					{
						mapName = mapName.Replace("Về chỗ cũ: ", "");
						bool flag4 = TileMap.mapNames[Pk9rXmap.IdMapCapsuleReturn].Equals(mapName);
						if (flag4)
						{
							return Pk9rXmap.IdMapCapsuleReturn;
						}
						bool flag5 = mapName.Equals("Rừng đá");
						if (flag5)
						{
							return -1;
						}
					}
					for (int i = 0; i < TileMap.mapNames.Length; i++)
					{
						bool flag6 = mapName.Equals(TileMap.mapNames[i]);
						if (flag6)
						{
							return i;
						}
					}
					num = -1;
				}
			}
			return num;
		}

		// Token: 0x06000CBF RID: 3263 RVA: 0x000C4524 File Offset: 0x000C2724
		private static string GetTextPopup(PopUp popUp)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < popUp.says.Length; i++)
			{
				stringBuilder.Append(popUp.says[i]);
				stringBuilder.Append(" ");
			}
			return stringBuilder.ToString().Trim();
		}

		// Token: 0x06000CC0 RID: 3264 RVA: 0x000C457C File Offset: 0x000C277C
		private static bool CanUseCapsuleNormal()
		{
			bool flag = !XmapData.IsMyCharDie() && Pk9rXmap.IsUseCapsuleNormal;
			return flag && XmapData.HasItemCapsuleNormal();
		}

		// Token: 0x06000CC1 RID: 3265 RVA: 0x000C45AC File Offset: 0x000C27AC
		private static bool HasItemCapsuleNormal()
		{
			Item[] arrItemBag = global::Char.myCharz().arrItemBag;
			for (int i = 0; i < arrItemBag.Length; i++)
			{
				bool flag = arrItemBag[i] != null && arrItemBag[i].template.id == 193;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000CC2 RID: 3266 RVA: 0x000C4608 File Offset: 0x000C2808
		private static bool CanUseCapsuleVip()
		{
			bool flag = !XmapData.IsMyCharDie() && Pk9rXmap.IsUseCapsuleVip;
			return flag && XmapData.HasItemCapsuleVip();
		}

		// Token: 0x06000CC3 RID: 3267 RVA: 0x000C4638 File Offset: 0x000C2838
		private static bool HasItemCapsuleVip()
		{
			Item[] arrItemBag = global::Char.myCharz().arrItemBag;
			for (int i = 0; i < arrItemBag.Length; i++)
			{
				bool flag = arrItemBag[i] != null && arrItemBag[i].template.id == 194;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x040015CC RID: 5580
		private const int ID_MAP_HOME_BASE = 21;

		// Token: 0x040015CD RID: 5581
		private const int ID_MAP_TTVT_BASE = 24;

		// Token: 0x040015CE RID: 5582
		private const int ID_ITEM_CAPSUAL_VIP = 194;

		// Token: 0x040015CF RID: 5583
		private const int ID_ITEM_CAPSUAL_NORMAL = 193;

		// Token: 0x040015D0 RID: 5584
		private const int ID_MAP_TPVGT = 19;

		// Token: 0x040015D1 RID: 5585
		private const int ID_MAP_TO_COLD = 109;

		// Token: 0x040015D2 RID: 5586
		public List<GroupMap> GroupMaps;

		// Token: 0x040015D3 RID: 5587
		public Dictionary<int, List<MapNext>> MyLinkMaps;

		// Token: 0x040015D4 RID: 5588
		public bool IsLoading;

		// Token: 0x040015D5 RID: 5589
		private bool IsLoadingCapsule;

		// Token: 0x040015D6 RID: 5590
		private static XmapData _Instance;
	}
}
using System;

namespace Assets.src.e
{
	// Token: 0x020000FC RID: 252
	public class Small
	{
		// Token: 0x06000C71 RID: 3185 RVA: 0x00006D18 File Offset: 0x00004F18
		public Small(Image img, int id)
		{
			this.img = img;
			this.id = id;
			this.timePaint = 0;
			this.timeUpdate = 0;
		}

		// Token: 0x06000C72 RID: 3186 RVA: 0x000C2D4C File Offset: 0x000C0F4C
		public void paint(mGraphics g, int transform, int x, int y, int anchor)
		{
			g.drawRegion(this.img, 0, 0, mGraphics.getImageWidth(this.img), mGraphics.getImageHeight(this.img), transform, x, y, anchor);
			bool flag = GameCanvas.gameTick % 1000 == 0;
			if (flag)
			{
				this.timePaint++;
				this.timeUpdate = this.timePaint;
			}
		}

		// Token: 0x06000C73 RID: 3187 RVA: 0x000C2DB4 File Offset: 0x000C0FB4
		public void paint(mGraphics g, int transform, int f, int x, int y, int w, int h, int anchor)
		{
			this.paint(g, transform, f, x, y, w, h, anchor, false);
		}

		// Token: 0x06000C74 RID: 3188 RVA: 0x000C2DD8 File Offset: 0x000C0FD8
		public void paint(mGraphics g, int transform, int f, int x, int y, int w, int h, int anchor, bool isClip)
		{
			bool flag = mGraphics.getImageWidth(this.img) != 1;
			if (flag)
			{
				g.drawRegion(this.img, 0, f * w, w, h, transform, x, y, anchor, isClip);
				bool flag2 = GameCanvas.gameTick % 1000 == 0;
				if (flag2)
				{
					this.timePaint++;
					this.timeUpdate = this.timePaint;
				}
			}
		}

		// Token: 0x06000C75 RID: 3189 RVA: 0x000C2E4C File Offset: 0x000C104C
		public void update()
		{
			this.timeUpdate++;
			bool flag = this.timeUpdate - this.timePaint > 1 && !global::Char.myCharz().isCharBodyImageID(this.id);
			if (flag)
			{
				SmallImage.imgNew[this.id] = null;
			}
		}

		// Token: 0x040015A6 RID: 5542
		public Image img;

		// Token: 0x040015A7 RID: 5543
		public int id;

		// Token: 0x040015A8 RID: 5544
		public int timePaint;

		// Token: 0x040015A9 RID: 5545
		public int timeUpdate;
	}
}
using System;
using Assets.src.g;

namespace Assets.src.f
{
	// Token: 0x020000FA RID: 250
	internal class Controller2
	{
		// Token: 0x06000C6A RID: 3178 RVA: 0x000BFDD8 File Offset: 0x000BDFD8
		public static void readMessage(Message msg)
		{
			try
			{
				sbyte command = msg.command;
				sbyte b = command;
				if (b <= 42)
				{
					switch (b)
					{
					case -128:
						Controller2.readInfoEffChar(msg);
						break;
					case -127:
						Controller2.readLuckyRound(msg);
						break;
					case -126:
					{
						sbyte b2 = msg.reader().readByte();
						Res.outz("type quay= " + b2.ToString());
						bool flag = b2 == 1;
						if (flag)
						{
							sbyte b3 = msg.reader().readByte();
							string text = msg.reader().readUTF();
							string text2 = msg.reader().readUTF();
							GameScr.gI().showWinNumber(text, text2);
						}
						bool flag2 = b2 == 0;
						if (flag2)
						{
							GameScr.gI().showYourNumber(msg.reader().readUTF());
						}
						break;
					}
					case -125:
					{
						ChatTextField.gI().isShow = false;
						string text3 = msg.reader().readUTF();
						Res.outz("titile= " + text3);
						sbyte b4 = msg.reader().readByte();
						ClientInput.gI().setInput((int)b4, text3);
						for (int i = 0; i < (int)b4; i++)
						{
							ClientInput.gI().tf[i].name = msg.reader().readUTF();
							sbyte b5 = msg.reader().readByte();
							bool flag3 = b5 == 0;
							if (flag3)
							{
								ClientInput.gI().tf[i].setIputType(TField.INPUT_TYPE_NUMERIC);
							}
							bool flag4 = b5 == 1;
							if (flag4)
							{
								ClientInput.gI().tf[i].setIputType(TField.INPUT_TYPE_ANY);
							}
							bool flag5 = b5 == 2;
							if (flag5)
							{
								ClientInput.gI().tf[i].setIputType(TField.INPUT_TYPE_PASSWORD);
							}
						}
						break;
					}
					case -124:
					{
						sbyte b6 = msg.reader().readByte();
						sbyte b7 = msg.reader().readByte();
						bool flag6 = b7 == 0;
						if (flag6)
						{
							bool flag7 = b6 == 2;
							if (flag7)
							{
								int num = msg.reader().readInt();
								bool flag8 = num == global::Char.myCharz().charID;
								if (flag8)
								{
									global::Char.myCharz().removeEffect();
								}
								else
								{
									bool flag9 = GameScr.findCharInMap(num) != null;
									if (flag9)
									{
										GameScr.findCharInMap(num).removeEffect();
									}
								}
							}
							int num2 = (int)msg.reader().readUnsignedByte();
							int num3 = msg.reader().readInt();
							bool flag10 = num2 == 32;
							if (flag10)
							{
								bool flag11 = b6 == 1;
								if (flag11)
								{
									int num4 = msg.reader().readInt();
									bool flag12 = num3 == global::Char.myCharz().charID;
									if (flag12)
									{
										global::Char.myCharz().holdEffID = num2;
										GameScr.findCharInMap(num4).setHoldChar(global::Char.myCharz());
									}
									else
									{
										bool flag13 = GameScr.findCharInMap(num3) != null && num4 != global::Char.myCharz().charID;
										if (flag13)
										{
											GameScr.findCharInMap(num3).holdEffID = num2;
											GameScr.findCharInMap(num4).setHoldChar(GameScr.findCharInMap(num3));
										}
										else
										{
											bool flag14 = GameScr.findCharInMap(num3) != null && num4 == global::Char.myCharz().charID;
											if (flag14)
											{
												GameScr.findCharInMap(num3).holdEffID = num2;
												global::Char.myCharz().setHoldChar(GameScr.findCharInMap(num3));
											}
										}
									}
								}
								else
								{
									bool flag15 = num3 == global::Char.myCharz().charID;
									if (flag15)
									{
										global::Char.myCharz().removeHoleEff();
									}
									else
									{
										bool flag16 = GameScr.findCharInMap(num3) != null;
										if (flag16)
										{
											GameScr.findCharInMap(num3).removeHoleEff();
										}
									}
								}
							}
							bool flag17 = num2 == 33;
							if (flag17)
							{
								bool flag18 = b6 == 1;
								if (flag18)
								{
									bool flag19 = num3 == global::Char.myCharz().charID;
									if (flag19)
									{
										global::Char.myCharz().protectEff = true;
									}
									else
									{
										bool flag20 = GameScr.findCharInMap(num3) != null;
										if (flag20)
										{
											GameScr.findCharInMap(num3).protectEff = true;
										}
									}
								}
								else
								{
									bool flag21 = num3 == global::Char.myCharz().charID;
									if (flag21)
									{
										global::Char.myCharz().removeProtectEff();
									}
									else
									{
										bool flag22 = GameScr.findCharInMap(num3) != null;
										if (flag22)
										{
											GameScr.findCharInMap(num3).removeProtectEff();
										}
									}
								}
							}
							bool flag23 = num2 == 39;
							if (flag23)
							{
								bool flag24 = b6 == 1;
								if (flag24)
								{
									bool flag25 = num3 == global::Char.myCharz().charID;
									if (flag25)
									{
										global::Char.myCharz().huytSao = true;
									}
									else
									{
										bool flag26 = GameScr.findCharInMap(num3) != null;
										if (flag26)
										{
											GameScr.findCharInMap(num3).huytSao = true;
										}
									}
								}
								else
								{
									bool flag27 = num3 == global::Char.myCharz().charID;
									if (flag27)
									{
										global::Char.myCharz().removeHuytSao();
									}
									else
									{
										bool flag28 = GameScr.findCharInMap(num3) != null;
										if (flag28)
										{
											GameScr.findCharInMap(num3).removeHuytSao();
										}
									}
								}
							}
							bool flag29 = num2 == 40;
							if (flag29)
							{
								bool flag30 = b6 == 1;
								if (flag30)
								{
									bool flag31 = num3 == global::Char.myCharz().charID;
									if (flag31)
									{
										global::Char.myCharz().blindEff = true;
									}
									else
									{
										bool flag32 = GameScr.findCharInMap(num3) != null;
										if (flag32)
										{
											GameScr.findCharInMap(num3).blindEff = true;
										}
									}
								}
								else
								{
									bool flag33 = num3 == global::Char.myCharz().charID;
									if (flag33)
									{
										global::Char.myCharz().removeBlindEff();
									}
									else
									{
										bool flag34 = GameScr.findCharInMap(num3) != null;
										if (flag34)
										{
											GameScr.findCharInMap(num3).removeBlindEff();
										}
									}
								}
							}
							bool flag35 = num2 == 41;
							if (flag35)
							{
								bool flag36 = b6 == 1;
								if (flag36)
								{
									bool flag37 = num3 == global::Char.myCharz().charID;
									if (flag37)
									{
										global::Char.myCharz().sleepEff = true;
									}
									else
									{
										bool flag38 = GameScr.findCharInMap(num3) != null;
										if (flag38)
										{
											GameScr.findCharInMap(num3).sleepEff = true;
										}
									}
								}
								else
								{
									bool flag39 = num3 == global::Char.myCharz().charID;
									if (flag39)
									{
										global::Char.myCharz().removeSleepEff();
									}
									else
									{
										bool flag40 = GameScr.findCharInMap(num3) != null;
										if (flag40)
										{
											GameScr.findCharInMap(num3).removeSleepEff();
										}
									}
								}
							}
							bool flag41 = num2 == 42;
							if (flag41)
							{
								bool flag42 = b6 == 1;
								if (flag42)
								{
									bool flag43 = num3 == global::Char.myCharz().charID;
									if (flag43)
									{
										global::Char.myCharz().stone = true;
									}
								}
								else
								{
									bool flag44 = num3 == global::Char.myCharz().charID;
									if (flag44)
									{
										global::Char.myCharz().stone = false;
									}
								}
							}
						}
						bool flag45 = b7 != 1;
						if (!flag45)
						{
							int num5 = (int)msg.reader().readUnsignedByte();
							sbyte b8 = msg.reader().readByte();
							Res.outz(string.Concat(new string[]
							{
								"modbHoldID= ",
								b8.ToString(),
								" skillID= ",
								num5.ToString(),
								"eff ID= ",
								b6.ToString()
							}));
							bool flag46 = num5 == 32;
							if (flag46)
							{
								bool flag47 = b6 == 1;
								if (flag47)
								{
									int num6 = msg.reader().readInt();
									bool flag48 = num6 == global::Char.myCharz().charID;
									if (flag48)
									{
										GameScr.findMobInMap(b8).holdEffID = num5;
										global::Char.myCharz().setHoldMob(GameScr.findMobInMap(b8));
									}
									else
									{
										bool flag49 = GameScr.findCharInMap(num6) != null;
										if (flag49)
										{
											GameScr.findMobInMap(b8).holdEffID = num5;
											GameScr.findCharInMap(num6).setHoldMob(GameScr.findMobInMap(b8));
										}
									}
								}
								else
								{
									GameScr.findMobInMap(b8).removeHoldEff();
								}
							}
							bool flag50 = num5 == 40;
							if (flag50)
							{
								bool flag51 = b6 == 1;
								if (flag51)
								{
									GameScr.findMobInMap(b8).blindEff = true;
								}
								else
								{
									GameScr.findMobInMap(b8).removeBlindEff();
								}
							}
							bool flag52 = num5 == 41;
							if (flag52)
							{
								bool flag53 = b6 == 1;
								if (flag53)
								{
									GameScr.findMobInMap(b8).sleepEff = true;
								}
								else
								{
									GameScr.findMobInMap(b8).removeSleepEff();
								}
							}
						}
						break;
					}
					case -123:
					{
						int num7 = msg.reader().readInt();
						bool flag54 = GameScr.findCharInMap(num7) != null;
						if (flag54)
						{
							GameScr.findCharInMap(num7).perCentMp = (int)msg.reader().readByte();
						}
						break;
					}
					case -122:
					{
						short num8 = msg.reader().readShort();
						Npc npc = GameScr.findNPCInMap(num8);
						sbyte b9 = msg.reader().readByte();
						npc.duahau = new int[(int)b9];
						Res.outz("N DUA HAU= " + b9.ToString());
						for (int j = 0; j < (int)b9; j++)
						{
							npc.duahau[j] = (int)msg.reader().readShort();
						}
						npc.setStatus(msg.reader().readByte(), msg.reader().readInt());
						break;
					}
					case -121:
					{
						long num9 = mSystem.currentTimeMillis();
						Service.logMap = num9 - Service.curCheckMap;
						Service.gI().sendCheckMap();
						break;
					}
					case -120:
					{
						long num10 = mSystem.currentTimeMillis();
						Service.logController = num10 - Service.curCheckController;
						Service.gI().sendCheckController();
						break;
					}
					case -119:
						global::Char.myCharz().rank = msg.reader().readInt();
						break;
					case -118:
					case -114:
					case -112:
					case -109:
					case -108:
					case -107:
					case -104:
					case -99:
					case -98:
					case -97:
					case -96:
					case -95:
					case -94:
					case -93:
					case -92:
					case -91:
					case -90:
						break;
					case -117:
					{
						GameScr.gI().tMabuEff = 0;
						GameScr.gI().percentMabu = msg.reader().readByte();
						bool flag55 = GameScr.gI().percentMabu == 100;
						if (flag55)
						{
							GameScr.gI().mabuEff = true;
						}
						bool flag56 = GameScr.gI().percentMabu == 101;
						if (flag56)
						{
							Npc.mabuEff = true;
						}
						break;
					}
					case -116:
						GameScr.canAutoPlay = msg.reader().readByte() == 1;
						break;
					case -115:
						global::Char.myCharz().setPowerInfo(msg.reader().readUTF(), msg.reader().readShort(), msg.reader().readShort(), msg.reader().readShort());
						break;
					case -113:
					{
						sbyte[] array = new sbyte[10];
						for (int k = 0; k < 10; k++)
						{
							array[k] = msg.reader().readByte();
							Res.outz("vlue i= " + array[k].ToString());
						}
						GameScr.gI().onKSkill(array);
						GameScr.gI().onOSkill(array);
						GameScr.gI().onCSkill(array);
						break;
					}
					case -111:
					{
						short num11 = msg.reader().readShort();
						ImageSource.vSource = new MyVector();
						for (int l = 0; l < (int)num11; l++)
						{
							string text4 = msg.reader().readUTF();
							sbyte b10 = msg.reader().readByte();
							ImageSource.vSource.addElement(new ImageSource(text4, b10));
						}
						ImageSource.checkRMS();
						ImageSource.saveRMS();
						break;
					}
					case -110:
					{
						sbyte b11 = msg.reader().readByte();
						bool flag57 = b11 == 1;
						if (flag57)
						{
							int num12 = msg.reader().readInt();
							sbyte[] array2 = Rms.loadRMS(num12.ToString() + string.Empty);
							bool flag58 = array2 == null;
							if (flag58)
							{
								Service.gI().sendServerData(1, -1, null);
							}
							else
							{
								Service.gI().sendServerData(1, num12, array2);
							}
						}
						bool flag59 = b11 == 0;
						if (flag59)
						{
							int num13 = msg.reader().readInt();
							short num14 = msg.reader().readShort();
							sbyte[] array3 = new sbyte[(int)num14];
							msg.reader().read(ref array3, 0, (int)num14);
							Rms.saveRMS(num13.ToString() + string.Empty, array3);
						}
						break;
					}
					case -106:
					{
						short num15 = msg.reader().readShort();
						int num16 = (int)msg.reader().readShort();
						bool flag60 = ItemTime.isExistItem((int)num15);
						if (flag60)
						{
							ItemTime.getItemById((int)num15).initTime(num16);
						}
						else
						{
							ItemTime itemTime = new ItemTime(num15, num16);
							global::Char.vItemTime.addElement(itemTime);
						}
						break;
					}
					case -105:
						TransportScr.gI().time = 0;
						TransportScr.gI().maxTime = msg.reader().readShort();
						TransportScr.gI().last = (TransportScr.gI().curr = mSystem.currentTimeMillis());
						TransportScr.gI().type = msg.reader().readByte();
						TransportScr.gI().switchToMe();
						break;
					case -103:
					{
						sbyte b12 = msg.reader().readByte();
						bool flag61 = b12 == 0;
						if (flag61)
						{
							GameCanvas.panel.vFlag.removeAllElements();
							sbyte b13 = msg.reader().readByte();
							for (int m = 0; m < (int)b13; m++)
							{
								Item item = new Item();
								short num17 = msg.reader().readShort();
								bool flag62 = num17 != -1;
								if (flag62)
								{
									item.template = ItemTemplates.get(num17);
									sbyte b14 = msg.reader().readByte();
									bool flag63 = b14 != -1;
									if (flag63)
									{
										item.itemOption = new ItemOption[(int)b14];
										for (int n = 0; n < item.itemOption.Length; n++)
										{
											ItemOption itemOption = Controller.gI().readItemOption(msg);
											bool flag64 = itemOption != null;
											if (flag64)
											{
												item.itemOption[n] = itemOption;
											}
										}
									}
								}
								GameCanvas.panel.vFlag.addElement(item);
							}
							GameCanvas.panel.setTypeFlag();
							GameCanvas.panel.show();
						}
						else
						{
							bool flag65 = b12 == 1;
							if (flag65)
							{
								int num18 = msg.reader().readInt();
								sbyte b15 = msg.reader().readByte();
								Res.outz("---------------actionFlag1:  " + num18.ToString() + " : " + b15.ToString());
								bool flag66 = num18 == global::Char.myCharz().charID;
								if (flag66)
								{
									global::Char.myCharz().cFlag = b15;
								}
								else
								{
									bool flag67 = GameScr.findCharInMap(num18) != null;
									if (flag67)
									{
										GameScr.findCharInMap(num18).cFlag = b15;
									}
								}
								GameScr.gI().getFlagImage(num18, b15);
							}
							else
							{
								bool flag68 = b12 != 2;
								if (!flag68)
								{
									sbyte b16 = msg.reader().readByte();
									int num19 = (int)msg.reader().readShort();
									PKFlag pkflag = new PKFlag();
									pkflag.cflag = b16;
									pkflag.IDimageFlag = num19;
									GameScr.vFlag.addElement(pkflag);
									for (int num20 = 0; num20 < GameScr.vFlag.size(); num20++)
									{
										PKFlag pkflag2 = (PKFlag)GameScr.vFlag.elementAt(num20);
										Res.outz(string.Concat(new string[]
										{
											"i: ",
											num20.ToString(),
											"  cflag: ",
											pkflag2.cflag.ToString(),
											"   IDimageFlag: ",
											pkflag2.IDimageFlag.ToString()
										}));
									}
									for (int num21 = 0; num21 < GameScr.vCharInMap.size(); num21++)
									{
										global::Char @char = (global::Char)GameScr.vCharInMap.elementAt(num21);
										bool flag69 = @char != null && @char.cFlag == b16;
										if (flag69)
										{
											@char.flagImage = num19;
										}
									}
									bool flag70 = global::Char.myCharz().cFlag == b16;
									if (flag70)
									{
										global::Char.myCharz().flagImage = num19;
									}
								}
							}
						}
						break;
					}
					case -102:
					{
						sbyte b17 = msg.reader().readByte();
						bool flag71 = b17 != 0 && b17 == 1;
						if (flag71)
						{
							GameCanvas.loginScr.isLogin2 = false;
							Service.gI().login(Rms.loadRMSString("acc"), Rms.loadRMSString("pass"), GameMidlet.VERSION, 0);
							LoginScr.isLoggingIn = true;
						}
						break;
					}
					case -101:
					{
						GameCanvas.loginScr.isLogin2 = true;
						GameCanvas.connect();
						string text5 = msg.reader().readUTF();
						Rms.saveRMSString("userAo" + ServerListScreen.ipSelect.ToString(), text5);
						Service.gI().setClientType();
						Service.gI().login(text5, string.Empty, GameMidlet.VERSION, 1);
						break;
					}
					case -100:
					{
						InfoDlg.hide();
						bool flag72 = false;
						bool flag73 = GameCanvas.w > 2 * Panel.WIDTH_PANEL;
						if (flag73)
						{
							flag72 = true;
						}
						sbyte b18 = msg.reader().readByte();
						bool flag74 = b18 < 0;
						if (!flag74)
						{
							Res.outz("t Indxe= " + b18.ToString());
							GameCanvas.panel.maxPageShop[(int)b18] = (int)msg.reader().readByte();
							GameCanvas.panel.currPageShop[(int)b18] = (int)msg.reader().readByte();
							Res.outz("max page= " + GameCanvas.panel.maxPageShop[(int)b18].ToString() + " curr page= " + GameCanvas.panel.currPageShop[(int)b18].ToString());
							int num22 = (int)msg.reader().readUnsignedByte();
							global::Char.myCharz().arrItemShop[(int)b18] = new Item[num22];
							for (int num23 = 0; num23 < num22; num23++)
							{
								short num24 = msg.reader().readShort();
								bool flag75 = num24 == -1;
								if (!flag75)
								{
									Res.outz("template id= " + num24.ToString());
									global::Char.myCharz().arrItemShop[(int)b18][num23] = new Item();
									global::Char.myCharz().arrItemShop[(int)b18][num23].template = ItemTemplates.get(num24);
									global::Char.myCharz().arrItemShop[(int)b18][num23].itemId = (int)msg.reader().readShort();
									global::Char.myCharz().arrItemShop[(int)b18][num23].buyCoin = msg.reader().readInt();
									global::Char.myCharz().arrItemShop[(int)b18][num23].buyGold = msg.reader().readInt();
									global::Char.myCharz().arrItemShop[(int)b18][num23].buyType = msg.reader().readByte();
									global::Char.myCharz().arrItemShop[(int)b18][num23].quantity = msg.reader().readInt();
									global::Char.myCharz().arrItemShop[(int)b18][num23].isMe = msg.reader().readByte();
									Panel.strWantToBuy = mResources.say_wat_do_u_want_to_buy;
									sbyte b19 = msg.reader().readByte();
									bool flag76 = b19 != -1;
									if (flag76)
									{
										global::Char.myCharz().arrItemShop[(int)b18][num23].itemOption = new ItemOption[(int)b19];
										for (int num25 = 0; num25 < global::Char.myCharz().arrItemShop[(int)b18][num23].itemOption.Length; num25++)
										{
											ItemOption itemOption2 = Controller.gI().readItemOption(msg);
											bool flag77 = itemOption2 != null;
											if (flag77)
											{
												global::Char.myCharz().arrItemShop[(int)b18][num23].itemOption[num25] = itemOption2;
												global::Char.myCharz().arrItemShop[(int)b18][num23].compare = GameCanvas.panel.getCompare(global::Char.myCharz().arrItemShop[(int)b18][num23]);
											}
										}
									}
									sbyte b20 = msg.reader().readByte();
									bool flag78 = b20 == 1;
									if (flag78)
									{
										int num26 = (int)msg.reader().readShort();
										int num27 = (int)msg.reader().readShort();
										int num28 = (int)msg.reader().readShort();
										int num29 = (int)msg.reader().readShort();
										global::Char.myCharz().arrItemShop[(int)b18][num23].setPartTemp(num26, num27, num28, num29);
									}
									bool flag79 = GameMidlet.intVERSION >= 237;
									if (flag79)
									{
										global::Char.myCharz().arrItemShop[(int)b18][num23].nameNguoiKyGui = msg.reader().readUTF();
										Res.err("nguoi ki gui  " + global::Char.myCharz().arrItemShop[(int)b18][num23].nameNguoiKyGui);
									}
								}
							}
							bool flag80 = flag72;
							if (flag80)
							{
								GameCanvas.panel2.setTabKiGui();
							}
							GameCanvas.panel.setTabShop();
							GameCanvas.panel.cmy = (GameCanvas.panel.cmtoY = 0);
						}
						break;
					}
					case -89:
						GameCanvas.open3Hour = msg.reader().readByte() == 1;
						break;
					default:
						if (b != 31)
						{
							if (b == 42)
							{
								GameCanvas.endDlg();
								LoginScr.isContinueToLogin = false;
								global::Char.isLoadingMap = false;
								sbyte b21 = msg.reader().readByte();
								bool flag81 = GameCanvas.registerScr == null;
								if (flag81)
								{
									GameCanvas.registerScr = new RegisterScreen(b21);
								}
								GameCanvas.registerScr.switchToMe();
							}
						}
						else
						{
							int num30 = msg.reader().readInt();
							sbyte b22 = msg.reader().readByte();
							bool flag82 = b22 == 1;
							if (flag82)
							{
								short num31 = msg.reader().readShort();
								sbyte b23 = -1;
								int[] array4 = null;
								short num32 = 0;
								short num33 = 0;
								try
								{
									b23 = msg.reader().readByte();
									bool flag83 = b23 > 0;
									if (flag83)
									{
										sbyte b24 = msg.reader().readByte();
										array4 = new int[(int)b24];
										for (int num34 = 0; num34 < (int)b24; num34++)
										{
											array4[num34] = (int)msg.reader().readByte();
										}
										num32 = msg.reader().readShort();
										num33 = msg.reader().readShort();
									}
								}
								catch (Exception)
								{
								}
								bool flag84 = num30 == global::Char.myCharz().charID;
								if (flag84)
								{
									global::Char.myCharz().petFollow = new PetFollow();
									global::Char.myCharz().petFollow.smallID = num31;
									bool flag85 = b23 > 0;
									if (flag85)
									{
										global::Char.myCharz().petFollow.SetImg((int)b23, array4, (int)num32, (int)num33);
									}
								}
								else
								{
									global::Char char2 = GameScr.findCharInMap(num30);
									char2.petFollow = new PetFollow();
									char2.petFollow.smallID = num31;
									bool flag86 = b23 > 0;
									if (flag86)
									{
										char2.petFollow.SetImg((int)b23, array4, (int)num32, (int)num33);
									}
								}
							}
							else
							{
								bool flag87 = num30 == global::Char.myCharz().charID;
								if (flag87)
								{
									global::Char.myCharz().petFollow.remove();
									global::Char.myCharz().petFollow = null;
								}
								else
								{
									global::Char char3 = GameScr.findCharInMap(num30);
									char3.petFollow.remove();
									char3.petFollow = null;
								}
							}
						}
						break;
					}
				}
				else if (b <= 93)
				{
					switch (b)
					{
					case 48:
					{
						sbyte b25 = msg.reader().readByte();
						ServerListScreen.SetIpSelect((int)b25, false);
						GameCanvas.instance.doResetToLoginScr(GameCanvas.serverScreen);
						Session_ME.gI().close();
						GameCanvas.endDlg();
						ServerListScreen.waitToLogin = true;
						break;
					}
					case 49:
					case 50:
						break;
					case 51:
					{
						int num35 = msg.reader().readInt();
						Mabu mabu = (Mabu)GameScr.findCharInMap(num35);
						sbyte b26 = msg.reader().readByte();
						short num36 = msg.reader().readShort();
						short num37 = msg.reader().readShort();
						sbyte b27 = msg.reader().readByte();
						global::Char[] array5 = new global::Char[(int)b27];
						long[] array6 = new long[(int)b27];
						for (int num38 = 0; num38 < (int)b27; num38++)
						{
							int num39 = msg.reader().readInt();
							Res.outz("char ID=" + num39.ToString());
							array5[num38] = null;
							bool flag88 = num39 != global::Char.myCharz().charID;
							if (flag88)
							{
								array5[num38] = GameScr.findCharInMap(num39);
							}
							else
							{
								array5[num38] = global::Char.myCharz();
							}
							array6[num38] = msg.reader().readLong();
						}
						mabu.setSkill(b26, num36, num37, array5, array6);
						break;
					}
					case 52:
					{
						sbyte b28 = msg.reader().readByte();
						bool flag89 = b28 == 1;
						if (flag89)
						{
							int num40 = msg.reader().readInt();
							bool flag90 = num40 == global::Char.myCharz().charID;
							if (flag90)
							{
								global::Char.myCharz().setMabuHold(true);
								global::Char.myCharz().cx = (int)msg.reader().readShort();
								global::Char.myCharz().cy = (int)msg.reader().readShort();
							}
							else
							{
								global::Char char4 = GameScr.findCharInMap(num40);
								bool flag91 = char4 != null;
								if (flag91)
								{
									char4.setMabuHold(true);
									char4.cx = (int)msg.reader().readShort();
									char4.cy = (int)msg.reader().readShort();
								}
							}
						}
						bool flag92 = b28 == 0;
						if (flag92)
						{
							int num41 = msg.reader().readInt();
							bool flag93 = num41 == global::Char.myCharz().charID;
							if (flag93)
							{
								global::Char.myCharz().setMabuHold(false);
							}
							else
							{
								global::Char char5 = GameScr.findCharInMap(num41);
								if (char5 != null)
								{
									char5.setMabuHold(false);
								}
							}
						}
						bool flag94 = b28 == 2;
						if (flag94)
						{
							int num42 = msg.reader().readInt();
							int num43 = msg.reader().readInt();
							Mabu mabu2 = (Mabu)GameScr.findCharInMap(num42);
							mabu2.eat(num43);
						}
						bool flag95 = b28 == 3;
						if (flag95)
						{
							GameScr.mabuPercent = msg.reader().readByte();
						}
						break;
					}
					default:
						if (b == 93)
						{
							string text6 = msg.reader().readUTF();
							text6 = Res.changeString(text6);
							GameScr.gI().chatVip(text6);
						}
						break;
					}
				}
				else
				{
					switch (b)
					{
					case 100:
					{
						sbyte b29 = msg.reader().readByte();
						sbyte b30 = msg.reader().readByte();
						Item item2 = null;
						bool flag96 = b29 == 0;
						if (flag96)
						{
							item2 = global::Char.myCharz().arrItemBody[(int)b30];
						}
						bool flag97 = b29 == 1;
						if (flag97)
						{
							item2 = global::Char.myCharz().arrItemBag[(int)b30];
						}
						short num44 = msg.reader().readShort();
						bool flag98 = num44 == -1;
						if (!flag98)
						{
							item2.template = ItemTemplates.get(num44);
							item2.quantity = msg.reader().readInt();
							item2.info = msg.reader().readUTF();
							item2.content = msg.reader().readUTF();
							sbyte b31 = msg.reader().readByte();
							bool flag99 = b31 != 0;
							if (flag99)
							{
								item2.itemOption = new ItemOption[(int)b31];
								for (int num45 = 0; num45 < item2.itemOption.Length; num45++)
								{
									ItemOption itemOption3 = Controller.gI().readItemOption(msg);
									bool flag100 = itemOption3 != null;
									if (flag100)
									{
										item2.itemOption[num45] = itemOption3;
									}
								}
							}
							bool flag101 = item2.quantity <= 0;
							if (flag101)
							{
							}
						}
						break;
					}
					case 101:
					{
						Res.outz("big boss--------------------------------------------------");
						BigBoss bigBoss = Mob.getBigBoss();
						bool flag102 = bigBoss == null;
						if (!flag102)
						{
							sbyte b32 = msg.reader().readByte();
							bool flag103 = b32 == 0 || b32 == 1 || b32 == 2 || b32 == 4 || b32 == 3;
							if (flag103)
							{
								bool flag104 = b32 == 3;
								if (flag104)
								{
									bigBoss.xTo = (bigBoss.xFirst = (int)msg.reader().readShort());
									bigBoss.yTo = (bigBoss.yFirst = (int)msg.reader().readShort());
									bigBoss.setFly();
								}
								else
								{
									sbyte b33 = msg.reader().readByte();
									Res.outz("CHUONG nChar= " + b33.ToString());
									global::Char[] array7 = new global::Char[(int)b33];
									long[] array8 = new long[(int)b33];
									for (int num46 = 0; num46 < (int)b33; num46++)
									{
										int num47 = msg.reader().readInt();
										Res.outz("char ID=" + num47.ToString());
										array7[num46] = null;
										bool flag105 = num47 != global::Char.myCharz().charID;
										if (flag105)
										{
											array7[num46] = GameScr.findCharInMap(num47);
										}
										else
										{
											array7[num46] = global::Char.myCharz();
										}
										array8[num46] = msg.reader().readLong();
									}
									bigBoss.setAttack(array7, array8, b32);
								}
							}
							bool flag106 = b32 == 5;
							if (flag106)
							{
								bigBoss.haftBody = true;
								bigBoss.status = 2;
							}
							bool flag107 = b32 == 6;
							if (flag107)
							{
								bigBoss.getDataB2();
								bigBoss.x = (int)msg.reader().readShort();
								bigBoss.y = (int)msg.reader().readShort();
							}
							bool flag108 = b32 == 7;
							if (flag108)
							{
								bigBoss.setAttack(null, null, b32);
							}
							bool flag109 = b32 == 8;
							if (flag109)
							{
								bigBoss.xTo = (bigBoss.xFirst = (int)msg.reader().readShort());
								bigBoss.yTo = (bigBoss.yFirst = (int)msg.reader().readShort());
								bigBoss.status = 2;
							}
							bool flag110 = b32 == 9;
							if (flag110)
							{
								bigBoss.x = (bigBoss.y = (bigBoss.xTo = (bigBoss.yTo = (bigBoss.xFirst = (bigBoss.yFirst = -1000)))));
							}
						}
						break;
					}
					case 102:
					{
						sbyte b34 = msg.reader().readByte();
						bool flag111 = b34 == 0 || b34 == 1 || b34 == 2 || b34 == 6;
						if (flag111)
						{
							BigBoss2 bigBoss2 = Mob.getBigBoss2();
							bool flag112 = bigBoss2 == null;
							if (flag112)
							{
								break;
							}
							bool flag113 = b34 == 6;
							if (flag113)
							{
								bigBoss2.x = (bigBoss2.y = (bigBoss2.xTo = (bigBoss2.yTo = (bigBoss2.xFirst = (bigBoss2.yFirst = -1000)))));
								break;
							}
							sbyte b35 = msg.reader().readByte();
							global::Char[] array9 = new global::Char[(int)b35];
							long[] array10 = new long[(int)b35];
							for (int num48 = 0; num48 < (int)b35; num48++)
							{
								int num49 = msg.reader().readInt();
								array9[num48] = null;
								bool flag114 = num49 != global::Char.myCharz().charID;
								if (flag114)
								{
									array9[num48] = GameScr.findCharInMap(num49);
								}
								else
								{
									array9[num48] = global::Char.myCharz();
								}
								array10[num48] = msg.reader().readLong();
							}
							bigBoss2.setAttack(array9, array10, b34);
						}
						bool flag115 = b34 == 3 || b34 == 4 || b34 == 5 || b34 == 7;
						if (flag115)
						{
							BachTuoc bachTuoc = Mob.getBachTuoc();
							bool flag116 = bachTuoc == null;
							if (flag116)
							{
								break;
							}
							bool flag117 = b34 == 7;
							if (flag117)
							{
								bachTuoc.x = (bachTuoc.y = (bachTuoc.xTo = (bachTuoc.yTo = (bachTuoc.xFirst = (bachTuoc.yFirst = -1000)))));
								break;
							}
							bool flag118 = b34 == 3 || b34 == 4;
							if (flag118)
							{
								sbyte b36 = msg.reader().readByte();
								global::Char[] array11 = new global::Char[(int)b36];
								long[] array12 = new long[(int)b36];
								for (int num50 = 0; num50 < (int)b36; num50++)
								{
									int num51 = msg.reader().readInt();
									array11[num50] = null;
									bool flag119 = num51 != global::Char.myCharz().charID;
									if (flag119)
									{
										array11[num50] = GameScr.findCharInMap(num51);
									}
									else
									{
										array11[num50] = global::Char.myCharz();
									}
									array12[num50] = msg.reader().readLong();
								}
								bachTuoc.setAttack(array11, array12, b34);
							}
							bool flag120 = b34 == 5;
							if (flag120)
							{
								short num52 = msg.reader().readShort();
								bachTuoc.move(num52);
							}
						}
						bool flag121 = b34 > 9 && b34 < 30;
						if (flag121)
						{
							Controller2.readActionBoss(msg, (int)b34);
						}
						break;
					}
					default:
					{
						switch (b)
						{
						case 113:
							break;
						case 114:
							try
							{
								string text7 = msg.reader().readUTF();
								mSystem.curINAPP = msg.reader().readByte();
								mSystem.maxINAPP = msg.reader().readByte();
								goto IL_266A;
							}
							catch (Exception)
							{
								goto IL_266A;
							}
							break;
						case 115:
						case 116:
						case 117:
						case 118:
						case 119:
						case 120:
						case 126:
							goto IL_266A;
						case 121:
							mSystem.publicID = msg.reader().readUTF();
							mSystem.strAdmob = msg.reader().readUTF();
							Res.outz("SHOW AD public ID= " + mSystem.publicID);
							mSystem.createAdmob();
							goto IL_266A;
						case 122:
						{
							short num53 = msg.reader().readShort();
							Res.outz("second login = " + num53.ToString());
							LoginScr.timeLogin = num53;
							LoginScr.currTimeLogin = (LoginScr.lastTimeLogin = mSystem.currentTimeMillis());
							GameCanvas.endDlg();
							goto IL_266A;
						}
						case 123:
						{
							Res.outz("SET POSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSss");
							int num54 = msg.reader().readInt();
							short num55 = msg.reader().readShort();
							short num56 = msg.reader().readShort();
							sbyte b37 = msg.reader().readByte();
							global::Char char6 = null;
							bool flag122 = num54 == global::Char.myCharz().charID;
							if (flag122)
							{
								char6 = global::Char.myCharz();
							}
							else
							{
								bool flag123 = GameScr.findCharInMap(num54) != null;
								if (flag123)
								{
									char6 = GameScr.findCharInMap(num54);
								}
							}
							bool flag124 = char6 != null;
							if (flag124)
							{
								ServerEffect.addServerEffect((b37 != 0) ? 173 : 60, char6, 1);
								char6.setPos(num55, num56, b37);
							}
							goto IL_266A;
						}
						case 124:
						{
							short num57 = msg.reader().readShort();
							string text8 = msg.reader().readUTF();
							Res.outz("noi chuyen = " + text8 + "npc ID= " + num57.ToString());
							Npc npc2 = GameScr.findNPCInMap(num57);
							if (npc2 != null)
							{
								npc2.addInfo(text8);
							}
							goto IL_266A;
						}
						case 125:
						{
							sbyte b38 = msg.reader().readByte();
							int num58 = msg.reader().readInt();
							bool flag125 = num58 == global::Char.myCharz().charID;
							if (flag125)
							{
								global::Char.myCharz().setFusion(b38);
							}
							else
							{
								bool flag126 = GameScr.findCharInMap(num58) != null;
								if (flag126)
								{
									GameScr.findCharInMap(num58).setFusion(b38);
								}
							}
							goto IL_266A;
						}
						case 127:
							Controller2.readInfoRada(msg);
							goto IL_266A;
						default:
							goto IL_266A;
						}
						int num59 = 0;
						int num60 = 0;
						int num61 = 0;
						short num62 = 0;
						short num63 = 0;
						short num64 = -1;
						try
						{
							num59 = (int)msg.reader().readByte();
							num60 = (int)msg.reader().readByte();
							num61 = (int)msg.reader().readShort();
							num62 = msg.reader().readShort();
							num63 = msg.reader().readShort();
							num64 = msg.reader().readShort();
						}
						catch (Exception)
						{
						}
						EffecMn.addEff(new Effect(num61, (int)num62, (int)num63, num60, num59, (int)num64));
						break;
					}
					}
				}
				IL_266A:;
			}
			catch (Exception ex)
			{
				Res.outz("=====> Controller2 " + ex.StackTrace);
			}
		}

		// Token: 0x06000C6B RID: 3179 RVA: 0x000C24DC File Offset: 0x000C06DC
		private static void readLuckyRound(Message msg)
		{
			try
			{
				sbyte b = msg.reader().readByte();
				bool flag = b == 0;
				if (flag)
				{
					sbyte b2 = msg.reader().readByte();
					short[] array = new short[(int)b2];
					for (int i = 0; i < (int)b2; i++)
					{
						array[i] = msg.reader().readShort();
					}
					sbyte b3 = msg.reader().readByte();
					int num = msg.reader().readInt();
					short num2 = msg.reader().readShort();
					CrackBallScr.gI().SetCrackBallScr(array, (byte)b3, num, num2);
				}
				else
				{
					bool flag2 = b == 1;
					if (flag2)
					{
						sbyte b4 = msg.reader().readByte();
						short[] array2 = new short[(int)b4];
						for (int j = 0; j < (int)b4; j++)
						{
							array2[j] = msg.reader().readShort();
						}
						CrackBallScr.gI().DoneCrackBallScr(array2);
					}
				}
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06000C6C RID: 3180 RVA: 0x000C25E8 File Offset: 0x000C07E8
		private static void readInfoRada(Message msg)
		{
			try
			{
				sbyte b = msg.reader().readByte();
				bool flag = b == 0;
				if (flag)
				{
					RadarScr.gI();
					MyVector myVector = new MyVector(string.Empty);
					short num = msg.reader().readShort();
					int num2 = 0;
					for (int i = 0; i < (int)num; i++)
					{
						Info_RadaScr info_RadaScr = new Info_RadaScr();
						int num3 = (int)msg.reader().readShort();
						int num4 = i + 1;
						int num5 = (int)msg.reader().readShort();
						sbyte b2 = msg.reader().readByte();
						sbyte b3 = msg.reader().readByte();
						sbyte b4 = msg.reader().readByte();
						short num6 = -1;
						global::Char @char = null;
						sbyte b5 = msg.reader().readByte();
						bool flag2 = b5 == 0;
						if (flag2)
						{
							num6 = msg.reader().readShort();
						}
						else
						{
							int num7 = (int)msg.reader().readShort();
							int num8 = (int)msg.reader().readShort();
							int num9 = (int)msg.reader().readShort();
							int num10 = (int)msg.reader().readShort();
							@char = Info_RadaScr.SetCharInfo(num7, num8, num9, num10);
						}
						string text = msg.reader().readUTF();
						string text2 = msg.reader().readUTF();
						sbyte b6 = msg.reader().readByte();
						sbyte b7 = msg.reader().readByte();
						sbyte b8 = msg.reader().readByte();
						ItemOption[] array = null;
						bool flag3 = b8 != 0;
						if (flag3)
						{
							array = new ItemOption[(int)b8];
							for (int j = 0; j < array.Length; j++)
							{
								ItemOption itemOption = Controller.gI().readItemOption(msg);
								sbyte b9 = msg.reader().readByte();
								bool flag4 = itemOption != null;
								if (flag4)
								{
									array[j] = itemOption;
									array[j].activeCard = b9;
								}
							}
						}
						info_RadaScr.SetInfo(num3, num4, num5, b2, b5, num6, text, text2, @char, array);
						info_RadaScr.SetLevel(b6);
						info_RadaScr.SetUse(b7);
						info_RadaScr.SetAmount(b3, b4);
						myVector.addElement(info_RadaScr);
						bool flag5 = b6 > 0;
						if (flag5)
						{
							num2++;
						}
					}
					RadarScr.gI().SetRadarScr(myVector, num2, (int)num);
					RadarScr.gI().switchToMe();
				}
				else
				{
					bool flag6 = b == 1;
					if (flag6)
					{
						int num11 = (int)msg.reader().readShort();
						sbyte b10 = msg.reader().readByte();
						bool flag7 = Info_RadaScr.GetInfo(RadarScr.list, num11) != null;
						if (flag7)
						{
							Info_RadaScr.GetInfo(RadarScr.list, num11).SetUse(b10);
						}
						RadarScr.SetListUse();
					}
					else
					{
						bool flag8 = b == 2;
						if (flag8)
						{
							int num12 = (int)msg.reader().readShort();
							sbyte b11 = msg.reader().readByte();
							int num13 = 0;
							for (int k = 0; k < RadarScr.list.size(); k++)
							{
								Info_RadaScr info_RadaScr2 = (Info_RadaScr)RadarScr.list.elementAt(k);
								bool flag9 = info_RadaScr2 != null;
								if (flag9)
								{
									bool flag10 = info_RadaScr2.id == num12;
									if (flag10)
									{
										info_RadaScr2.SetLevel(b11);
									}
									bool flag11 = info_RadaScr2.level > 0;
									if (flag11)
									{
										num13++;
									}
								}
							}
							RadarScr.SetNum(num13, RadarScr.list.size());
							bool flag12 = Info_RadaScr.GetInfo(RadarScr.listUse, num12) != null;
							if (flag12)
							{
								Info_RadaScr.GetInfo(RadarScr.listUse, num12).SetLevel(b11);
							}
						}
						else
						{
							bool flag13 = b == 3;
							if (flag13)
							{
								int num14 = (int)msg.reader().readShort();
								sbyte b12 = msg.reader().readByte();
								sbyte b13 = msg.reader().readByte();
								bool flag14 = Info_RadaScr.GetInfo(RadarScr.list, num14) != null;
								if (flag14)
								{
									Info_RadaScr.GetInfo(RadarScr.list, num14).SetAmount(b12, b13);
								}
								bool flag15 = Info_RadaScr.GetInfo(RadarScr.listUse, num14) != null;
								if (flag15)
								{
									Info_RadaScr.GetInfo(RadarScr.listUse, num14).SetAmount(b12, b13);
								}
							}
							else
							{
								bool flag16 = b == 4;
								if (flag16)
								{
									int num15 = msg.reader().readInt();
									short num16 = msg.reader().readShort();
									global::Char char2 = ((num15 != global::Char.myCharz().charID) ? GameScr.findCharInMap(num15) : global::Char.myCharz());
									bool flag17 = char2 != null;
									if (flag17)
									{
										char2.idAuraEff = num16;
										char2.idEff_Set_Item = (short)msg.reader().readByte();
									}
								}
							}
						}
					}
				}
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06000C6D RID: 3181 RVA: 0x000C2A9C File Offset: 0x000C0C9C
		private static void readInfoEffChar(Message msg)
		{
			try
			{
				sbyte b = msg.reader().readByte();
				int num = msg.reader().readInt();
				global::Char @char = ((num != global::Char.myCharz().charID) ? GameScr.findCharInMap(num) : global::Char.myCharz());
				bool flag = b == 0;
				if (flag)
				{
					int num2 = (int)msg.reader().readShort();
					int num3 = (int)msg.reader().readByte();
					int num4 = (int)msg.reader().readByte();
					short num5 = msg.reader().readShort();
					sbyte b2 = msg.reader().readByte();
					if (@char != null)
					{
						@char.addEffChar(new Effect(num2, @char, num3, num4, (int)num5, b2));
					}
				}
				else
				{
					bool flag2 = b == 1;
					if (flag2)
					{
						int num6 = (int)msg.reader().readShort();
						if (@char != null)
						{
							@char.removeEffChar(0, num6);
						}
					}
					else
					{
						bool flag3 = b == 2;
						if (flag3)
						{
							if (@char != null)
							{
								@char.removeEffChar(-1, 0);
							}
						}
					}
				}
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06000C6E RID: 3182 RVA: 0x000C2BA8 File Offset: 0x000C0DA8
		private static void readActionBoss(Message msg, int actionBoss)
		{
			try
			{
				sbyte b = msg.reader().readByte();
				NewBoss newBoss = Mob.getNewBoss(b);
				bool flag = newBoss == null;
				if (!flag)
				{
					bool flag2 = actionBoss == 10;
					if (flag2)
					{
						short num = msg.reader().readShort();
						short num2 = msg.reader().readShort();
						newBoss.move(num, num2);
					}
					bool flag3 = actionBoss >= 11 && actionBoss <= 20;
					if (flag3)
					{
						sbyte b2 = msg.reader().readByte();
						global::Char[] array = new global::Char[(int)b2];
						long[] array2 = new long[(int)b2];
						for (int i = 0; i < (int)b2; i++)
						{
							int num3 = msg.reader().readInt();
							array[i] = null;
							bool flag4 = num3 != global::Char.myCharz().charID;
							if (flag4)
							{
								array[i] = GameScr.findCharInMap(num3);
							}
							else
							{
								array[i] = global::Char.myCharz();
							}
							array2[i] = msg.reader().readLong();
						}
						sbyte b3 = msg.reader().readByte();
						newBoss.setAttack(array, array2, (sbyte)(actionBoss - 10), b3);
					}
					bool flag5 = actionBoss == 21;
					if (flag5)
					{
						newBoss.xTo = (int)msg.reader().readShort();
						newBoss.yTo = (int)msg.reader().readShort();
						newBoss.setFly();
					}
					bool flag6 = actionBoss == 22;
					if (flag6)
					{
					}
					bool flag7 = actionBoss == 23;
					if (flag7)
					{
						newBoss.setDie();
					}
				}
			}
			catch (Exception)
			{
			}
		}
	}
}
using System;

namespace Assets.src.g
{
	// Token: 0x020000F3 RID: 243
	public class BigBoss : Mob, IMapObject
	{
		// Token: 0x06000C07 RID: 3079 RVA: 0x000BB4CC File Offset: 0x000B96CC
		public BigBoss(int id, short px, short py, int templateID, long hp, long maxhp, int s)
		{
			this.xFirst = (this.x = (int)(px + 20));
			this.y = (int)py;
			this.yFirst = (int)py;
			this.mobId = id;
			this.hp = hp;
			this.maxHp = maxhp;
			this.templateId = templateID;
			this.w_hp_bar = 100;
			this.h_hp_bar = 6;
			this.len = this.w_hp_bar;
			base.updateHp_bar();
			bool flag = s == 0;
			if (flag)
			{
				this.getDataB();
			}
			bool flag2 = s == 1;
			if (flag2)
			{
				this.getDataB2();
			}
			bool flag3 = s == 2;
			if (flag3)
			{
				this.getDataB2();
				this.haftBody = true;
			}
			this.status = 2;
		}

		// Token: 0x06000C08 RID: 3080 RVA: 0x000BB6B4 File Offset: 0x000B98B4
		public void getDataB2()
		{
			BigBoss.data = null;
			BigBoss.data = new EffectData();
			string text = string.Concat(new string[]
			{
				"/x",
				mGraphics.zoomLevel.ToString(),
				"/effectdata/",
				100.ToString(),
				"/data"
			});
			try
			{
				BigBoss.data.readData2(text);
				BigBoss.data.img = GameCanvas.loadImage("/effectdata/" + 100.ToString() + "/img.png");
			}
			catch (Exception)
			{
				Service.gI().requestModTemplate(this.templateId);
			}
			this.status = 2;
			this.w = BigBoss.data.width;
			this.h = BigBoss.data.height;
		}

		// Token: 0x06000C09 RID: 3081 RVA: 0x000BB798 File Offset: 0x000B9998
		public void getDataB()
		{
			BigBoss.data = null;
			BigBoss.data = new EffectData();
			string text = string.Concat(new string[]
			{
				"/x",
				mGraphics.zoomLevel.ToString(),
				"/effectdata/",
				101.ToString(),
				"/data"
			});
			try
			{
				BigBoss.data.readData2(text);
				BigBoss.data.img = GameCanvas.loadImage("/effectdata/" + 101.ToString() + "/img.png");
				Res.outz("read xong data");
			}
			catch (Exception)
			{
				Service.gI().requestModTemplate(this.templateId);
			}
			this.w = BigBoss.data.width;
			this.h = BigBoss.data.height;
		}

		// Token: 0x06000C0A RID: 3082 RVA: 0x00004141 File Offset: 0x00002341
		public override void setBody(short id)
		{
			this.changBody = true;
			this.smallBody = id;
		}

		// Token: 0x06000C0B RID: 3083 RVA: 0x00004152 File Offset: 0x00002352
		public override void clearBody()
		{
			this.changBody = false;
		}

		// Token: 0x06000C0C RID: 3084 RVA: 0x00012DD4 File Offset: 0x00010FD4
		public new static bool isExistNewMob(string id)
		{
			for (int i = 0; i < Mob.newMob.size(); i++)
			{
				string text = (string)Mob.newMob.elementAt(i);
				bool flag = text.Equals(id);
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000C0D RID: 3085 RVA: 0x000BB880 File Offset: 0x000B9A80
		public new void checkFrameTick(int[] array)
		{
			this.tick++;
			bool flag = this.tick > array.Length - 1;
			if (flag)
			{
				this.tick = 0;
			}
			this.frame = array[this.tick];
		}

		// Token: 0x06000C0E RID: 3086 RVA: 0x000BB8C4 File Offset: 0x000B9AC4
		private void updateShadown()
		{
			int size = (int)TileMap.size;
			this.xSd = this.x;
			this.wCount = 0;
			bool flag = this.ySd <= 0 || TileMap.tileTypeAt(this.xSd, this.ySd, 2);
			if (!flag)
			{
				bool flag2 = TileMap.tileTypeAt(this.xSd / size, this.ySd / size) == 0;
				if (flag2)
				{
					this.isOutMap = true;
				}
				else
				{
					bool flag3 = TileMap.tileTypeAt(this.xSd / size, this.ySd / size) != 0 && !TileMap.tileTypeAt(this.xSd, this.ySd, 2);
					if (flag3)
					{
						this.xSd = this.x;
						this.ySd = this.y;
						this.isOutMap = false;
					}
				}
				while (this.isOutMap && this.wCount < 10)
				{
					this.wCount++;
					this.ySd += 24;
					bool flag4 = TileMap.tileTypeAt(this.xSd, this.ySd, 2);
					if (flag4)
					{
						bool flag5 = this.ySd % 24 != 0;
						if (flag5)
						{
							this.ySd -= this.ySd % 24;
						}
						break;
					}
				}
			}
		}

		// Token: 0x06000C0F RID: 3087 RVA: 0x000BBA10 File Offset: 0x000B9C10
		private void paintShadow(mGraphics g)
		{
			g.drawImage(BigBoss.shadowBig, this.xSd, this.yFirst, 3);
			g.setClip(GameScr.cmx, GameScr.cmy - GameCanvas.transY, GameScr.gW, GameScr.gH + 2 * GameCanvas.transY);
		}

		// Token: 0x06000C10 RID: 3088 RVA: 0x00003E4C File Offset: 0x0000204C
		public new void updateSuperEff()
		{
		}

		// Token: 0x06000C11 RID: 3089 RVA: 0x000BBA60 File Offset: 0x000B9C60
		public override void update()
		{
			bool flag = !this.isUpdate();
			if (!flag)
			{
				this.updateShadown();
				switch (this.status)
				{
				case 0:
				case 1:
					this.updateDead();
					break;
				case 2:
					this.updateMobStandWait();
					break;
				case 3:
					this.updateMobAttack();
					break;
				case 4:
					this.timeStatus = 0;
					this.updateMobFly();
					break;
				case 5:
					this.timeStatus = 0;
					this.updateMobWalk();
					break;
				case 6:
				{
					this.timeStatus = 0;
					this.p1++;
					this.y += this.p1;
					bool flag2 = this.y >= this.yFirst;
					if (flag2)
					{
						this.y = this.yFirst;
						this.p1 = 0;
						this.status = 5;
					}
					break;
				}
				case 7:
					this.updateInjure();
					break;
				}
			}
		}

		// Token: 0x06000C12 RID: 3090 RVA: 0x000BBB64 File Offset: 0x000B9D64
		private void updateDead()
		{
			this.checkFrameTick((!this.haftBody) ? this.stand : this.stand_1);
			bool flag = GameCanvas.gameTick % 5 == 0;
			if (flag)
			{
				ServerEffect.addServerEffect(167, Res.random(this.x - this.getW() / 2, this.x + this.getW() / 2), Res.random(this.getY() + this.getH() / 2, this.getY() + this.getH()), 1);
			}
			bool flag2 = this.x != this.xFirst || this.y != this.yFirst;
			if (flag2)
			{
				this.x += (this.xFirst - this.x) / 4;
				this.y += (this.yFirst - this.y) / 4;
			}
		}

		// Token: 0x06000C13 RID: 3091 RVA: 0x000BBC50 File Offset: 0x000B9E50
		private void updateMobFly()
		{
			bool flag = this.flyUp;
			if (flag)
			{
				this.dy++;
				this.y -= this.dy;
				this.checkFrameTick(this.fly);
				bool flag2 = this.y <= -500;
				if (flag2)
				{
					this.flyUp = false;
					this.flyDown = true;
					this.dy = 0;
				}
			}
			bool flag3 = this.flyDown;
			if (flag3)
			{
				this.x = this.xTo;
				this.dy += 2;
				this.y += this.dy;
				this.checkFrameTick(this.hitground);
				bool flag4 = this.y > this.yFirst;
				if (flag4)
				{
					this.y = this.yFirst;
					this.flyDown = false;
					this.dy = 0;
					this.status = 2;
					GameScr.shock_scr = 10;
					this.shock = true;
				}
			}
		}

		// Token: 0x06000C14 RID: 3092 RVA: 0x00003E4C File Offset: 0x0000204C
		public new void setInjure()
		{
		}

		// Token: 0x06000C15 RID: 3093 RVA: 0x000BBD4C File Offset: 0x000B9F4C
		public new void setAttack(global::Char cFocus)
		{
			this.isBusyAttackSomeOne = true;
			this.mobToAttack = null;
			this.cFocus = cFocus;
			this.p1 = 0;
			this.p2 = 0;
			this.status = 3;
			this.tick = 0;
			this.dir = ((cFocus.cx > this.x) ? 1 : (-1));
			int cx = cFocus.cx;
			int cy = cFocus.cy;
			bool flag = Res.abs(cx - this.x) < this.w * 2 && Res.abs(cy - this.y) < this.h * 2;
			if (flag)
			{
				bool flag2 = this.x < cx;
				if (flag2)
				{
					this.x = cx - this.w;
				}
				else
				{
					this.x = cx + this.w;
				}
				this.p3 = 0;
			}
			else
			{
				this.p3 = 1;
			}
		}

		// Token: 0x06000C16 RID: 3094 RVA: 0x000132BC File Offset: 0x000114BC
		private bool isSpecial()
		{
			return (this.templateId >= 58 && this.templateId <= 65) || this.templateId == 67 || this.templateId == 68;
		}

		// Token: 0x06000C17 RID: 3095 RVA: 0x00003E4C File Offset: 0x0000204C
		private void updateInjure()
		{
		}

		// Token: 0x06000C18 RID: 3096 RVA: 0x000BBE2C File Offset: 0x000BA02C
		private void updateMobStandWait()
		{
			this.checkFrameTick((!this.haftBody) ? this.stand : this.stand_1);
			bool flag = this.x != this.xFirst || this.y != this.yFirst;
			if (flag)
			{
				this.x += (this.xFirst - this.x) / 4;
				this.y += (this.yFirst - this.y) / 4;
			}
		}

		// Token: 0x06000C19 RID: 3097 RVA: 0x00006B50 File Offset: 0x00004D50
		public void setFly()
		{
			this.status = 4;
			this.flyUp = true;
		}

		// Token: 0x06000C1A RID: 3098 RVA: 0x000BBEB8 File Offset: 0x000BA0B8
		public void setAttack(global::Char[] cAttack, long[] dame, sbyte type)
		{
			this.charAttack = cAttack;
			this.dameHP = dame;
			this.type = type;
			this.tick = 0;
			bool flag = type < 3;
			if (flag)
			{
				this.status = 3;
			}
			bool flag2 = type == 3;
			if (flag2)
			{
				this.flyUp = true;
				this.status = 4;
			}
			bool flag3 = type == 4;
			if (flag3)
			{
				for (int i = 0; i < this.charAttack.Length; i++)
				{
					this.charAttack[i].doInjure(this.dameHP[i], 0L, false, false);
				}
			}
			bool flag4 = type == 7;
			if (flag4)
			{
				this.status = 3;
			}
		}

		// Token: 0x06000C1B RID: 3099 RVA: 0x000BBF60 File Offset: 0x000BA160
		public new void updateMobAttack()
		{
			bool flag = this.type == 7;
			if (flag)
			{
				bool flag2 = this.tick > 8;
				if (flag2)
				{
					this.tick = 8;
				}
				this.checkFrameTick(this.attack1);
				bool flag3 = GameCanvas.gameTick % 4 == 0;
				if (flag3)
				{
					ServerEffect.addServerEffect(70, this.x + ((this.dir != 1) ? (-15) : 15), this.y - 40, 1);
				}
			}
			bool flag4 = this.type == 0;
			if (flag4)
			{
				bool flag5 = this.tick == this.attack1.Length - 1;
				if (flag5)
				{
					this.status = 2;
				}
				this.dir = ((this.x < this.charAttack[0].cx) ? 1 : (-1));
				this.checkFrameTick(this.attack1);
				bool flag6 = this.tick == 8;
				if (flag6)
				{
					for (int i = 0; i < this.charAttack.Length; i++)
					{
						MonsterDart.addMonsterDart(this.x + ((this.dir != 1) ? (-45) : 45), this.y - 30, true, this.dameHP[i], 0L, this.charAttack[i], 24);
					}
				}
			}
			bool flag7 = this.type == 1;
			if (flag7)
			{
				bool flag8 = this.tick == ((!this.haftBody) ? (this.attack2.Length - 1) : (this.attack2_1.Length - 1));
				if (flag8)
				{
					this.status = 2;
				}
				this.dir = ((this.x < this.charAttack[0].cx) ? 1 : (-1));
				this.checkFrameTick((!this.haftBody) ? this.attack2 : this.attack2_1);
				this.x += (this.charAttack[0].cx - this.x) / 4;
				this.y += (this.charAttack[0].cy - this.y) / 4;
				bool flag9 = this.tick == 18;
				if (flag9)
				{
					for (int j = 0; j < this.charAttack.Length; j++)
					{
						this.charAttack[j].doInjure(this.dameHP[j], 0L, false, false);
						ServerEffect.addServerEffect(102, this.charAttack[j].cx, this.charAttack[j].cy, 1);
					}
				}
			}
			bool flag10 = this.type == 8;
			if (flag10)
			{
			}
			bool flag11 = this.type != 2;
			if (!flag11)
			{
				bool flag12 = this.tick == ((!this.haftBody) ? (this.attack3.Length - 1) : (this.attack3_1.Length - 1));
				if (flag12)
				{
					this.status = 2;
				}
				this.dir = ((this.x < this.charAttack[0].cx) ? 1 : (-1));
				this.checkFrameTick((!this.haftBody) ? this.attack3 : this.attack3_1);
				bool flag13 = this.tick == 13;
				if (flag13)
				{
					GameScr.shock_scr = 10;
					this.shock = true;
					for (int k = 0; k < this.charAttack.Length; k++)
					{
						this.charAttack[k].doInjure(this.dameHP[k], 0L, false, false);
					}
				}
			}
		}

		// Token: 0x06000C1C RID: 3100 RVA: 0x00003E4C File Offset: 0x0000204C
		public new void updateMobWalk()
		{
		}

		// Token: 0x06000C1D RID: 3101 RVA: 0x0001360C File Offset: 0x0001180C
		public new bool isPaint()
		{
			bool flag = this.x < GameScr.cmx;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = this.x > GameScr.cmx + GameScr.gW;
				if (flag3)
				{
					flag2 = false;
				}
				else
				{
					bool flag4 = this.y < GameScr.cmy;
					if (flag4)
					{
						flag2 = false;
					}
					else
					{
						bool flag5 = this.y > GameScr.cmy + GameScr.gH + 30;
						if (flag5)
						{
							flag2 = false;
						}
						else
						{
							bool flag6 = this.status == 0;
							flag2 = !flag6;
						}
					}
				}
			}
			return flag2;
		}

		// Token: 0x06000C1E RID: 3102 RVA: 0x0001369C File Offset: 0x0001189C
		public new bool isUpdate()
		{
			bool flag = this.status == 0;
			return !flag;
		}

		// Token: 0x06000C1F RID: 3103 RVA: 0x000136C4 File Offset: 0x000118C4
		public new bool checkIsBoss()
		{
			return this.isBoss || this.levelBoss > 0;
		}

		// Token: 0x06000C20 RID: 3104 RVA: 0x000BC2D8 File Offset: 0x000BA4D8
		public override void paint(mGraphics g)
		{
			bool flag = BigBoss.data == null || this.isHide;
			if (!flag)
			{
				bool isMafuba = this.isMafuba;
				if (isMafuba)
				{
					bool flag2 = !this.changBody;
					if (flag2)
					{
						BigBoss.data.paintFrame(g, this.frame, this.xMFB, this.yMFB, (this.dir != 1) ? 1 : 0, 2);
					}
					else
					{
						SmallImage.drawSmallImage(g, (int)this.smallBody, this.xMFB, this.yMFB, (this.dir != 1) ? 2 : 0, mGraphics.BOTTOM | mGraphics.HCENTER);
					}
				}
				else
				{
					bool flag3 = this.isShadown && this.status != 0;
					if (flag3)
					{
						this.paintShadow(g);
					}
					g.translate(0, GameCanvas.transY);
					bool flag4 = !this.changBody;
					if (flag4)
					{
						BigBoss.data.paintFrame(g, this.frame, this.x, this.y + this.fy, (this.dir != 1) ? 1 : 0, 2);
					}
					else
					{
						SmallImage.drawSmallImage(g, (int)this.smallBody, this.x, this.y + this.fy - 9, (this.dir != 1) ? 2 : 0, mGraphics.BOTTOM | mGraphics.HCENTER);
					}
					g.translate(0, -GameCanvas.transY);
					int imageWidth = mGraphics.getImageWidth(this.imgHPtem);
					int imageHeight = mGraphics.getImageHeight(this.imgHPtem);
					int num = imageWidth;
					int num2 = this.x - imageWidth;
					int num3 = this.y - this.h - 5;
					int num4 = imageWidth * 2 * this.per / 100;
					bool flag5 = num4 > num;
					int num5;
					if (flag5)
					{
						num5 = num4 - num;
						bool flag6 = num5 <= 0;
						if (flag6)
						{
							num5 = 0;
						}
					}
					else
					{
						num = num4;
						num5 = 0;
					}
					g.drawImage(GameScr.imgHP_tm_xam, num2, num3, mGraphics.TOP | mGraphics.LEFT);
					g.drawImage(GameScr.imgHP_tm_xam, num2 + imageWidth, num3, mGraphics.TOP | mGraphics.LEFT);
					g.drawRegion(this.imgHPtem, 0, 0, num, imageHeight, 0, num2, num3, mGraphics.TOP | mGraphics.LEFT);
					g.drawRegion(this.imgHPtem, 0, 0, num5, imageHeight, 0, num2 + imageWidth, num3, mGraphics.TOP | mGraphics.LEFT);
					bool flag7 = this.shock;
					if (flag7)
					{
						Res.outz("type= " + this.type.ToString());
						this.tShock++;
						Effect effect = new Effect((this.type != 2) ? 22 : 19, this.x + this.tShock * 50, this.y + 25, 2, 1, -1);
						EffecMn.addEff(effect);
						Effect effect2 = new Effect((this.type != 2) ? 22 : 19, this.x - this.tShock * 50, this.y + 25, 2, 1, -1);
						EffecMn.addEff(effect2);
						bool flag8 = this.tShock == 50;
						if (flag8)
						{
							this.tShock = 0;
							this.shock = false;
						}
					}
				}
			}
		}

		// Token: 0x06000C21 RID: 3105 RVA: 0x00013A08 File Offset: 0x00011C08
		public new int getHPColor()
		{
			return 16711680;
		}

		// Token: 0x06000C22 RID: 3106 RVA: 0x00006B61 File Offset: 0x00004D61
		public new void startDie()
		{
			this.hp = 0L;
			this.injureThenDie = true;
			this.hp = 0L;
			this.status = 1;
			this.p1 = -3;
			this.p2 = -this.dir;
			this.p3 = 0;
		}

		// Token: 0x06000C23 RID: 3107 RVA: 0x000BC604 File Offset: 0x000BA804
		public new void attackOtherMob(Mob mobToAttack)
		{
			this.mobToAttack = mobToAttack;
			this.isBusyAttackSomeOne = true;
			this.cFocus = null;
			this.p1 = 0;
			this.p2 = 0;
			this.status = 3;
			this.tick = 0;
			this.dir = ((mobToAttack.x > this.x) ? 1 : (-1));
			int x = mobToAttack.x;
			int y = mobToAttack.y;
			bool flag = Res.abs(x - this.x) < this.w * 2 && Res.abs(y - this.y) < this.h * 2;
			if (flag)
			{
				bool flag2 = this.x < x;
				if (flag2)
				{
					this.x = x - this.w;
				}
				else
				{
					this.x = x + this.w;
				}
				this.p3 = 0;
			}
			else
			{
				this.p3 = 1;
			}
		}

		// Token: 0x06000C24 RID: 3108 RVA: 0x00013B00 File Offset: 0x00011D00
		public new int getX()
		{
			return this.x;
		}

		// Token: 0x06000C25 RID: 3109 RVA: 0x000BC6E4 File Offset: 0x000BA8E4
		public new int getY()
		{
			return (!this.haftBody) ? (this.y - 60) : (this.y - 20);
		}

		// Token: 0x06000C26 RID: 3110 RVA: 0x00013B34 File Offset: 0x00011D34
		public new int getH()
		{
			return 40;
		}

		// Token: 0x06000C27 RID: 3111 RVA: 0x000BC714 File Offset: 0x000BA914
		public new int getW()
		{
			return 60;
		}

		// Token: 0x06000C28 RID: 3112 RVA: 0x000BC728 File Offset: 0x000BA928
		public new void stopMoving()
		{
			bool flag = this.status == 5;
			if (flag)
			{
				this.status = 2;
				this.p1 = (this.p2 = (this.p3 = 0));
				this.forceWait = 50;
			}
		}

		// Token: 0x06000C29 RID: 3113 RVA: 0x00013B90 File Offset: 0x00011D90
		public new bool isInvisible()
		{
			return this.status == 0 || this.status == 1;
		}

		// Token: 0x06000C2A RID: 3114 RVA: 0x00013BB8 File Offset: 0x00011DB8
		public new void removeHoldEff()
		{
			bool flag = this.holdEffID != 0;
			if (flag)
			{
				this.holdEffID = 0;
			}
		}

		// Token: 0x06000C2B RID: 3115 RVA: 0x00006B9E File Offset: 0x00004D9E
		public new void removeBlindEff()
		{
			this.blindEff = false;
		}

		// Token: 0x06000C2C RID: 3116 RVA: 0x00006BA8 File Offset: 0x00004DA8
		public new void removeSleepEff()
		{
			this.sleepEff = false;
		}

		// Token: 0x040014F6 RID: 5366
		public static Image shadowBig = GameCanvas.loadImage("/mainImage/shadowBig.png");

		// Token: 0x040014F7 RID: 5367
		public static EffectData data;

		// Token: 0x040014F8 RID: 5368
		public int xTo;

		// Token: 0x040014F9 RID: 5369
		public int yTo;

		// Token: 0x040014FA RID: 5370
		public bool haftBody;

		// Token: 0x040014FB RID: 5371
		public bool change;

		// Token: 0x040014FC RID: 5372
		public new int xSd;

		// Token: 0x040014FD RID: 5373
		public new int ySd;

		// Token: 0x040014FE RID: 5374
		private bool isOutMap;

		// Token: 0x040014FF RID: 5375
		private int wCount;

		// Token: 0x04001500 RID: 5376
		public new bool isShadown = true;

		// Token: 0x04001501 RID: 5377
		private int tick;

		// Token: 0x04001502 RID: 5378
		private int frame;

		// Token: 0x04001503 RID: 5379
		private bool wy;

		// Token: 0x04001504 RID: 5380
		private int wt;

		// Token: 0x04001505 RID: 5381
		private int fy;

		// Token: 0x04001506 RID: 5382
		private int ty;

		// Token: 0x04001507 RID: 5383
		public new int typeSuperEff;

		// Token: 0x04001508 RID: 5384
		private global::Char focus;

		// Token: 0x04001509 RID: 5385
		private bool flyUp;

		// Token: 0x0400150A RID: 5386
		private bool flyDown;

		// Token: 0x0400150B RID: 5387
		private int dy;

		// Token: 0x0400150C RID: 5388
		public bool changePos;

		// Token: 0x0400150D RID: 5389
		private int tShock;

		// Token: 0x0400150E RID: 5390
		public new bool isBusyAttackSomeOne = true;

		// Token: 0x0400150F RID: 5391
		private int tA;

		// Token: 0x04001510 RID: 5392
		private global::Char[] charAttack;

		// Token: 0x04001511 RID: 5393
		private long[] dameHP;

		// Token: 0x04001512 RID: 5394
		private sbyte type;

		// Token: 0x04001513 RID: 5395
		public new int[] stand = new int[]
		{
			0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
			1, 1
		};

		// Token: 0x04001514 RID: 5396
		public int[] stand_1 = new int[]
		{
			37, 37, 37, 38, 38, 38, 39, 39, 40, 40,
			40, 39, 39, 39, 38, 38, 38
		};

		// Token: 0x04001515 RID: 5397
		public new int[] move = new int[]
		{
			1, 1, 1, 1, 2, 2, 2, 2, 3, 3,
			3, 3, 2, 2, 2
		};

		// Token: 0x04001516 RID: 5398
		public new int[] moveFast = new int[] { 1, 1, 2, 2, 3, 3, 2 };

		// Token: 0x04001517 RID: 5399
		public new int[] attack1 = new int[]
		{
			0, 0, 34, 34, 35, 35, 36, 36, 2, 2,
			1, 1
		};

		// Token: 0x04001518 RID: 5400
		public new int[] attack2 = new int[]
		{
			0, 0, 0, 4, 4, 6, 6, 9, 9, 10,
			10, 13, 13, 15, 15, 17, 17, 19, 19, 21,
			21, 23, 23
		};

		// Token: 0x04001519 RID: 5401
		public int[] attack3 = new int[]
		{
			0, 0, 1, 1, 4, 4, 6, 6, 8, 8,
			25, 25, 26, 26, 28, 28, 30, 30, 32, 32,
			2, 2, 1, 1
		};

		// Token: 0x0400151A RID: 5402
		public int[] attack2_1 = new int[]
		{
			37, 37, 5, 5, 7, 7, 11, 11, 14, 14,
			16, 16, 18, 18, 20, 20, 22, 22, 24, 24
		};

		// Token: 0x0400151B RID: 5403
		public int[] attack3_1 = new int[]
		{
			37, 37, 37, 38, 38, 5, 5, 7, 7, 11,
			11, 27, 27, 29, 29, 31, 31, 33, 33, 38,
			38
		};

		// Token: 0x0400151C RID: 5404
		public int[] fly = new int[] { 8, 8, 9, 9, 10, 10, 12, 12 };

		// Token: 0x0400151D RID: 5405
		public int[] hitground = new int[]
		{
			0, 0, 1, 1, 4, 4, 6, 6, 8, 8,
			25, 25, 26, 26, 28, 28, 30, 30, 32, 32,
			2, 2, 1, 1
		};

		// Token: 0x0400151E RID: 5406
		private bool shock;

		// Token: 0x0400151F RID: 5407
		private sbyte[] cou = new sbyte[] { -1, 1 };

		// Token: 0x04001520 RID: 5408
		public new global::Char injureBy;

		// Token: 0x04001521 RID: 5409
		public new bool injureThenDie;

		// Token: 0x04001522 RID: 5410
		public new Mob mobToAttack;

		// Token: 0x04001523 RID: 5411
		public new int forceWait;

		// Token: 0x04001524 RID: 5412
		public new bool blindEff;

		// Token: 0x04001525 RID: 5413
		public new bool sleepEff;
	}
}
using System;

namespace Assets.src.g
{
	// Token: 0x020000F4 RID: 244
	public class ClientInput : mScreen, IActionListener
	{
		// Token: 0x06000C2E RID: 3118 RVA: 0x000BC770 File Offset: 0x000BA970
		private void init(string t)
		{
			this.w = GameCanvas.w - 20;
			bool flag = this.w > 320;
			if (flag)
			{
				this.w = 320;
			}
			Res.outz("title= " + t);
			this.strPaint = mFont.tahoma_7b_dark.splitFontArray(t, this.w - 20);
			this.x = (GameCanvas.w - this.w) / 2;
			this.tf = new TField[this.nTf];
			this.h = this.tf.Length * 35 + (this.strPaint.Length - 1) * 20 + 40;
			this.y = GameCanvas.h - this.h - 40 - (this.strPaint.Length - 1) * 20;
			for (int i = 0; i < this.tf.Length; i++)
			{
				this.tf[i] = new TField();
				this.tf[i].name = string.Empty;
				this.tf[i].x = this.x + 10;
				this.tf[i].y = this.y + 35 + (this.strPaint.Length - 1) * 20 + i * 35;
				this.tf[i].width = this.w - 20;
				this.tf[i].height = mScreen.ITEM_HEIGHT + 2;
				bool isTouch = GameCanvas.isTouch;
				if (isTouch)
				{
					this.tf[0].isFocus = false;
				}
				else
				{
					this.tf[0].isFocus = true;
				}
				bool flag2 = !GameCanvas.isTouch;
				if (flag2)
				{
					this.right = this.tf[0].cmdClear;
				}
			}
			this.left = new Command(mResources.CLOSE, this, 1, null);
			this.center = new Command(mResources.OK, this, 2, null);
			bool isTouch2 = GameCanvas.isTouch;
			if (isTouch2)
			{
				this.center.x = GameCanvas.w / 2 + 18;
				this.left.x = GameCanvas.w / 2 - 85;
				this.center.y = (this.left.y = this.y + this.h + 5);
			}
		}

		// Token: 0x06000C2F RID: 3119 RVA: 0x000BC9BC File Offset: 0x000BABBC
		public static ClientInput gI()
		{
			bool flag = ClientInput.instance == null;
			if (flag)
			{
				ClientInput.instance = new ClientInput();
			}
			return ClientInput.instance;
		}

		// Token: 0x06000C30 RID: 3120 RVA: 0x00006BC3 File Offset: 0x00004DC3
		public override void switchToMe()
		{
			this.focus = 0;
			base.switchToMe();
		}

		// Token: 0x06000C31 RID: 3121 RVA: 0x00006BD4 File Offset: 0x00004DD4
		public void setInput(int type, string title)
		{
			this.nTf = type;
			this.init(title);
			this.switchToMe();
		}

		// Token: 0x06000C32 RID: 3122 RVA: 0x000BC9EC File Offset: 0x000BABEC
		public override void paint(mGraphics g)
		{
			GameScr.gI().paint(g);
			PopUp.paintPopUp(g, this.x, this.y, this.w, this.h, -1, true);
			for (int i = 0; i < this.strPaint.Length; i++)
			{
				mFont.tahoma_7b_green2.drawString(g, this.strPaint[i], GameCanvas.w / 2, this.y + 15 + i * 20, mFont.CENTER);
			}
			for (int j = 0; j < this.tf.Length; j++)
			{
				this.tf[j].paint(g);
			}
			base.paint(g);
		}

		// Token: 0x06000C33 RID: 3123 RVA: 0x000BCAA0 File Offset: 0x000BACA0
		public override void update()
		{
			GameScr.gI().update();
			for (int i = 0; i < this.tf.Length; i++)
			{
				this.tf[i].update();
			}
		}

		// Token: 0x06000C34 RID: 3124 RVA: 0x000BCAE0 File Offset: 0x000BACE0
		public override void keyPress(int keyCode)
		{
			for (int i = 0; i < this.tf.Length; i++)
			{
				bool isFocus = this.tf[i].isFocus;
				if (isFocus)
				{
					this.tf[i].keyPressed(keyCode);
					break;
				}
			}
			base.keyPress(keyCode);
		}

		// Token: 0x06000C35 RID: 3125 RVA: 0x000BCB34 File Offset: 0x000BAD34
		public override void updateKey()
		{
			bool flag = GameCanvas.keyPressed[2];
			if (flag)
			{
				this.focus--;
				bool flag2 = this.focus < 0;
				if (flag2)
				{
					this.focus = this.tf.Length - 1;
				}
			}
			else
			{
				bool flag3 = GameCanvas.keyPressed[8];
				if (flag3)
				{
					this.focus++;
					bool flag4 = this.focus > this.tf.Length - 1;
					if (flag4)
					{
						this.focus = 0;
					}
				}
			}
			bool flag5 = GameCanvas.keyPressed[2] || GameCanvas.keyPressed[8];
			if (flag5)
			{
				GameCanvas.clearKeyPressed();
				for (int i = 0; i < this.tf.Length; i++)
				{
					bool flag6 = this.focus == i;
					if (flag6)
					{
						this.tf[i].isFocus = true;
						bool flag7 = !GameCanvas.isTouch;
						if (flag7)
						{
							this.right = this.tf[i].cmdClear;
						}
					}
					else
					{
						this.tf[i].isFocus = false;
					}
					bool flag8 = GameCanvas.isPointerJustRelease && GameCanvas.isPointerHoldIn(this.tf[i].x, this.tf[i].y, this.tf[i].width, this.tf[i].height);
					if (flag8)
					{
						this.focus = i;
						break;
					}
				}
			}
			base.updateKey();
			GameCanvas.clearKeyPressed();
		}

		// Token: 0x06000C36 RID: 3126 RVA: 0x00006BED File Offset: 0x00004DED
		public void clearScreen()
		{
			ClientInput.instance = null;
		}

		// Token: 0x06000C37 RID: 3127 RVA: 0x000BCCC0 File Offset: 0x000BAEC0
		public void perform(int idAction, object p)
		{
			bool flag = idAction == 1;
			if (flag)
			{
				GameScr.instance.switchToMe();
				this.clearScreen();
			}
			bool flag2 = idAction != 2;
			if (!flag2)
			{
				for (int i = 0; i < this.tf.Length; i++)
				{
					bool flag3 = this.tf[i].getText() == null || this.tf[i].getText().Equals(string.Empty);
					if (flag3)
					{
						GameCanvas.startOKDlg(mResources.vuilongnhapduthongtin);
						return;
					}
				}
				Service.gI().sendClientInput(this.tf);
				GameScr.instance.switchToMe();
			}
		}

		// Token: 0x04001526 RID: 5414
		public static ClientInput instance;

		// Token: 0x04001527 RID: 5415
		public TField[] tf;

		// Token: 0x04001528 RID: 5416
		private int x;

		// Token: 0x04001529 RID: 5417
		private int y;

		// Token: 0x0400152A RID: 5418
		private int w;

		// Token: 0x0400152B RID: 5419
		private int h;

		// Token: 0x0400152C RID: 5420
		private string[] strPaint;

		// Token: 0x0400152D RID: 5421
		private int focus;

		// Token: 0x0400152E RID: 5422
		private int nTf;
	}
}
using System;

namespace Assets.src.g
{
	// Token: 0x020000F5 RID: 245
	internal class GameInfo
	{
		// Token: 0x0400152F RID: 5423
		public string main;

		// Token: 0x04001530 RID: 5424
		public string content;

		// Token: 0x04001531 RID: 5425
		public short id;

		// Token: 0x04001532 RID: 5426
		public bool hasRead;
	}
}
using System;

namespace Assets.src.g
{
	// Token: 0x020000F6 RID: 246
	internal class ImageSource
	{
		// Token: 0x06000C3A RID: 3130 RVA: 0x00006BFF File Offset: 0x00004DFF
		public ImageSource(string ID, sbyte version)
		{
			this.id = ID;
			this.version = version;
		}

		// Token: 0x06000C3B RID: 3131 RVA: 0x000BCD6C File Offset: 0x000BAF6C
		public static void checkRMS()
		{
			MyVector myVector = new MyVector();
			sbyte[] array = Rms.loadRMS("ImageSource");
			bool flag = array == null;
			if (flag)
			{
				Service.gI().imageSource(myVector);
			}
			else
			{
				ImageSource.vRms = new MyVector();
				DataInputStream dataInputStream = new DataInputStream(array);
				bool flag2 = dataInputStream == null;
				if (!flag2)
				{
					try
					{
						short num = dataInputStream.readShort();
						string[] array2 = new string[(int)num];
						sbyte[] array3 = new sbyte[(int)num];
						for (int i = 0; i < (int)num; i++)
						{
							array2[i] = dataInputStream.readUTF();
							array3[i] = dataInputStream.readByte();
							ImageSource.vRms.addElement(new ImageSource(array2[i], array3[i]));
						}
						dataInputStream.close();
					}
					catch (Exception ex)
					{
						ex.StackTrace.ToString();
					}
					Res.outz("vS size= " + ImageSource.vSource.size().ToString() + " vRMS size= " + ImageSource.vRms.size().ToString());
					Service.gI().imageSource(myVector);
				}
			}
		}

		// Token: 0x06000C3C RID: 3132 RVA: 0x000BCEA4 File Offset: 0x000BB0A4
		public static sbyte getVersionRMSByID(string id)
		{
			for (int i = 0; i < ImageSource.vRms.size(); i++)
			{
				bool flag = id.Equals(((ImageSource)ImageSource.vRms.elementAt(i)).id);
				if (flag)
				{
					return ((ImageSource)ImageSource.vRms.elementAt(i)).version;
				}
			}
			return -1;
		}

		// Token: 0x06000C3D RID: 3133 RVA: 0x000BCF0C File Offset: 0x000BB10C
		public static sbyte getCurrVersionByID(string id)
		{
			for (int i = 0; i < ImageSource.vSource.size(); i++)
			{
				bool flag = id.Equals(((ImageSource)ImageSource.vSource.elementAt(i)).id);
				if (flag)
				{
					return ((ImageSource)ImageSource.vSource.elementAt(i)).version;
				}
			}
			return -1;
		}

		// Token: 0x06000C3E RID: 3134 RVA: 0x000BCF74 File Offset: 0x000BB174
		public static bool isExistID(string id)
		{
			for (int i = 0; i < ImageSource.vRms.size(); i++)
			{
				bool flag = id.Equals(((ImageSource)ImageSource.vRms.elementAt(i)).id);
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000C3F RID: 3135 RVA: 0x000BCFC8 File Offset: 0x000BB1C8
		public static void saveRMS()
		{
			DataOutputStream dataOutputStream = new DataOutputStream();
			try
			{
				dataOutputStream.writeShort((short)ImageSource.vSource.size());
				for (int i = 0; i < ImageSource.vSource.size(); i++)
				{
					dataOutputStream.writeUTF(((ImageSource)ImageSource.vSource.elementAt(i)).id);
					dataOutputStream.writeByte(((ImageSource)ImageSource.vSource.elementAt(i)).version);
				}
				Rms.saveRMS("ImageSource", dataOutputStream.toByteArray());
				dataOutputStream.close();
			}
			catch (Exception ex)
			{
				ex.StackTrace.ToString();
			}
		}

		// Token: 0x04001533 RID: 5427
		public sbyte version;

		// Token: 0x04001534 RID: 5428
		public string id;

		// Token: 0x04001535 RID: 5429
		public static MyVector vSource = new MyVector();

		// Token: 0x04001536 RID: 5430
		public static MyVector vRms = new MyVector();
	}
}
using System;

namespace Assets.src.g
{
	// Token: 0x020000F7 RID: 247
	internal class Mabu : global::Char
	{
		// Token: 0x06000C41 RID: 3137 RVA: 0x00006C2D File Offset: 0x00004E2D
		public Mabu()
		{
			this.getData1();
			this.getData2();
		}

		// Token: 0x06000C42 RID: 3138 RVA: 0x000BD080 File Offset: 0x000BB280
		public void eat(int id)
		{
			this.effEat = new Effect(105, this.cx, this.cy + 20, 2, 1, -1);
			EffecMn.addEff(this.effEat);
			bool flag = id == global::Char.myCharz().charID;
			if (flag)
			{
				this.focus = global::Char.myCharz();
			}
			else
			{
				this.focus = GameScr.findCharInMap(id);
			}
		}

		// Token: 0x06000C43 RID: 3139 RVA: 0x000BD0E8 File Offset: 0x000BB2E8
		public new void checkFrameTick(int[] array)
		{
			bool flag = this.skillID == 0;
			if (flag)
			{
				bool flag2 = this.tick == 11;
				if (flag2)
				{
					this.addFoot = true;
					Effect effect = new Effect(19, this.cx, this.cy + 20, 2, 1, -1);
					EffecMn.addEff(effect);
				}
				bool flag3 = this.tick >= array.Length - 1;
				if (flag3)
				{
					this.skillID = 2;
					return;
				}
			}
			bool flag4 = this.skillID == 1 && this.tick == array.Length - 1;
			if (flag4)
			{
				this.skillID = 3;
				this.cy -= 15;
			}
			else
			{
				this.tick++;
				bool flag5 = this.tick > array.Length - 1;
				if (flag5)
				{
					this.tick = 0;
				}
				this.frame = array[this.tick];
			}
		}

		// Token: 0x06000C44 RID: 3140 RVA: 0x000BD1CC File Offset: 0x000BB3CC
		public void getData1()
		{
			Mabu.data1 = null;
			Mabu.data1 = new EffectData();
			string text = string.Concat(new string[]
			{
				"/x",
				mGraphics.zoomLevel.ToString(),
				"/effectdata/",
				102.ToString(),
				"/data"
			});
			try
			{
				Mabu.data1.readData2(text);
				Mabu.data1.img = GameCanvas.loadImage("/effectdata/" + 102.ToString() + "/img.png");
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06000C45 RID: 3141 RVA: 0x000BD278 File Offset: 0x000BB478
		public void setSkill(sbyte id, short x, short y, global::Char[] charHit, long[] damageHit)
		{
			this.skillID = id;
			this.xTo = (int)x;
			this.yTo = (int)y;
			this.lastDir = this.cdir;
			this.cdir = ((this.xTo > this.cx) ? 1 : (-1));
			this.charAttack = charHit;
			this.damageAttack = damageHit;
		}

		// Token: 0x06000C46 RID: 3142 RVA: 0x000BD2D0 File Offset: 0x000BB4D0
		public void getData2()
		{
			Mabu.data2 = null;
			Mabu.data2 = new EffectData();
			string text = string.Concat(new string[]
			{
				"/x",
				mGraphics.zoomLevel.ToString(),
				"/effectdata/",
				103.ToString(),
				"/data"
			});
			try
			{
				Mabu.data2.readData2(text);
				Mabu.data2.img = GameCanvas.loadImage("/effectdata/" + 103.ToString() + "/img.png");
				Res.outz("read xong data");
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06000C47 RID: 3143 RVA: 0x000BD384 File Offset: 0x000BB584
		public override void update()
		{
			bool flag = this.focus != null;
			if (flag)
			{
				bool flag2 = this.effEat.t >= 30;
				if (flag2)
				{
					this.effEat.x += (this.cx - this.effEat.x) / 4;
					this.effEat.y += (this.cy - this.effEat.y) / 4;
					this.focus.cx = this.effEat.x;
					this.focus.cy = this.effEat.y;
					this.focus.isMabuHold = true;
				}
				else
				{
					this.effEat.trans = ((this.effEat.x > this.focus.cx) ? 1 : 0);
					this.effEat.x += (this.focus.cx - this.effEat.x) / 3;
					this.effEat.y += (this.focus.cy - this.effEat.y) / 3;
				}
			}
			bool flag3 = this.skillID != -1;
			if (flag3)
			{
				bool flag4 = this.skillID == 0 && this.addFoot && GameCanvas.gameTick % 2 == 0;
				if (flag4)
				{
					this.dx += ((this.xTo <= this.cx) ? (-30) : 30);
					EffecMn.addEff(new Effect(103, this.cx + this.dx, this.cy + 20, 2, 1, -1)
					{
						trans = ((this.xTo <= this.cx) ? 1 : 0)
					});
					bool flag5 = (this.cdir == 1 && this.cx + this.dx >= this.xTo) || (this.cdir == -1 && this.cx + this.dx <= this.xTo);
					if (flag5)
					{
						this.addFoot = false;
						this.skillID = -1;
						this.dx = 0;
						this.tick = 0;
						this.cdir = this.lastDir;
						for (int i = 0; i < this.charAttack.Length; i++)
						{
							this.charAttack[i].doInjure(this.damageAttack[i], 0L, false, false);
						}
					}
				}
				bool flag6 = this.skillID != 3;
				if (!flag6)
				{
					this.xTo = this.charAttack[this.pIndex].cx;
					this.yTo = this.charAttack[this.pIndex].cy;
					this.cx += (this.xTo - this.cx) / 3;
					this.cy += (this.yTo - this.cy) / 3;
					bool flag7 = GameCanvas.gameTick % 5 == 0;
					if (flag7)
					{
						Effect effect = new Effect(19, this.cx, this.cy, 2, 1, -1);
						EffecMn.addEff(effect);
					}
					bool flag8 = Res.abs(this.cx - this.xTo) <= 20 && Res.abs(this.cy - this.yTo) <= 20;
					if (flag8)
					{
						this.cx = this.xTo;
						this.cy = this.yTo;
						this.charAttack[this.pIndex].doInjure(this.damageAttack[this.pIndex], 0L, false, false);
						this.pIndex++;
						bool flag9 = this.pIndex == this.charAttack.Length;
						if (flag9)
						{
							this.skillID = -1;
							this.pIndex = 0;
						}
					}
				}
			}
			else
			{
				base.update();
			}
		}

		// Token: 0x06000C48 RID: 3144 RVA: 0x000BD77C File Offset: 0x000BB97C
		public override void paint(mGraphics g)
		{
			bool flag = this.skillID != -1;
			if (flag)
			{
				base.paintShadow(g);
				g.translate(0, GameCanvas.transY);
				this.checkFrameTick(Mabu.skills[(int)this.skillID]);
				bool flag2 = this.skillID == 0 || this.skillID == 1;
				if (flag2)
				{
					Mabu.data1.paintFrame(g, this.frame, this.cx, this.cy + this.fy, (this.cdir != 1) ? 1 : 0, 2);
				}
				else
				{
					Mabu.data2.paintFrame(g, this.frame, this.cx, this.cy + this.fy, (this.cdir != 1) ? 1 : 0, 2);
				}
				g.translate(0, -GameCanvas.transY);
			}
			else
			{
				base.paint(g);
			}
		}

		// Token: 0x04001537 RID: 5431
		public static EffectData data1;

		// Token: 0x04001538 RID: 5432
		public static EffectData data2;

		// Token: 0x04001539 RID: 5433
		private new int tick;

		// Token: 0x0400153A RID: 5434
		private int lastDir;

		// Token: 0x0400153B RID: 5435
		private bool addFoot;

		// Token: 0x0400153C RID: 5436
		private Effect effEat;

		// Token: 0x0400153D RID: 5437
		private new global::Char focus;

		// Token: 0x0400153E RID: 5438
		public int xTo;

		// Token: 0x0400153F RID: 5439
		public int yTo;

		// Token: 0x04001540 RID: 5440
		public bool haftBody;

		// Token: 0x04001541 RID: 5441
		public bool change;

		// Token: 0x04001542 RID: 5442
		private global::Char[] charAttack;

		// Token: 0x04001543 RID: 5443
		private long[] damageAttack;

		// Token: 0x04001544 RID: 5444
		private int dx;

		// Token: 0x04001545 RID: 5445
		public static int[] skill1 = new int[]
		{
			0, 0, 1, 1, 2, 2, 3, 3, 4, 4,
			5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
			5, 5, 5, 5, 5, 5, 5, 5, 5, 5
		};

		// Token: 0x04001546 RID: 5446
		public static int[] skill2 = new int[]
		{
			0, 0, 6, 6, 7, 7, 8, 8, 9, 9,
			9, 9, 9, 10, 10
		};

		// Token: 0x04001547 RID: 5447
		public static int[] skill3 = new int[]
		{
			0, 0, 1, 1, 2, 2, 3, 3, 4, 4,
			5, 5, 6, 6, 7, 7, 8, 8, 9, 9,
			10, 10, 11, 11, 12, 12
		};

		// Token: 0x04001548 RID: 5448
		public static int[] skill4 = new int[] { 13, 13, 14, 14, 15, 15, 16, 16 };

		// Token: 0x04001549 RID: 5449
		public static int[][] skills = new int[][]
		{
			Mabu.skill1,
			Mabu.skill2,
			Mabu.skill3,
			Mabu.skill4
		};

		// Token: 0x0400154A RID: 5450
		public sbyte skillID = -1;

		// Token: 0x0400154B RID: 5451
		private int frame;

		// Token: 0x0400154C RID: 5452
		private int pIndex;
	}
}
using System;

namespace Assets.src.g
{
	// Token: 0x020000F8 RID: 248
	public class PetFollow
	{
		// Token: 0x06000C4A RID: 3146 RVA: 0x00006C4C File Offset: 0x00004E4C
		public PetFollow()
		{
			this.f = Res.random(0, 3);
		}

		// Token: 0x06000C4B RID: 3147 RVA: 0x000BD8FC File Offset: 0x000BBAFC
		public void SetImg(int fimg, int[] frameNew, int wimg, int himg)
		{
			bool flag = fimg >= 1;
			if (flag)
			{
				this.fimg = fimg;
				this.frame = frameNew;
				this.wimg = wimg;
				this.himg = himg;
			}
		}

		// Token: 0x06000C4C RID: 3148 RVA: 0x000BD934 File Offset: 0x000BBB34
		public void paint(mGraphics g)
		{
			int num = 32;
			int num2 = 32;
			int num3 = ((GameCanvas.gameTick % 10 > 5) ? 1 : 0);
			bool flag = this.fimg > 0;
			if (flag)
			{
				num = this.wimg;
				num2 = this.himg;
				num3 = 0;
			}
			SmallImage.drawSmallImage(g, (int)this.smallID, this.f, this.cmx, this.cmy + 3 + num3, num, num2, (this.dir != 1) ? 2 : 0, StaticObj.VCENTER_HCENTER);
		}

		// Token: 0x06000C4D RID: 3149 RVA: 0x000BD9B0 File Offset: 0x000BBBB0
		public void update()
		{
			this.moveCamera();
			bool flag = GameCanvas.gameTick % 3 == 0;
			if (flag)
			{
				this.f = this.frame[this.count];
				this.count++;
			}
			bool flag2 = this.count >= this.frame.Length;
			if (flag2)
			{
				this.count = 0;
			}
		}

		// Token: 0x06000C4E RID: 3150 RVA: 0x00006C8C File Offset: 0x00004E8C
		public void remove()
		{
			ServerEffect.addServerEffect(60, this.cmx, this.cmy + 3 + ((GameCanvas.gameTick % 10 > 5) ? 1 : 0), 1);
		}

		// Token: 0x06000C4F RID: 3151 RVA: 0x000BDA18 File Offset: 0x000BBC18
		public void moveCamera()
		{
			bool flag = this.cmy != this.cmtoY;
			if (flag)
			{
				this.cmvy = this.cmtoY - this.cmy << 2;
				this.cmdy += this.cmvy;
				this.cmy += this.cmdy >> 4;
				this.cmdy &= 15;
			}
			bool flag2 = this.cmx != this.cmtoX;
			if (flag2)
			{
				this.cmvx = this.cmtoX - this.cmx << 2;
				this.cmdx += this.cmvx;
				this.cmx += this.cmdx >> 4;
				this.cmdx &= 15;
			}
		}

		// Token: 0x0400154D RID: 5453
		public short smallID;

		// Token: 0x0400154E RID: 5454
		public Info info = new Info();

		// Token: 0x0400154F RID: 5455
		public int dir;

		// Token: 0x04001550 RID: 5456
		public int f;

		// Token: 0x04001551 RID: 5457
		public int tF;

		// Token: 0x04001552 RID: 5458
		public int cmtoY;

		// Token: 0x04001553 RID: 5459
		public int cmy;

		// Token: 0x04001554 RID: 5460
		public int cmdy;

		// Token: 0x04001555 RID: 5461
		public int cmvy;

		// Token: 0x04001556 RID: 5462
		public int cmyLim;

		// Token: 0x04001557 RID: 5463
		public int cmtoX;

		// Token: 0x04001558 RID: 5464
		public int cmx;

		// Token: 0x04001559 RID: 5465
		public int cmdx;

		// Token: 0x0400155A RID: 5466
		public int cmvx;

		// Token: 0x0400155B RID: 5467
		public int cmxLim;

		// Token: 0x0400155C RID: 5468
		public int fimg = -1;

		// Token: 0x0400155D RID: 5469
		public int wimg;

		// Token: 0x0400155E RID: 5470
		public int himg;

		// Token: 0x0400155F RID: 5471
		private int[] frame = new int[] { 0, 1, 2, 1 };

		// Token: 0x04001560 RID: 5472
		private int count;
	}
}
using System;

namespace Assets.src.g
{
	// Token: 0x020000F9 RID: 249
	public class RegisterScreen : mScreen, IActionListener
	{
		// Token: 0x06000C50 RID: 3152 RVA: 0x000BDAEC File Offset: 0x000BBCEC
		public RegisterScreen(sbyte haveName)
		{
			this.yLog = 130;
			TileMap.bgID = (int)((sbyte)(mSystem.currentTimeMillis() % 9L));
			bool flag = TileMap.bgID == 5 || TileMap.bgID == 6;
			if (flag)
			{
				TileMap.bgID = 4;
			}
			GameScr.loadCamera(true, -1, -1);
			GameScr.cmx = 100;
			GameScr.cmy = 200;
			bool flag2 = GameCanvas.h > 200;
			if (flag2)
			{
				this.defYL = GameCanvas.hh - 80;
			}
			else
			{
				this.defYL = GameCanvas.hh - 65;
			}
			this.resetLogo();
			this.wC = ((GameCanvas.w < 200) ? 140 : 160);
			this.yt = GameCanvas.hh - mScreen.ITEM_HEIGHT - 5;
			bool flag3 = GameCanvas.h <= 160;
			if (flag3)
			{
				this.yt = 20;
			}
			this.tfSodt = new TField();
			this.tfSodt.setIputType(TField.INPUT_TYPE_NUMERIC);
			this.tfSodt.width = 220;
			this.tfSodt.height = mScreen.ITEM_HEIGHT + 2;
			this.tfSodt.name = "Số điện thoại/ địa chỉ email";
			bool flag4 = haveName == 1;
			if (flag4)
			{
				this.tfSodt.setText("01234567890");
			}
			this.tfUser = new TField();
			this.tfUser.width = 220;
			this.tfUser.height = mScreen.ITEM_HEIGHT + 2;
			this.tfUser.isFocus = true;
			this.tfUser.name = "Họ và tên";
			bool flag5 = haveName == 1;
			if (flag5)
			{
				this.tfUser.setText("Nguyễn Văn A");
			}
			this.tfUser.setIputType(TField.INPUT_TYPE_ANY);
			this.tfNgay = new TField();
			this.tfNgay.setIputType(TField.INPUT_TYPE_NUMERIC);
			this.tfNgay.width = 70;
			this.tfNgay.height = mScreen.ITEM_HEIGHT + 2;
			this.tfNgay.name = "Ngày sinh";
			bool flag6 = haveName == 1;
			if (flag6)
			{
				this.tfNgay.setText("01");
			}
			this.tfThang = new TField();
			this.tfThang.setIputType(TField.INPUT_TYPE_NUMERIC);
			this.tfThang.width = 70;
			this.tfThang.height = mScreen.ITEM_HEIGHT + 2;
			this.tfThang.name = "Tháng sinh";
			bool flag7 = haveName == 1;
			if (flag7)
			{
				this.tfThang.setText("01");
			}
			this.tfNam = new TField();
			this.tfNam.setIputType(TField.INPUT_TYPE_NUMERIC);
			this.tfNam.width = 70;
			this.tfNam.height = mScreen.ITEM_HEIGHT + 2;
			this.tfNam.name = "Năm sinh";
			bool flag8 = haveName == 1;
			if (flag8)
			{
				this.tfNam.setText("1990");
			}
			this.tfDiachi = new TField();
			this.tfDiachi.setIputType(TField.INPUT_TYPE_ANY);
			this.tfDiachi.width = 220;
			this.tfDiachi.height = mScreen.ITEM_HEIGHT + 2;
			this.tfDiachi.name = "Địa chỉ đăng ký thường trú";
			bool flag9 = haveName == 1;
			if (flag9)
			{
				this.tfDiachi.setText("123 đường số 1, Quận 1, TP.HCM");
			}
			this.tfCMND = new TField();
			this.tfCMND.setIputType(TField.INPUT_TYPE_NUMERIC);
			this.tfCMND.width = 220;
			this.tfCMND.height = mScreen.ITEM_HEIGHT + 2;
			this.tfCMND.name = "Số Chứng minh nhân dân hoặc số hộ chiếu";
			bool flag10 = haveName == 1;
			if (flag10)
			{
				this.tfCMND.setText("123456789");
			}
			this.tfNgayCap = new TField();
			this.tfNgayCap.setIputType(TField.INPUT_TYPE_ANY);
			this.tfNgayCap.width = 220;
			this.tfNgayCap.height = mScreen.ITEM_HEIGHT + 2;
			this.tfNgayCap.name = "Ngày cấp";
			bool flag11 = haveName == 1;
			if (flag11)
			{
				this.tfNgayCap.setText("01/01/2005");
			}
			this.tfNoiCap = new TField();
			this.tfNoiCap.setIputType(TField.INPUT_TYPE_ANY);
			this.tfNoiCap.width = 220;
			this.tfNoiCap.height = mScreen.ITEM_HEIGHT + 2;
			this.tfNoiCap.name = "Nơi cấp";
			bool flag12 = haveName == 1;
			if (flag12)
			{
				this.tfNoiCap.setText("TP.HCM");
			}
			this.yt += 35;
			this.isCheck = true;
			this.focus = 0;
			this.cmdLogin = new Command((GameCanvas.w <= 200) ? mResources.login2 : mResources.login, GameCanvas.instance, 888393, null);
			this.cmdCheck = new Command(mResources.remember, this, 2001, null);
			this.cmdRes = new Command(mResources.register, this, 2002, null);
			this.cmdBackFromRegister = new Command(mResources.CANCEL, this, 10021, null);
			this.left = (this.cmdMenu = new Command(mResources.MENU, this, 2003, null));
			bool isTouch = GameCanvas.isTouch;
			if (isTouch)
			{
				this.cmdLogin.x = GameCanvas.w / 2 - 100;
				this.cmdMenu.x = GameCanvas.w / 2 - mScreen.cmdW - 8;
				bool flag13 = GameCanvas.h >= 200;
				if (flag13)
				{
					this.cmdLogin.y = GameCanvas.h / 2 - 40;
					this.cmdMenu.y = this.yLog + 110;
				}
				this.cmdBackFromRegister.x = GameCanvas.w / 2 + 3;
				this.cmdBackFromRegister.y = this.yLog + 110;
				this.cmdRes.x = GameCanvas.w / 2 - 84;
				this.cmdRes.y = this.cmdMenu.y;
			}
			this.wP = 170;
			this.hP = ((!this.isRes) ? 100 : 110);
			this.xP = GameCanvas.hw - this.wP / 2;
			this.yP = this.tfUser.y - 15;
			int num = 4;
			int num2 = num * 32 + 23 + 33;
			bool flag14 = num2 >= GameCanvas.w;
			if (flag14)
			{
				num--;
				num2 = num * 32 + 23 + 33;
			}
			this.xLog = GameCanvas.w / 2 - num2 / 2;
			this.yLog = 5;
			this.lY = ((GameCanvas.w < 200) ? (this.tfUser.y - 30) : (this.yLog - 30));
			this.tfUser.x = this.xLog + 10;
			this.tfUser.y = this.yLog + 20;
			this.cmdOK = new Command(mResources.OK, this, 2008, null);
			this.cmdOK.x = 260;
			this.cmdOK.y = GameCanvas.h - 60;
			this.cmdFogetPass = new Command("Thoát", this, 1003, null);
			this.cmdFogetPass.x = 260;
			this.cmdFogetPass.y = GameCanvas.h - 30;
			bool flag15 = GameCanvas.w < 250;
			if (flag15)
			{
				this.cmdOK.x = GameCanvas.w / 2 - 80;
				this.cmdFogetPass.x = GameCanvas.w / 2 + 10;
				this.cmdFogetPass.y = (this.cmdOK.y = GameCanvas.h - 25);
			}
			this.center = this.cmdOK;
			this.left = this.cmdFogetPass;
		}

		// Token: 0x06000C51 RID: 3153 RVA: 0x000BE334 File Offset: 0x000BC534
		public new void switchToMe()
		{
			Res.outz("Res switch");
			SoundMn.gI().stopAll();
			this.focus = 0;
			this.tfUser.isFocus = true;
			this.tfNgay.isFocus = false;
			bool isTouch = GameCanvas.isTouch;
			if (isTouch)
			{
				this.tfUser.isFocus = false;
				this.focus = -1;
			}
			base.switchToMe();
		}

		// Token: 0x06000C52 RID: 3154 RVA: 0x000BE39C File Offset: 0x000BC59C
		protected void doMenu()
		{
			MyVector myVector = new MyVector("vMenu Login");
			myVector.addElement(new Command(mResources.registerNewAcc, this, 2004, null));
			bool flag = !this.isLogin2;
			if (flag)
			{
				myVector.addElement(new Command(mResources.selectServer, this, 1004, null));
			}
			myVector.addElement(new Command(mResources.forgetPass, this, 1003, null));
			myVector.addElement(new Command(mResources.website, this, 1005, null));
			int num = Rms.loadRMSInt("lowGraphic");
			bool flag2 = num == 1;
			if (flag2)
			{
				myVector.addElement(new Command(mResources.increase_vga, this, 10041, null));
			}
			else
			{
				myVector.addElement(new Command(mResources.decrease_vga, this, 10042, null));
			}
			myVector.addElement(new Command(mResources.EXIT, GameCanvas.instance, 8885, null));
			GameCanvas.menu.startAt(myVector, 0);
		}

		// Token: 0x06000C53 RID: 3155 RVA: 0x000BE498 File Offset: 0x000BC698
		protected void doRegister()
		{
			bool flag = this.tfUser.getText().Equals(string.Empty);
			if (flag)
			{
				GameCanvas.startOKDlg(mResources.userBlank);
			}
			else
			{
				char[] array = this.tfUser.getText().ToCharArray();
				bool flag2 = this.tfNgay.getText().Equals(string.Empty);
				if (flag2)
				{
					GameCanvas.startOKDlg(mResources.passwordBlank);
				}
				else
				{
					bool flag3 = this.tfUser.getText().Length < 5;
					if (flag3)
					{
						GameCanvas.startOKDlg(mResources.accTooShort);
					}
					else
					{
						int num = 0;
						string text = null;
						bool flag4 = mResources.language == 2;
						if (flag4)
						{
							bool flag5 = this.tfUser.getText().IndexOf("@") == -1 || this.tfUser.getText().IndexOf(".") == -1;
							if (flag5)
							{
								text = mResources.emailInvalid;
							}
							num = 0;
						}
						else
						{
							try
							{
								long num2 = long.Parse(this.tfUser.getText());
								bool flag6 = this.tfUser.getText().Length < 8 || this.tfUser.getText().Length > 12 || (!this.tfUser.getText().StartsWith("0") && !this.tfUser.getText().StartsWith("84"));
								if (flag6)
								{
									text = mResources.phoneInvalid;
								}
								num = 1;
							}
							catch (Exception)
							{
								bool flag7 = this.tfUser.getText().IndexOf("@") == -1 || this.tfUser.getText().IndexOf(".") == -1;
								if (flag7)
								{
									text = mResources.emailInvalid;
								}
								num = 0;
							}
						}
						bool flag8 = text != null;
						if (flag8)
						{
							GameCanvas.startOKDlg(text);
						}
						else
						{
							GameCanvas.msgdlg.setInfo(string.Concat(new string[]
							{
								mResources.plsCheckAcc,
								(num != 1) ? (mResources.email + ": ") : (mResources.phone + ": "),
								this.tfUser.getText(),
								"\n",
								mResources.password,
								": ",
								this.tfNgay.getText()
							}), new Command(mResources.ACCEPT, this, 4000, null), null, new Command(mResources.NO, GameCanvas.instance, 8882, null));
						}
						GameCanvas.currentDialog = GameCanvas.msgdlg;
					}
				}
			}
		}

		// Token: 0x06000C54 RID: 3156 RVA: 0x00003E4C File Offset: 0x0000204C
		protected void doRegister(string user)
		{
		}

		// Token: 0x06000C55 RID: 3157 RVA: 0x000BE734 File Offset: 0x000BC934
		public void doViewFAQ()
		{
			bool flag = !this.listFAQ.Equals(string.Empty) || !this.listFAQ.Equals(string.Empty);
			if (flag)
			{
			}
			bool flag2 = !Session_ME.connected;
			if (flag2)
			{
				this.isFAQ = true;
				GameCanvas.connect();
			}
			GameCanvas.startWaitDlg();
		}

		// Token: 0x06000C56 RID: 3158 RVA: 0x000BE794 File Offset: 0x000BC994
		protected void doSelectServer()
		{
			MyVector myVector = new MyVector("vServer");
			bool flag = RegisterScreen.isLocal;
			if (flag)
			{
				myVector.addElement(new Command("Server LOCAL", this, 20004, null));
			}
			myVector.addElement(new Command("Server Bokken", this, 20001, null));
			myVector.addElement(new Command("Server Shuriken", this, 20002, null));
			myVector.addElement(new Command("Server Tessen (mới)", this, 20003, null));
			GameCanvas.menu.startAt(myVector, 0);
			bool flag2 = this.loadIndexServer() != -1 && !GameCanvas.isTouch;
			if (flag2)
			{
				GameCanvas.menu.menuSelectedItem = this.loadIndexServer();
			}
		}

		// Token: 0x06000C57 RID: 3159 RVA: 0x00005305 File Offset: 0x00003505
		protected void saveIndexServer(int index)
		{
			Rms.saveRMSInt("indServer", index);
		}

		// Token: 0x06000C58 RID: 3160 RVA: 0x0006C02C File Offset: 0x0006A22C
		protected int loadIndexServer()
		{
			return Rms.loadRMSInt("indServer");
		}

		// Token: 0x06000C59 RID: 3161 RVA: 0x00003E4C File Offset: 0x0000204C
		public void doLogin()
		{
		}

		// Token: 0x06000C5A RID: 3162 RVA: 0x00003E4C File Offset: 0x0000204C
		public void savePass()
		{
		}

		// Token: 0x06000C5B RID: 3163 RVA: 0x000BE850 File Offset: 0x000BCA50
		public override void update()
		{
			this.tfUser.update();
			this.tfNgay.update();
			this.tfThang.update();
			this.tfNam.update();
			this.tfDiachi.update();
			this.tfCMND.update();
			this.tfNoiCap.update();
			this.tfSodt.update();
			this.tfNgayCap.update();
			for (int i = 0; i < Effect2.vEffect2.size(); i++)
			{
				Effect2 effect = (Effect2)Effect2.vEffect2.elementAt(i);
				effect.update();
			}
			bool flag = RegisterScreen.isUpdateAll && !RegisterScreen.isUpdateData && !RegisterScreen.isUpdateItem && !RegisterScreen.isUpdateMap && !RegisterScreen.isUpdateSkill;
			if (flag)
			{
				RegisterScreen.isUpdateAll = false;
				mSystem.gcc();
				Service.gI().finishUpdate();
			}
			GameScr.cmx++;
			bool flag2 = GameScr.cmx > GameCanvas.w * 3 + 100;
			if (flag2)
			{
				GameScr.cmx = 100;
			}
			bool flag3 = ChatPopup.currChatPopup != null;
			if (!flag3)
			{
				GameCanvas.debug("LGU1", 0);
				GameCanvas.debug("LGU2", 0);
				GameCanvas.debug("LGU3", 0);
				this.updateLogo();
				GameCanvas.debug("LGU4", 0);
				GameCanvas.debug("LGU5", 0);
				bool flag4 = this.g >= 0;
				if (flag4)
				{
					this.ylogo += this.dir * this.g;
					this.g += this.dir * this.v;
					bool flag5 = this.g <= 0;
					if (flag5)
					{
						this.dir *= -1;
					}
					bool flag6 = this.ylogo > 0;
					if (flag6)
					{
						this.dir *= -1;
						this.g -= 2 * this.v;
					}
				}
				GameCanvas.debug("LGU6", 0);
				bool flag7 = this.tipid >= 0 && GameCanvas.gameTick % 100 == 0;
				if (flag7)
				{
					this.doChangeTip();
				}
				bool isTouch = GameCanvas.isTouch;
				if (isTouch)
				{
					bool flag8 = this.isRes;
					if (flag8)
					{
						this.center = this.cmdRes;
						this.left = this.cmdBackFromRegister;
					}
					else
					{
						this.center = this.cmdOK;
						this.left = this.cmdFogetPass;
					}
				}
				else
				{
					bool flag9 = this.isRes;
					if (flag9)
					{
						this.center = this.cmdRes;
						this.left = this.cmdBackFromRegister;
					}
					else
					{
						this.center = this.cmdOK;
						this.left = this.cmdFogetPass;
					}
				}
			}
		}

		// Token: 0x06000C5C RID: 3164 RVA: 0x000BEB2C File Offset: 0x000BCD2C
		private void doChangeTip()
		{
			this.tipid++;
			bool flag = this.tipid >= mResources.tips.Length;
			if (flag)
			{
				this.tipid = 0;
			}
			bool flag2 = GameCanvas.currentDialog == GameCanvas.msgdlg && GameCanvas.msgdlg.isWait;
			if (flag2)
			{
				GameCanvas.msgdlg.setInfo(mResources.tips[this.tipid]);
			}
		}

		// Token: 0x06000C5D RID: 3165 RVA: 0x000BEBA0 File Offset: 0x000BCDA0
		public void updateLogo()
		{
			bool flag = this.defYL != this.yL;
			if (flag)
			{
				this.yL += this.defYL - this.yL >> 1;
			}
		}

		// Token: 0x06000C5E RID: 3166 RVA: 0x000BEBE4 File Offset: 0x000BCDE4
		public override void keyPress(int keyCode)
		{
			bool isFocus = this.tfUser.isFocus;
			if (isFocus)
			{
				this.tfUser.keyPressed(keyCode);
			}
			else
			{
				bool isFocus2 = this.tfNgay.isFocus;
				if (isFocus2)
				{
					this.tfNgay.keyPressed(keyCode);
				}
				else
				{
					bool isFocus3 = this.tfThang.isFocus;
					if (isFocus3)
					{
						this.tfThang.keyPressed(keyCode);
					}
					else
					{
						bool isFocus4 = this.tfNam.isFocus;
						if (isFocus4)
						{
							this.tfNam.keyPressed(keyCode);
						}
						else
						{
							bool isFocus5 = this.tfDiachi.isFocus;
							if (isFocus5)
							{
								this.tfDiachi.keyPressed(keyCode);
							}
							else
							{
								bool isFocus6 = this.tfCMND.isFocus;
								if (isFocus6)
								{
									this.tfCMND.keyPressed(keyCode);
								}
								else
								{
									bool isFocus7 = this.tfNoiCap.isFocus;
									if (isFocus7)
									{
										this.tfNoiCap.keyPressed(keyCode);
									}
									else
									{
										bool isFocus8 = this.tfSodt.isFocus;
										if (isFocus8)
										{
											this.tfSodt.keyPressed(keyCode);
										}
										else
										{
											bool isFocus9 = this.tfNgayCap.isFocus;
											if (isFocus9)
											{
												this.tfNgayCap.keyPressed(keyCode);
											}
										}
									}
								}
							}
						}
					}
				}
			}
			base.keyPress(keyCode);
		}

		// Token: 0x06000C5F RID: 3167 RVA: 0x00005314 File Offset: 0x00003514
		public override void unLoad()
		{
			base.unLoad();
		}

		// Token: 0x06000C60 RID: 3168 RVA: 0x000BED34 File Offset: 0x000BCF34
		public override void paint(mGraphics g)
		{
			GameCanvas.debug("PLG1", 1);
			GameCanvas.paintBGGameScr(g);
			GameCanvas.debug("PLG2", 2);
			int num = this.tfUser.y - 50;
			bool flag = GameCanvas.h <= 220;
			if (flag)
			{
				num += 5;
			}
			bool flag2 = ChatPopup.currChatPopup != null || ChatPopup.serverChatPopUp != null;
			if (!flag2)
			{
				bool flag3 = GameCanvas.currentDialog == null;
				if (flag3)
				{
					this.xLog = 5;
					int num2 = 233;
					bool flag4 = GameCanvas.w < 260;
					if (flag4)
					{
						this.xLog = (GameCanvas.w - 240) / 2;
					}
					this.yLog = (GameCanvas.h - num2) / 2;
					int num3 = ((GameCanvas.w < 200) ? 160 : 180);
					PopUp.paintPopUp(g, this.xLog, this.yLog, 240, num2, -1, true);
					bool flag5 = GameCanvas.h > 160 && RegisterScreen.imgTitle != null;
					if (flag5)
					{
						g.drawImage(RegisterScreen.imgTitle, GameCanvas.hw, num, 3);
					}
					GameCanvas.debug("PLG4", 1);
					int num4 = 4;
					int num5 = num4 * 32 + 23 + 33;
					bool flag6 = num5 >= GameCanvas.w;
					if (flag6)
					{
						num4--;
						num5 = num4 * 32 + 23 + 33;
					}
					this.tfSodt.x = this.xLog + 10;
					this.tfSodt.y = this.yLog + 15;
					this.tfUser.x = this.tfSodt.x;
					this.tfUser.y = this.tfSodt.y + 30;
					this.tfNgay.x = this.xLog + 10;
					this.tfNgay.y = this.tfUser.y + 30;
					this.tfThang.x = this.tfNgay.x + 75;
					this.tfThang.y = this.tfNgay.y;
					this.tfNam.x = this.tfThang.x + 75;
					this.tfNam.y = this.tfThang.y;
					this.tfDiachi.x = this.tfUser.x;
					this.tfDiachi.y = this.tfNgay.y + 30;
					this.tfCMND.x = this.tfUser.x;
					this.tfCMND.y = this.tfDiachi.y + 30;
					this.tfNgayCap.x = this.tfUser.x;
					this.tfNgayCap.y = this.tfCMND.y + 30;
					this.tfNoiCap.x = this.tfUser.x;
					this.tfNoiCap.y = this.tfNgayCap.y + 30;
					this.tfUser.paint(g);
					this.tfNgay.paint(g);
					this.tfThang.paint(g);
					this.tfNam.paint(g);
					this.tfDiachi.paint(g);
					this.tfCMND.paint(g);
					this.tfNgayCap.paint(g);
					this.tfNoiCap.paint(g);
					this.tfSodt.paint(g);
					bool flag7 = GameCanvas.w >= 176;
					if (!flag7)
					{
						mFont.tahoma_7b_green2.drawString(g, mResources.acc + ":", this.tfUser.x - 35, this.tfUser.y + 7, 0);
						mFont.tahoma_7b_green2.drawString(g, mResources.pwd + ":", this.tfNgay.x - 35, this.tfNgay.y + 7, 0);
						mFont.tahoma_7b_green2.drawString(g, mResources.server + ": " + RegisterScreen.serverName, GameCanvas.w / 2, this.tfNgay.y + 32, 2);
						bool flag8 = this.isRes;
						if (flag8)
						{
						}
					}
				}
				string version = GameMidlet.VERSION;
				g.setColor(GameCanvas.skyColor);
				g.fillRect(GameCanvas.w - 40, 4, 36, 11);
				mFont.tahoma_7_grey.drawString(g, version, GameCanvas.w - 22, 4, mFont.CENTER);
				GameCanvas.resetTrans(g);
				bool flag9 = GameCanvas.currentDialog == null;
				if (flag9)
				{
					bool flag10 = GameCanvas.w > 250;
					if (flag10)
					{
						mFont.tahoma_7b_white.drawString(g, "Dưới 18 tuổi", 260, 10, 0, mFont.tahoma_7b_dark);
						mFont.tahoma_7b_white.drawString(g, "chỉ có thể chơi", 260, 25, 0, mFont.tahoma_7b_dark);
						mFont.tahoma_7b_white.drawString(g, "180p 1 ngày", 260, 40, 0, mFont.tahoma_7b_dark);
					}
					else
					{
						mFont.tahoma_7b_white.drawString(g, "Dưới 18 tuổi chỉ có thể chơi", GameCanvas.w / 2, 5, 2, mFont.tahoma_7b_dark);
						mFont.tahoma_7b_white.drawString(g, "180p 1 ngày", GameCanvas.w / 2, 15, 2, mFont.tahoma_7b_dark);
					}
				}
				base.paint(g);
			}
		}

		// Token: 0x06000C61 RID: 3169 RVA: 0x000BF2AC File Offset: 0x000BD4AC
		private void turnOffFocus()
		{
			this.tfUser.isFocus = false;
			this.tfNgay.isFocus = false;
			this.tfThang.isFocus = false;
			this.tfNam.isFocus = false;
			this.tfDiachi.isFocus = false;
			this.tfCMND.isFocus = false;
			this.tfNgayCap.isFocus = false;
			this.tfNoiCap.isFocus = false;
			this.tfSodt.isFocus = false;
		}

		// Token: 0x06000C62 RID: 3170 RVA: 0x000BF328 File Offset: 0x000BD528
		private void processFocus()
		{
			this.turnOffFocus();
			switch (this.focus)
			{
			case 0:
				this.tfUser.isFocus = true;
				break;
			case 1:
				this.tfNgay.isFocus = true;
				break;
			case 2:
				this.tfThang.isFocus = true;
				break;
			case 3:
				this.tfNam.isFocus = true;
				break;
			case 4:
				this.tfDiachi.isFocus = true;
				break;
			case 5:
				this.tfCMND.isFocus = true;
				break;
			case 6:
				this.tfNgayCap.isFocus = true;
				break;
			case 7:
				this.tfNoiCap.isFocus = true;
				break;
			case 8:
				this.tfSodt.isFocus = true;
				break;
			}
		}

		// Token: 0x06000C63 RID: 3171 RVA: 0x000BF3F0 File Offset: 0x000BD5F0
		public override void updateKey()
		{
			bool flag = RegisterScreen.isContinueToLogin;
			if (!flag)
			{
				bool flag2 = !GameCanvas.isTouch;
				if (flag2)
				{
					bool isFocus = this.tfUser.isFocus;
					if (isFocus)
					{
						this.right = this.tfUser.cmdClear;
					}
					else
					{
						bool isFocus2 = this.tfNgay.isFocus;
						if (isFocus2)
						{
							this.right = this.tfNgay.cmdClear;
						}
						else
						{
							bool isFocus3 = this.tfThang.isFocus;
							if (isFocus3)
							{
								this.right = this.tfThang.cmdClear;
							}
							else
							{
								bool isFocus4 = this.tfNam.isFocus;
								if (isFocus4)
								{
									this.right = this.tfNam.cmdClear;
								}
								else
								{
									bool isFocus5 = this.tfDiachi.isFocus;
									if (isFocus5)
									{
										this.right = this.tfDiachi.cmdClear;
									}
									else
									{
										bool isFocus6 = this.tfCMND.isFocus;
										if (isFocus6)
										{
											this.right = this.tfCMND.cmdClear;
										}
										else
										{
											bool isFocus7 = this.tfNgayCap.isFocus;
											if (isFocus7)
											{
												this.right = this.tfNgayCap.cmdClear;
											}
											else
											{
												bool isFocus8 = this.tfNoiCap.isFocus;
												if (isFocus8)
												{
													this.right = this.tfNoiCap.cmdClear;
												}
												else
												{
													bool isFocus9 = this.tfSodt.isFocus;
													if (isFocus9)
													{
														this.right = this.tfSodt.cmdClear;
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
				bool flag3 = GameCanvas.keyPressed[21];
				if (flag3)
				{
					this.focus--;
					bool flag4 = this.focus < 0;
					if (flag4)
					{
						this.focus = 8;
					}
					this.processFocus();
				}
				else
				{
					bool flag5 = GameCanvas.keyPressed[22];
					if (flag5)
					{
						this.focus++;
						bool flag6 = this.focus > 8;
						if (flag6)
						{
							this.focus = 0;
						}
						this.processFocus();
					}
				}
				bool flag7 = GameCanvas.keyPressed[21] || GameCanvas.keyPressed[22];
				if (flag7)
				{
					GameCanvas.clearKeyPressed();
					bool flag8 = !this.isLogin2 || this.isRes;
					if (flag8)
					{
						bool flag9 = this.focus == 1;
						if (flag9)
						{
							this.tfUser.isFocus = false;
							this.tfNgay.isFocus = true;
						}
						else
						{
							bool flag10 = this.focus == 0;
							if (flag10)
							{
								this.tfUser.isFocus = true;
								this.tfNgay.isFocus = false;
							}
							else
							{
								this.tfUser.isFocus = false;
								this.tfNgay.isFocus = false;
							}
						}
					}
				}
				bool isTouch = GameCanvas.isTouch;
				if (isTouch)
				{
					bool flag11 = this.isRes;
					if (flag11)
					{
						this.center = this.cmdRes;
						this.left = this.cmdBackFromRegister;
					}
					else
					{
						this.center = this.cmdOK;
						this.left = this.cmdFogetPass;
					}
				}
				else
				{
					bool flag12 = this.isRes;
					if (flag12)
					{
						this.center = this.cmdRes;
						this.left = this.cmdBackFromRegister;
					}
					else
					{
						this.center = this.cmdOK;
						this.left = this.cmdFogetPass;
					}
				}
				bool isPointerJustRelease = GameCanvas.isPointerJustRelease;
				if (isPointerJustRelease)
				{
					bool flag13 = GameCanvas.isPointerHoldIn(this.tfUser.x, this.tfUser.y, this.tfUser.width, this.tfUser.height);
					if (flag13)
					{
						this.focus = 0;
						this.processFocus();
					}
					else
					{
						bool flag14 = GameCanvas.isPointerHoldIn(this.tfNgay.x, this.tfNgay.y, this.tfNgay.width, this.tfNgay.height);
						if (flag14)
						{
							this.focus = 1;
							this.processFocus();
						}
						else
						{
							bool flag15 = GameCanvas.isPointerHoldIn(this.tfThang.x, this.tfThang.y, this.tfThang.width, this.tfThang.height);
							if (flag15)
							{
								this.focus = 2;
								this.processFocus();
							}
							else
							{
								bool flag16 = GameCanvas.isPointerHoldIn(this.tfNam.x, this.tfNam.y, this.tfNam.width, this.tfNam.height);
								if (flag16)
								{
									this.focus = 3;
									this.processFocus();
								}
								else
								{
									bool flag17 = GameCanvas.isPointerHoldIn(this.tfDiachi.x, this.tfDiachi.y, this.tfDiachi.width, this.tfDiachi.height);
									if (flag17)
									{
										this.focus = 4;
										this.processFocus();
									}
									else
									{
										bool flag18 = GameCanvas.isPointerHoldIn(this.tfCMND.x, this.tfCMND.y, this.tfCMND.width, this.tfCMND.height);
										if (flag18)
										{
											this.focus = 5;
											this.processFocus();
										}
										else
										{
											bool flag19 = GameCanvas.isPointerHoldIn(this.tfNgayCap.x, this.tfNgayCap.y, this.tfNgayCap.width, this.tfNgayCap.height);
											if (flag19)
											{
												this.focus = 6;
												this.processFocus();
											}
											else
											{
												bool flag20 = GameCanvas.isPointerHoldIn(this.tfNoiCap.x, this.tfNoiCap.y, this.tfNoiCap.width, this.tfNoiCap.height);
												if (flag20)
												{
													this.focus = 7;
													this.processFocus();
												}
												else
												{
													bool flag21 = GameCanvas.isPointerHoldIn(this.tfSodt.x, this.tfSodt.y, this.tfSodt.width, this.tfSodt.height);
													if (flag21)
													{
														this.focus = 8;
														this.processFocus();
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
				base.updateKey();
				GameCanvas.clearKeyPressed();
			}
		}

		// Token: 0x06000C64 RID: 3172 RVA: 0x00006CB6 File Offset: 0x00004EB6
		public void resetLogo()
		{
			this.yL = -50;
		}

		// Token: 0x06000C65 RID: 3173 RVA: 0x000BF9FC File Offset: 0x000BDBFC
		public void perform(int idAction, object p)
		{
			if (idAction <= 2008)
			{
				switch (idAction)
				{
				case 1000:
					try
					{
						GameMidlet.instance.platformRequest((string)p);
					}
					catch (Exception ex)
					{
						ex.StackTrace.ToString();
					}
					GameCanvas.endDlg();
					return;
				case 1001:
					GameCanvas.endDlg();
					this.isRes = false;
					return;
				case 1002:
					return;
				case 1003:
					Session_ME.gI().close();
					GameCanvas.serverScreen.switchToMe();
					return;
				case 1004:
					ServerListScreen.doUpdateServer();
					GameCanvas.serverScreen.switchToMe();
					return;
				case 1005:
					try
					{
						GameMidlet.instance.platformRequest("http://ngocrongonline.com");
						return;
					}
					catch (Exception ex2)
					{
						ex2.StackTrace.ToString();
						return;
					}
					break;
				default:
					switch (idAction)
					{
					case 2001:
						break;
					case 2002:
						this.doRegister();
						return;
					case 2003:
						this.doMenu();
						return;
					case 2004:
						this.actRegister();
						return;
					case 2005:
					case 2006:
					case 2007:
						return;
					case 2008:
					{
						bool flag = this.tfNgay.getText().Equals(string.Empty) || this.tfThang.getText().Equals(string.Empty) || this.tfNam.getText().Equals(string.Empty) || this.tfDiachi.getText().Equals(string.Empty) || this.tfCMND.getText().Equals(string.Empty) || this.tfNgayCap.getText().Equals(string.Empty) || this.tfNoiCap.getText().Equals(string.Empty) || this.tfSodt.getText().Equals(string.Empty) || this.tfUser.getText().Equals(string.Empty);
						if (flag)
						{
							GameCanvas.startOKDlg("Vui lòng điền đầy đủ thông tin");
							return;
						}
						GameCanvas.startOKDlg(mResources.PLEASEWAIT);
						Service.gI().charInfo(this.tfNgay.getText(), this.tfThang.getText(), this.tfNam.getText(), this.tfDiachi.getText(), this.tfCMND.getText(), this.tfNgayCap.getText(), this.tfNoiCap.getText(), this.tfSodt.getText(), this.tfUser.getText());
						return;
					}
					default:
						return;
					}
					break;
				}
				bool flag2 = this.isCheck;
				if (flag2)
				{
					this.isCheck = false;
				}
				else
				{
					this.isCheck = true;
				}
			}
			else if (idAction != 4000)
			{
				if (idAction == 10021)
				{
					this.actRegisterLeft();
				}
			}
			else
			{
				this.doRegister(this.tfUser.getText());
			}
		}

		// Token: 0x06000C66 RID: 3174 RVA: 0x000BFD18 File Offset: 0x000BDF18
		public void actRegisterLeft()
		{
			bool flag = this.isLogin2;
			if (flag)
			{
				this.doLogin();
			}
			else
			{
				this.isRes = false;
				this.tfNgay.isFocus = false;
				this.tfUser.isFocus = true;
				this.left = this.cmdMenu;
			}
		}

		// Token: 0x06000C67 RID: 3175 RVA: 0x00006CC1 File Offset: 0x00004EC1
		public void actRegister()
		{
			GameCanvas.endDlg();
			GameCanvas.startOKDlg(mResources.regNote);
			this.isRes = true;
			this.tfNgay.isFocus = false;
			this.tfUser.isFocus = true;
		}

		// Token: 0x06000C68 RID: 3176 RVA: 0x000BFD68 File Offset: 0x000BDF68
		public void backToRegister()
		{
			bool flag = GameCanvas.loginScr.isLogin2;
			if (flag)
			{
				GameCanvas.startYesNoDlg(mResources.note, new Command(mResources.YES, GameCanvas.panel, 10019, null), new Command(mResources.NO, GameCanvas.panel, 10020, null));
			}
			else
			{
				GameCanvas.instance.doResetToLoginScr(GameCanvas.loginScr);
				Session_ME.gI().close();
			}
		}

		// Token: 0x04001561 RID: 5473
		public TField tfUser;

		// Token: 0x04001562 RID: 5474
		public TField tfNgay;

		// Token: 0x04001563 RID: 5475
		public TField tfThang;

		// Token: 0x04001564 RID: 5476
		public TField tfNam;

		// Token: 0x04001565 RID: 5477
		public TField tfDiachi;

		// Token: 0x04001566 RID: 5478
		public TField tfCMND;

		// Token: 0x04001567 RID: 5479
		public TField tfNgayCap;

		// Token: 0x04001568 RID: 5480
		public TField tfNoiCap;

		// Token: 0x04001569 RID: 5481
		public TField tfSodt;

		// Token: 0x0400156A RID: 5482
		public static bool isContinueToLogin = false;

		// Token: 0x0400156B RID: 5483
		private int focus;

		// Token: 0x0400156C RID: 5484
		private int wC;

		// Token: 0x0400156D RID: 5485
		private int yL;

		// Token: 0x0400156E RID: 5486
		private int defYL;

		// Token: 0x0400156F RID: 5487
		public bool isCheck;

		// Token: 0x04001570 RID: 5488
		public bool isRes;

		// Token: 0x04001571 RID: 5489
		private Command cmdLogin;

		// Token: 0x04001572 RID: 5490
		private Command cmdCheck;

		// Token: 0x04001573 RID: 5491
		private Command cmdFogetPass;

		// Token: 0x04001574 RID: 5492
		private Command cmdRes;

		// Token: 0x04001575 RID: 5493
		private Command cmdMenu;

		// Token: 0x04001576 RID: 5494
		private Command cmdBackFromRegister;

		// Token: 0x04001577 RID: 5495
		public string listFAQ = string.Empty;

		// Token: 0x04001578 RID: 5496
		public string titleFAQ;

		// Token: 0x04001579 RID: 5497
		public string subtitleFAQ;

		// Token: 0x0400157A RID: 5498
		private string numSupport = string.Empty;

		// Token: 0x0400157B RID: 5499
		private string strUser;

		// Token: 0x0400157C RID: 5500
		private string strPass;

		// Token: 0x0400157D RID: 5501
		public static bool isLocal = false;

		// Token: 0x0400157E RID: 5502
		public static bool isUpdateAll;

		// Token: 0x0400157F RID: 5503
		public static bool isUpdateData;

		// Token: 0x04001580 RID: 5504
		public static bool isUpdateMap;

		// Token: 0x04001581 RID: 5505
		public static bool isUpdateSkill;

		// Token: 0x04001582 RID: 5506
		public static bool isUpdateItem;

		// Token: 0x04001583 RID: 5507
		public static string serverName;

		// Token: 0x04001584 RID: 5508
		public static Image imgTitle;

		// Token: 0x04001585 RID: 5509
		public int plX;

		// Token: 0x04001586 RID: 5510
		public int plY;

		// Token: 0x04001587 RID: 5511
		public int lY;

		// Token: 0x04001588 RID: 5512
		public int lX;

		// Token: 0x04001589 RID: 5513
		public int logoDes;

		// Token: 0x0400158A RID: 5514
		public int lineX;

		// Token: 0x0400158B RID: 5515
		public int lineY;

		// Token: 0x0400158C RID: 5516
		public static int[] bgId = new int[] { 0, 8, 2, 6, 9 };

		// Token: 0x0400158D RID: 5517
		public static bool isTryGetIPFromWap;

		// Token: 0x0400158E RID: 5518
		public static short timeLogin;

		// Token: 0x0400158F RID: 5519
		public static long lastTimeLogin;

		// Token: 0x04001590 RID: 5520
		public static long currTimeLogin;

		// Token: 0x04001591 RID: 5521
		private int yt;

		// Token: 0x04001592 RID: 5522
		private Command cmdSelect;

		// Token: 0x04001593 RID: 5523
		private Command cmdOK;

		// Token: 0x04001594 RID: 5524
		private int xLog;

		// Token: 0x04001595 RID: 5525
		private int yLog;

		// Token: 0x04001596 RID: 5526
		private int xP;

		// Token: 0x04001597 RID: 5527
		private int yP;

		// Token: 0x04001598 RID: 5528
		private int wP;

		// Token: 0x04001599 RID: 5529
		private int hP;

		// Token: 0x0400159A RID: 5530
		private string passRe = string.Empty;

		// Token: 0x0400159B RID: 5531
		public bool isFAQ;

		// Token: 0x0400159C RID: 5532
		private int tipid = -1;

		// Token: 0x0400159D RID: 5533
		public bool isLogin2;

		// Token: 0x0400159E RID: 5534
		private int v = 2;

		// Token: 0x0400159F RID: 5535
		private int g;

		// Token: 0x040015A0 RID: 5536
		private int ylogo = -40;

		// Token: 0x040015A1 RID: 5537
		private int dir = 1;

		// Token: 0x040015A2 RID: 5538
		public static bool isLoggingIn;
	}
}
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.CodeAnalysis
{
	// Token: 0x02000002 RID: 2
	[CompilerGenerated]
	[Embedded]
	internal sealed class EmbeddedAttribute : Attribute
	{
	}
}
using System;
using System.Collections.Generic;

namespace Mod.DungPham.KoiOctiiu957
{
	// Token: 0x020000F0 RID: 240
	public class AutoSkill : IActionListener, IChatable
	{
		// Token: 0x06000BE5 RID: 3045 RVA: 0x000BA054 File Offset: 0x000B8254
		public static AutoSkill getInstance()
		{
			bool flag = AutoSkill._Instance == null;
			if (flag)
			{
				AutoSkill._Instance = new AutoSkill();
			}
			return AutoSkill._Instance;
		}

		// Token: 0x06000BE6 RID: 3046 RVA: 0x000BA084 File Offset: 0x000B8284
		public static void LienHoan()
		{
			try
			{
				for (int i = 0; i < GameScr.onScreenSkill.Length; i++)
				{
					Skill skill = GameScr.onScreenSkill[i];
					bool flag = skill != null && skill.template.id == 17 && AutoSkill.delayUseskill[i] < 1L;
					if (flag)
					{
						GameScr.gI().doSelectSkill(skill, true);
						AutoSkill.AutoSendAttack();
					}
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000BE7 RID: 3047 RVA: 0x00007DEC File Offset: 0x00005FEC
		private static void TeleportTo(int x, int y)
		{
			global::Char.myCharz().cx = x;
			global::Char.myCharz().cy = y;
			Service.gI().charMove();
			global::Char.myCharz().cx = x;
			global::Char.myCharz().cy = y + 1;
			Service.gI().charMove();
			global::Char.myCharz().cx = x;
			global::Char.myCharz().cy = y;
			Service.gI().charMove();
		}

		// Token: 0x06000BE8 RID: 3048 RVA: 0x000BA104 File Offset: 0x000B8304
		public static void Update()
		{
			bool flag = AutoSkill.isAutoSendAttack;
			if (flag)
			{
				AutoSkill.AutoSendAttack();
			}
			bool flag2 = AutoSkill.isTrainPet;
			if (flag2)
			{
				AutoSkill.AutoSkillForPet();
			}
			bool flag3 = !global::Char.myCharz().meDead;
			if (flag3)
			{
				for (int i = 0; i < GameScr.keySkill.Length; i++)
				{
					bool flag4 = AutoSkill.isAutoUseSkills[i];
					if (flag4)
					{
						AutoSkill.AutoUseSkill(i);
					}
				}
			}
			bool flag5 = AutoSkill.isLoadKeySkill && GameCanvas.gameTick % 20 == 0;
			if (flag5)
			{
				AutoSkill.isLoadKeySkill = false;
				AutoSkill.LoadKeySkills();
			}
			bool flag6 = AutoSkill.isAutoChangeFocus;
			if (flag6)
			{
				AutoSkill.AutoChangeFocus();
			}
		}

		// Token: 0x06000BE9 RID: 3049 RVA: 0x000BA1B4 File Offset: 0x000B83B4
		public void onChatFromMe(string text, string to)
		{
			bool flag = ChatTextField.gI().tfChat.getText() != null && !ChatTextField.gI().tfChat.getText().Equals(string.Empty) && !text.Equals(string.Empty) && text != null;
			if (flag)
			{
				bool flag2 = ChatTextField.gI().strChat.Equals(AutoSkill.inputDelay[0]);
				if (flag2)
				{
					try
					{
						long num = long.Parse(ChatTextField.gI().tfChat.getText());
						AutoSkill.timeAutoSkills[AutoSkill.indexSkillAuto] = num;
						AutoSkill.isAutoUseSkills[AutoSkill.indexSkillAuto] = true;
						GameScr.info1.addInfo(string.Concat(new string[]
						{
							"Auto ",
							GameScr.keySkill[AutoSkill.indexSkillAuto].template.name,
							": ",
							NinjaUtil.getMoneys(num),
							" mili giây"
						}), 0);
					}
					catch
					{
						GameScr.info1.addInfo("Vui Lòng Nhập Lại Delay!", 0);
					}
					AutoSkill.ResetChatTextField();
				}
			}
			else
			{
				ChatTextField.gI().isShow = false;
			}
		}

		// Token: 0x06000BEA RID: 3050 RVA: 0x00003E4C File Offset: 0x0000204C
		public void onCancelChat()
		{
		}

		// Token: 0x06000BEB RID: 3051 RVA: 0x000BA2E8 File Offset: 0x000B84E8
		public void perform(int idAction, object p)
		{
			switch (idAction)
			{
			case 1:
			{
				AutoSkill.isAutoSendAttack = !AutoSkill.isAutoSendAttack;
				GameScr.info1.addInfo("Tự Đánh\n" + (AutoSkill.isAutoSendAttack ? "[STATUS: ON]" : "[STATUS: OFF]"), 0);
				bool flag = AutoSkill.isAutoSendAttack;
				if (flag)
				{
					AutoSkill.isAutoChangeFocus = false;
				}
				break;
			}
			case 2:
				AutoSkill.isTrainPet = !AutoSkill.isTrainPet;
				GameScr.info1.addInfo("Đánh Khi Đệ Cần\n" + (AutoSkill.isTrainPet ? "[STATUS: ON]" : "[STATUS: OFF]"), 0);
				break;
			case 3:
			{
				MyVector myVector = new MyVector();
				for (int i = 0; i < GameScr.keySkill.Length; i++)
				{
					myVector.addElement(new Command(((GameScr.keySkill[i] != null) ? GameScr.keySkill[i].template.name : "null") + "\n[" + (i + 1).ToString() + "]\n", AutoSkill.getInstance(), 10, i));
				}
				GameCanvas.menu.startAt(myVector, 3);
				break;
			}
			case 4:
				AutoSkill.isAutoShield = !AutoSkill.isAutoShield;
				GameScr.info1.addInfo("Auto Khiên Pro\n" + (AutoSkill.isAutoShield ? "[STATUS: ON]" : "[STATUS: OFF]"), 0);
				break;
			case 5:
				GameScr.info1.addInfo("Không Hỗ Trợ Lưu Cài Đặt Ở Mục Này!", 0);
				break;
			case 6:
			{
				AutoSkill.isAutoChangeFocus = !AutoSkill.isAutoChangeFocus;
				GameScr.info1.addInfo("Đánh Chuyển Mục Tiêu\n" + (AutoSkill.isAutoChangeFocus ? "[STATUS: ON]" : "[STATUS: OFF]"), 0);
				bool flag2 = AutoSkill.isAutoChangeFocus;
				if (flag2)
				{
					AutoSkill.isAutoSendAttack = false;
				}
				break;
			}
			case 7:
				AutoSkill.listTargetAutoChangeFocus.Clear();
				GameScr.info1.addInfo("Đã Xóa Danh Sách Đánh Chuyển Mục Tiêu!", 0);
				break;
			case 8:
			{
				bool flag3 = global::Char.myCharz().charFocus != null;
				if (flag3)
				{
					AutoSkill.listTargetAutoChangeFocus.Remove(global::Char.myCharz().charFocus);
					GameScr.info1.addInfo(string.Concat(new string[]
					{
						"Đã Xóa ",
						global::Char.myCharz().charFocus.cName,
						" [",
						global::Char.myCharz().charFocus.charID.ToString(),
						"]"
					}), 0);
				}
				break;
			}
			case 9:
			{
				bool flag4 = global::Char.myCharz().charFocus != null;
				if (flag4)
				{
					AutoSkill.listTargetAutoChangeFocus.Add(global::Char.myCharz().charFocus);
					GameScr.info1.addInfo(string.Concat(new string[]
					{
						"Đã Thêm ",
						global::Char.myCharz().charFocus.cName,
						" [",
						global::Char.myCharz().charFocus.charID.ToString(),
						"]"
					}), 0);
				}
				break;
			}
			case 10:
				AutoSkill.ShowMenuAutoSkill((int)p);
				break;
			case 11:
			{
				int num = (int)p;
				AutoSkill.isAutoUseSkills[num] = !AutoSkill.isAutoUseSkills[num];
				bool flag5 = AutoSkill.isAutoUseSkills[num];
				if (flag5)
				{
					AutoSkill.timeAutoSkills[num] = -1L;
				}
				GameScr.info1.addInfo("Auto " + GameScr.keySkill[num].template.name + (AutoSkill.isAutoUseSkills[num] ? (": " + NinjaUtil.getMoneys(AutoSkill.timeAutoSkills[num]) + " mili giây") : "\n[STATUS: OFF]"), 0);
				break;
			}
			case 12:
				ChatTextField.gI().strChat = AutoSkill.inputDelay[0];
				ChatTextField.gI().tfChat.name = AutoSkill.inputDelay[1];
				ChatTextField.gI().startChat2(AutoSkill.getInstance(), string.Empty);
				AutoSkill.indexSkillAuto = (int)p;
				break;
			case 13:
			{
				int num2 = (int)p;
				GameScr.keySkill[num2].coolDown = 0;
				GameScr.keySkill[num2].manaUse = 0;
				GameScr.info1.addInfo("Đóng Băng " + GameScr.keySkill[num2].template.name, 0);
				break;
			}
			}
		}

		// Token: 0x06000BEC RID: 3052 RVA: 0x000BA75C File Offset: 0x000B895C
		public static void ShowMenu()
		{
			AutoSkill.LoadData();
			MyVector myVector = new MyVector();
			myVector.addElement(new Command("Tự Đánh\n" + (AutoSkill.isAutoSendAttack ? "[STATUS: ON]" : "[STATUS: OFF]"), AutoSkill.getInstance(), 1, null));
			myVector.addElement(new Command("Đánh Khi Đệ Cần\n" + (AutoSkill.isTrainPet ? "[STATUS: ON]" : "[STATUS: OFF]"), AutoSkill.getInstance(), 2, null));
			myVector.addElement(new Command(GameScr.keySkill.Length.ToString() + " Ô Kỹ Năng", AutoSkill.getInstance(), 3, null));
			myVector.addElement(new Command("Lưu Cài Đặt\n" + (AutoSkill.isSaveData ? "[STATUS: ON]" : "[STATUS: OFF]"), AutoSkill.getInstance(), 5, null));
			myVector.addElement(new Command("Đánh Chuyển Mục Tiêu\n" + (AutoSkill.isAutoChangeFocus ? "[STATUS: ON]" : "[STATUS: OFF]"), AutoSkill.getInstance(), 6, null));
			bool flag = AutoSkill.listTargetAutoChangeFocus.Count > 0;
			if (flag)
			{
				myVector.addElement(new Command("Clear Danh Sách Chuyển Mục Tiêu", AutoSkill.getInstance(), 7, null));
			}
			bool flag2 = global::Char.myCharz().charFocus != null;
			if (flag2)
			{
				bool flag3 = AutoSkill.listTargetAutoChangeFocus.Contains(global::Char.myCharz().charFocus);
				if (flag3)
				{
					myVector.addElement(new Command("Xóa Khỏi Danh Sách Chuyển Mục Tiêu", AutoSkill.getInstance(), 8, null));
				}
				else
				{
					myVector.addElement(new Command("Thêm Vào Danh Sách Chuyển Mục Tiêu", AutoSkill.getInstance(), 9, null));
				}
			}
			GameCanvas.menu.startAt(myVector, 3);
		}

		// Token: 0x06000BED RID: 3053 RVA: 0x000BA900 File Offset: 0x000B8B00
		private static void ShowMenuAutoSkill(int skillIndex)
		{
			MyVector myVector = new MyVector();
			myVector.addElement(new Command("Auto Sử Dụng\n" + (AutoSkill.isAutoUseSkills[skillIndex] ? ("[" + NinjaUtil.getMoneys(AutoSkill.timeAutoSkills[skillIndex]) + " mili giây]") : "[STATUS: OFF]"), AutoSkill.getInstance(), 11, skillIndex));
			myVector.addElement(new Command("Nhập Delay\n[mili giây]", AutoSkill.getInstance(), 12, skillIndex));
			myVector.addElement(new Command("Đóng Băng\n" + GameScr.keySkill[skillIndex].template.name, AutoSkill.getInstance(), 13, skillIndex));
			GameCanvas.menu.startAt(myVector, 3);
		}

		// Token: 0x06000BEE RID: 3054 RVA: 0x00006B1F File Offset: 0x00004D1F
		private static void ResetChatTextField()
		{
			ChatTextField.gI().strChat = "Chat";
			ChatTextField.gI().tfChat.name = "chat";
			ChatTextField.gI().isShow = false;
		}

		// Token: 0x06000BEF RID: 3055 RVA: 0x00003E4C File Offset: 0x0000204C
		private static void LoadData()
		{
		}

		// Token: 0x06000BF0 RID: 3056 RVA: 0x00003E4C File Offset: 0x0000204C
		private static void smethod_6()
		{
		}

		// Token: 0x06000BF1 RID: 3057 RVA: 0x000BA9C4 File Offset: 0x000B8BC4
		private static void LoadKeySkills()
		{
			for (int i = 0; i < global::Char.myCharz().nClass.skillTemplates.Length; i++)
			{
				SkillTemplate skillTemplate = global::Char.myCharz().nClass.skillTemplates[i];
				Skill skill = global::Char.myCharz().getSkill(skillTemplate);
				bool flag = skill != null;
				if (flag)
				{
					GameScr.keySkill[i] = skill;
				}
				bool flag2 = skill.template.id == 17;
				if (flag2)
				{
					GameScr.keySkill[0] = skill;
				}
				GameScr.gI().saveKeySkillToRMS();
			}
		}

		// Token: 0x06000BF2 RID: 3058 RVA: 0x000BAA50 File Offset: 0x000B8C50
		public static void AutoSendAttack()
		{
			bool flag = global::Char.myCharz().meDead || global::Char.myCharz().cHP <= 0L || global::Char.myCharz().statusMe == 14 || global::Char.myCharz().statusMe == 5 || global::Char.myCharz().myskill.template.type == 3 || global::Char.myCharz().myskill.template.id == 10 || global::Char.myCharz().myskill.template.id == 11 || (global::Char.myCharz().myskill.paintCanNotUseSkill && !GameCanvas.panel.isShow);
			if (!flag)
			{
				int mySkillIndex = AutoSkill.GetMySkillIndex();
				bool flag2 = mSystem.currentTimeMillis() - AutoSkill.lastTimeSendAttack[mySkillIndex] > AutoSkill.GetCoolDown(global::Char.myCharz().myskill);
				if (flag2)
				{
					bool flag3 = GameScr.gI().isMeCanAttackMob(global::Char.myCharz().mobFocus);
					if (flag3)
					{
						global::Char.myCharz().myskill.lastTimeUseThisSkill = mSystem.currentTimeMillis();
						AutoSkill.SendAttackToMobFocus();
						AutoSkill.lastTimeSendAttack[mySkillIndex] = mSystem.currentTimeMillis();
					}
					else
					{
						bool flag4 = global::Char.myCharz().charFocus != null && AutoSkill.isMeCanAttackChar(global::Char.myCharz().charFocus) && (double)global::Math.abs(global::Char.myCharz().charFocus.cx - global::Char.myCharz().cx) < (double)global::Char.myCharz().myskill.dx * 1.7;
						if (flag4)
						{
							global::Char.myCharz().myskill.lastTimeUseThisSkill = mSystem.currentTimeMillis();
							AutoSkill.SendAttackToCharFocus();
							AutoSkill.lastTimeSendAttack[mySkillIndex] = mSystem.currentTimeMillis();
						}
					}
				}
			}
		}

		// Token: 0x06000BF3 RID: 3059 RVA: 0x000BAC0C File Offset: 0x000B8E0C
		private static void AutoSkillForPet()
		{
			bool flag = !AutoSkill.isPetAskedForUseSkill || GameScr.vMob.size() == 0 || global::Char.myCharz().myskill.template.type == 3;
			if (!flag)
			{
				Mob mob = (Mob)GameScr.vMob.elementAt(0);
				int num = 0;
				for (int i = 0; i < GameScr.vMob.size(); i++)
				{
					Mob mob2 = (Mob)GameScr.vMob.elementAt(i);
					int num2 = global::Math.abs(global::Char.myCharz().cx - mob2.x);
					int num3 = global::Math.abs(global::Char.myCharz().cy - mob2.y);
					int num4 = num2 * num2 + num3 * num3;
					bool flag2 = num < num4;
					if (flag2)
					{
						num = num4;
						mob = mob2;
					}
				}
				global::Char.myCharz().mobFocus = mob;
				int mySkillIndex = AutoSkill.GetMySkillIndex();
				bool flag3 = mSystem.currentTimeMillis() - AutoSkill.lastTimeSendAttack[mySkillIndex] > (long)(global::Char.myCharz().myskill.coolDown + 100) && GameScr.gI().isMeCanAttackMob(global::Char.myCharz().mobFocus);
				if (flag3)
				{
					AutoSkill.SendAttackToMobFocus();
					AutoSkill.lastTimeSendAttack[mySkillIndex] = mSystem.currentTimeMillis();
					AutoSkill.isPetAskedForUseSkill = false;
				}
			}
		}

		// Token: 0x06000BF4 RID: 3060 RVA: 0x000BAD58 File Offset: 0x000B8F58
		private static void AutoUseSkill(int skillIndex)
		{
			bool flag = TileMap.mapID == 21 || TileMap.mapID == 22 || TileMap.mapID == 23;
			if (!flag)
			{
				bool flag2 = skillIndex >= GameScr.keySkill.Length;
				if (flag2)
				{
					skillIndex = GameScr.keySkill.Length - 1;
				}
				bool flag3 = skillIndex < 0;
				if (flag3)
				{
					skillIndex = 0;
				}
				bool flag4 = GameScr.keySkill[skillIndex] == null || GameScr.keySkill[skillIndex].paintCanNotUseSkill;
				if (!flag4)
				{
					bool flag5 = GameScr.keySkill[skillIndex].coolDown == 0;
					if (flag5)
					{
						AutoSkill.timeAutoSkills[skillIndex] = 500L;
					}
					else
					{
						bool flag6 = AutoSkill.isMeHasEnoughMP(GameScr.keySkill[skillIndex]) && !GameScr.gI().isCharging() && mSystem.currentTimeMillis() - AutoSkill.lastTimeAutoUseSkill > 150L;
						if (flag6)
						{
							bool flag7 = AutoSkill.timeAutoSkills[skillIndex] == -1L && GameCanvas.gameTick % 20 == 0;
							if (flag7)
							{
								AutoSkill.lastTimeUseSkill[skillIndex] = mSystem.currentTimeMillis();
								AutoSkill.lastTimeAutoUseSkill = mSystem.currentTimeMillis();
								GameScr.gI().doSelectSkill(GameScr.keySkill[skillIndex], true);
							}
							bool flag8 = mSystem.currentTimeMillis() - AutoSkill.lastTimeUseSkill[skillIndex] > AutoSkill.timeAutoSkills[skillIndex];
							if (flag8)
							{
								AutoSkill.lastTimeUseSkill[skillIndex] = mSystem.currentTimeMillis();
								AutoSkill.lastTimeAutoUseSkill = mSystem.currentTimeMillis();
								GameScr.gI().doSelectSkill(GameScr.keySkill[skillIndex], true);
							}
						}
					}
				}
			}
		}

		// Token: 0x06000BF5 RID: 3061 RVA: 0x00008200 File Offset: 0x00006400
		public static bool isMeCanAttackChar(global::Char ch)
		{
			bool flag = TileMap.mapID == 113;
			bool flag4;
			if (flag)
			{
				bool flag2 = ch != null && global::Char.myCharz().myskill != null;
				if (flag2)
				{
					bool flag3 = ch.cTypePk != 5;
					flag4 = !flag3 || ch.cTypePk == 3;
				}
				else
				{
					flag4 = false;
				}
			}
			else
			{
				bool flag5 = ch != null && global::Char.myCharz().myskill != null;
				if (flag5)
				{
					bool flag6 = ch.statusMe != 14 && ch.statusMe != 5 && global::Char.myCharz().myskill.template.type != 2 && ((global::Char.myCharz().cFlag == 8 && ch.cFlag != 0) || (global::Char.myCharz().cFlag != 0 && ch.cFlag == 8) || (global::Char.myCharz().cFlag != ch.cFlag && global::Char.myCharz().cFlag != 0 && ch.cFlag != 0) || (ch.cTypePk == 3 && global::Char.myCharz().cTypePk == 3) || global::Char.myCharz().cTypePk == 5 || ch.cTypePk == 5 || (global::Char.myCharz().cTypePk == 1 && ch.cTypePk == 1) || (global::Char.myCharz().cTypePk == 4 && ch.cTypePk == 4));
					if (flag6)
					{
						flag4 = true;
					}
					else
					{
						bool flag7 = global::Char.myCharz().myskill.template.type == 2;
						flag4 = flag7 && ch.cTypePk != 5;
					}
				}
				else
				{
					flag4 = false;
				}
			}
			return flag4;
		}

		// Token: 0x06000BF6 RID: 3062 RVA: 0x00008CCC File Offset: 0x00006ECC
		private static bool isMeHasEnoughMP(Skill skillToUse)
		{
			bool flag = skillToUse.template.manaUseType == 2;
			bool flag2;
			if (flag)
			{
				flag2 = true;
			}
			else
			{
				bool flag3 = skillToUse.template.manaUseType != 1;
				if (flag3)
				{
					flag2 = global::Char.myCharz().cMP >= (long)skillToUse.manaUse;
				}
				else
				{
					flag2 = global::Char.myCharz().cMP >= (long)skillToUse.manaUse * global::Char.myCharz().cMPFull / 100L;
				}
			}
			return flag2;
		}

		// Token: 0x06000BF7 RID: 3063 RVA: 0x0000808C File Offset: 0x0000628C
		private static void SendAttackToCharFocus()
		{
			try
			{
				MyVector myVector = new MyVector();
				myVector.addElement(global::Char.myCharz().charFocus);
				Service.gI().sendPlayerAttack(new MyVector(), myVector, 2);
			}
			catch
			{
			}
		}

		// Token: 0x06000BF8 RID: 3064 RVA: 0x000080DC File Offset: 0x000062DC
		private static void SendAttackToMobFocus()
		{
			try
			{
				MyVector myVector = new MyVector();
				myVector.addElement(global::Char.myCharz().mobFocus);
				Service.gI().sendPlayerAttack(myVector, new MyVector(), 1);
			}
			catch
			{
			}
		}

		// Token: 0x06000BF9 RID: 3065 RVA: 0x0000812C File Offset: 0x0000632C
		private static long GetCoolDown(Skill skill)
		{
			bool flag = skill.template.id != 20 && skill.template.id != 22 && skill.template.id != 7 && skill.template.id != 18 && skill.template.id != 23;
			long num;
			if (flag)
			{
				num = (long)(skill.coolDown + 100);
			}
			else
			{
				num = (long)skill.coolDown + 500L;
			}
			return num;
		}

		// Token: 0x06000BFA RID: 3066 RVA: 0x000081B0 File Offset: 0x000063B0
		private static int GetMySkillIndex()
		{
			int num = 0;
			for (;;)
			{
				bool flag = num < GameScr.keySkill.Length;
				if (!flag)
				{
					goto IL_0035;
				}
				bool flag2 = GameScr.keySkill[num] == global::Char.myCharz().myskill;
				if (flag2)
				{
					break;
				}
				num++;
			}
			return num;
			IL_0035:
			return 0;
		}

		// Token: 0x06000BFB RID: 3067 RVA: 0x000BAED0 File Offset: 0x000B90D0
		private static void AutoChangeFocus()
		{
			for (int i = 0; i < GameScr.vCharInMap.size(); i++)
			{
				global::Char @char = (global::Char)GameScr.vCharInMap.elementAt(i);
				bool flag = @char.cTypePk == 5 && @char.cx - global::Char.myCharz().cx <= 5 && @char.cy - global::Char.myCharz().cy <= 5;
				if (flag)
				{
					AutoSkill.listTargetAutoChangeFocus.Add(@char);
				}
			}
			bool flag2 = AutoSkill.listTargetAutoChangeFocus.Count == 0;
			if (flag2)
			{
				GameScr.info1.addInfo("Danh sách chuyển mục tiêu trống!", 0);
				AutoSkill.isAutoChangeFocus = false;
			}
			else
			{
				bool flag3 = global::Char.myCharz().meDead || global::Char.myCharz().statusMe == 14 || global::Char.myCharz().statusMe == 5 || global::Char.myCharz().myskill.template.type == 3 || global::Char.myCharz().myskill.template.id == 10 || global::Char.myCharz().myskill.template.id == 11 || global::Char.myCharz().myskill.paintCanNotUseSkill;
				if (!flag3)
				{
					AutoSkill.cooldownAutoChangeFocus = AutoSkill.GetCooldownAutoChangeFocus(global::Char.myCharz().myskill);
					bool flag4 = AutoSkill.targetIndex >= AutoSkill.listTargetAutoChangeFocus.Count;
					if (flag4)
					{
						AutoSkill.targetIndex = 0;
					}
					bool flag5 = mSystem.currentTimeMillis() - AutoSkill.lastTimeChangeFocus >= AutoSkill.cooldownAutoChangeFocus;
					if (flag5)
					{
						AutoSkill.lastTimeChangeFocus = mSystem.currentTimeMillis();
						global::Char.myCharz().charFocus = GameScr.findCharInMap(AutoSkill.listTargetAutoChangeFocus[AutoSkill.targetIndex].charID);
						AutoSkill.targetIndex++;
						bool flag6 = AutoSkill.targetIndex >= AutoSkill.listTargetAutoChangeFocus.Count;
						if (flag6)
						{
							AutoSkill.targetIndex = 0;
						}
						bool flag7 = global::Char.myCharz().charFocus != null && AutoSkill.isMeCanAttackChar(global::Char.myCharz().charFocus) && (double)global::Math.abs(global::Char.myCharz().charFocus.cx - global::Char.myCharz().cx) < (double)global::Char.myCharz().myskill.dx * 1.5;
						if (flag7)
						{
							global::Char.myCharz().myskill.lastTimeUseThisSkill = mSystem.currentTimeMillis();
							AutoSkill.SendAttackToCharFocus();
						}
					}
				}
			}
		}

		// Token: 0x06000BFC RID: 3068 RVA: 0x00007E60 File Offset: 0x00006060
		private static long GetCooldownAutoChangeFocus(Skill skill)
		{
			bool flag = skill.coolDown <= 500;
			long num;
			if (flag)
			{
				num = 1000L;
			}
			else
			{
				num = (long)((double)skill.coolDown * 1.2 + 200.0);
			}
			return num;
		}

		// Token: 0x06000BFD RID: 3069 RVA: 0x00003E4C File Offset: 0x0000204C
		private static void smethod_0()
		{
		}

		// Token: 0x06000BFE RID: 3070 RVA: 0x000BB144 File Offset: 0x000B9344
		static AutoSkill()
		{
			AutoSkill.LoadData();
		}

		// Token: 0x06000BFF RID: 3071 RVA: 0x000BB1C0 File Offset: 0x000B93C0
		public static void FreezeSelectedSkill()
		{
			int mySkillIndex = AutoSkill.GetMySkillIndex();
			GameScr.keySkill[mySkillIndex].coolDown = 0;
			GameScr.keySkill[mySkillIndex].manaUse = 0;
			GameScr.info1.addInfo("Đóng Băng\n" + GameScr.keySkill[mySkillIndex].template.name, 0);
		}

		// Token: 0x040014C4 RID: 5316
		private static AutoSkill _Instance;

		// Token: 0x040014C5 RID: 5317
		public static bool isLoadKeySkill = true;

		// Token: 0x040014C6 RID: 5318
		public static bool isAutoSendAttack;

		// Token: 0x040014C7 RID: 5319
		private static long[] lastTimeSendAttack = new long[10];

		// Token: 0x040014C8 RID: 5320
		public static bool isTrainPet;

		// Token: 0x040014C9 RID: 5321
		public static bool isPetAskedForUseSkill;

		// Token: 0x040014CA RID: 5322
		public static bool[] isAutoUseSkills = new bool[10];

		// Token: 0x040014CB RID: 5323
		private static long[] lastTimeUseSkill = new long[10];

		// Token: 0x040014CC RID: 5324
		private static long[] timeAutoSkills = new long[10];

		// Token: 0x040014CD RID: 5325
		private static int indexSkillAuto;

		// Token: 0x040014CE RID: 5326
		private static bool isAutoChangeFocus;

		// Token: 0x040014CF RID: 5327
		private static long cooldownAutoChangeFocus;

		// Token: 0x040014D0 RID: 5328
		private static long lastTimeChangeFocus;

		// Token: 0x040014D1 RID: 5329
		private static List<global::Char> listTargetAutoChangeFocus = new List<global::Char>();

		// Token: 0x040014D2 RID: 5330
		private static int targetIndex;

		// Token: 0x040014D3 RID: 5331
		private static bool isAutoShield;

		// Token: 0x040014D4 RID: 5332
		private static string[] inputDelay = new string[] { "Nhập delay", "mili giây" };

		// Token: 0x040014D5 RID: 5333
		private static bool isSaveData;

		// Token: 0x040014D6 RID: 5334
		private static long lastTimeAutoUseSkill;

		// Token: 0x040014D7 RID: 5335
		public static long[] delayUseskill = new long[10];
	}
}
using System;

namespace Mod.DungPham.KoiOctiiu957
{
	// Token: 0x020000F1 RID: 241
	public class Boss
	{
		// Token: 0x06000C01 RID: 3073 RVA: 0x0000523A File Offset: 0x0000343A
		public Boss()
		{
		}

		// Token: 0x06000C02 RID: 3074 RVA: 0x000BB218 File Offset: 0x000B9418
		public Boss(string chatVip)
		{
			chatVip = chatVip.Replace("BOSS ", "").Replace(" vừa xuất hiện tại ", "|").Replace(" appear at ", "|");
			string[] array = chatVip.Split(new char[] { '|' });
			this.NameBoss = array[0].Trim();
			this.MapName = array[1].Trim();
			this.MapId = this.GetMapID(this.MapName);
			this.AppearTime = DateTime.Now;
		}

		// Token: 0x06000C03 RID: 3075 RVA: 0x000BB2A8 File Offset: 0x000B94A8
		public int GetMapID(string mapName)
		{
			int num = 0;
			for (;;)
			{
				bool flag = num < TileMap.mapNames.Length;
				if (!flag)
				{
					goto IL_002F;
				}
				bool flag2 = TileMap.mapNames[num].Equals(mapName);
				if (flag2)
				{
					break;
				}
				num++;
			}
			return num;
			IL_002F:
			return -1;
		}

		// Token: 0x06000C04 RID: 3076 RVA: 0x000BB2F4 File Offset: 0x000B94F4
		public void Paint(mGraphics g, int x, int y, int align)
		{
			TimeSpan timeSpan = DateTime.Now.Subtract(this.AppearTime);
			int num = (int)timeSpan.TotalSeconds;
			mFont mFont = mFont.tahoma_7_yellow;
			bool flag = TileMap.mapID == this.MapId;
			if (flag)
			{
				mFont = mFont.tahoma_7_red;
				for (int i = 0; i < GameScr.vCharInMap.size(); i++)
				{
					bool flag2 = ((global::Char)GameScr.vCharInMap.elementAt(i)).cName.Equals(this.NameBoss);
					if (flag2)
					{
						mFont = mFont.tahoma_7b_red;
						break;
					}
				}
			}
			mFont.drawString(g, string.Concat(new string[]
			{
				this.NameBoss,
				" - ",
				this.MapName,
				" - ",
				(num < 60) ? (num.ToString() + "s") : (timeSpan.Minutes.ToString() + "ph"),
				" trước"
			}), x, y, align);
		}

		// Token: 0x040014D8 RID: 5336
		public string NameBoss;

		// Token: 0x040014D9 RID: 5337
		public string MapName;

		// Token: 0x040014DA RID: 5338
		public int MapId;

		// Token: 0x040014DB RID: 5339
		public DateTime AppearTime;
	}
}
using System;

namespace Mod.DungPham.KoiOctiiu957
{
	// Token: 0x020000F2 RID: 242
	public class Hotkeys
	{
		// Token: 0x040014DC RID: 5340
		public static int A = 97;

		// Token: 0x040014DD RID: 5341
		public static int B = 98;

		// Token: 0x040014DE RID: 5342
		public static int C = 99;

		// Token: 0x040014DF RID: 5343
		public static int D = 100;

		// Token: 0x040014E0 RID: 5344
		public static int E = 101;

		// Token: 0x040014E1 RID: 5345
		public static int F = 102;

		// Token: 0x040014E2 RID: 5346
		public static int G = 103;

		// Token: 0x040014E3 RID: 5347
		public static int H = 104;

		// Token: 0x040014E4 RID: 5348
		public static int I = 105;

		// Token: 0x040014E5 RID: 5349
		public static int J = 106;

		// Token: 0x040014E6 RID: 5350
		public static int K = 107;

		// Token: 0x040014E7 RID: 5351
		public static int L = 108;

		// Token: 0x040014E8 RID: 5352
		public static int M = 109;

		// Token: 0x040014E9 RID: 5353
		public static int N = 110;

		// Token: 0x040014EA RID: 5354
		public static int O = 111;

		// Token: 0x040014EB RID: 5355
		public static int P = 112;

		// Token: 0x040014EC RID: 5356
		public static int Q = 113;

		// Token: 0x040014ED RID: 5357
		public static int R = 114;

		// Token: 0x040014EE RID: 5358
		public static int S = 115;

		// Token: 0x040014EF RID: 5359
		public static int T = 116;

		// Token: 0x040014F0 RID: 5360
		public static int U = 117;

		// Token: 0x040014F1 RID: 5361
		public static int V = 118;

		// Token: 0x040014F2 RID: 5362
		public static int W = 119;

		// Token: 0x040014F3 RID: 5363
		public static int X = 120;

		// Token: 0x040014F4 RID: 5364
		public static int Y = 121;

		// Token: 0x040014F5 RID: 5365
		public static int Z = 122;
	}
}
using System;
using Microsoft.CodeAnalysis;

namespace System.Runtime.CompilerServices
{
	// Token: 0x02000003 RID: 3
	[CompilerGenerated]
	[Embedded]
	[AttributeUsage(AttributeTargets.Module, AllowMultiple = false, Inherited = false)]
	internal sealed class RefSafetyRulesAttribute : Attribute
	{
		// Token: 0x06000002 RID: 2 RVA: 0x00003DAD File Offset: 0x00001FAD
		public RefSafetyRulesAttribute(int A_1)
		{
			this.Version = A_1;
		}

		// Token: 0x04000001 RID: 1
		public readonly int Version;
	}
}
using System;

// Token: 0x020000EF RID: 239
public class Waypoint : IActionListener
{
	// Token: 0x06000BE3 RID: 3043 RVA: 0x000B9C3C File Offset: 0x000B7E3C
	public Waypoint(short minX, short minY, short maxX, short maxY, bool isEnter, bool isOffline, string name)
	{
		this.minX = minX;
		this.minY = minY;
		this.maxX = maxX;
		this.maxY = maxY;
		name = Res.changeString(name);
		this.isEnter = isEnter;
		this.isOffline = isOffline;
		bool flag = ((TileMap.mapID == 21 || TileMap.mapID == 22 || TileMap.mapID == 23) && this.minX >= 0 && this.minX <= 24) || (((TileMap.mapID == 0 && global::Char.myCharz().cgender != 0) || (TileMap.mapID == 7 && global::Char.myCharz().cgender != 1) || (TileMap.mapID == 14 && global::Char.myCharz().cgender != 2)) && isOffline);
		if (!flag)
		{
			bool flag2 = TileMap.isInAirMap() || TileMap.mapID == 47;
			if (flag2)
			{
				bool flag3 = minY <= 150 || !TileMap.isInAirMap();
				if (flag3)
				{
					this.popup = new PopUp(name, (int)(minX + (maxX - minX) / 2), (int)(maxY - ((minX <= 100) ? 48 : 24)));
					this.popup.command = new Command(null, this, 1, this);
					this.popup.isWayPoint = true;
					this.popup.isPaint = false;
					PopUp.addPopUp(this.popup);
					TileMap.vGo.addElement(this);
				}
			}
			else
			{
				bool flag4 = !isEnter && !isOffline;
				if (flag4)
				{
					this.popup = new PopUp(name, (int)minX, (int)(minY - 24));
					this.popup.command = new Command(null, this, 1, this);
					this.popup.isWayPoint = true;
					this.popup.isPaint = false;
					PopUp.addPopUp(this.popup);
				}
				else
				{
					bool flag5 = TileMap.isTrainingMap();
					if (flag5)
					{
						this.popup = new PopUp(name, (int)minX, (int)(minY - 16));
					}
					else
					{
						int num = (int)(minX + (maxX - minX) / 2);
						this.popup = new PopUp(name, num, (int)(minY - ((minY == 0) ? -32 : 16)));
					}
					this.popup.command = new Command(null, this, 2, this);
					this.popup.isWayPoint = true;
					this.popup.isPaint = false;
					PopUp.addPopUp(this.popup);
				}
				TileMap.vGo.addElement(this);
			}
		}
	}

	// Token: 0x06000BE4 RID: 3044 RVA: 0x000B9E98 File Offset: 0x000B8098
	public void perform(int idAction, object p)
	{
		if (idAction != 1)
		{
			if (idAction == 2)
			{
				GameScr.gI().auto = 0;
				bool flag = global::Char.myCharz().isInEnterOfflinePoint() != null;
				if (flag)
				{
					Service.gI().charMove();
					InfoDlg.showWait();
					Service.gI().getMapOffline();
					global::Char.ischangingMap = true;
				}
				else
				{
					bool flag2 = global::Char.myCharz().isInEnterOnlinePoint() != null;
					if (flag2)
					{
						Service.gI().charMove();
						Service.gI().requestChangeMap();
						global::Char.isLockKey = true;
						global::Char.ischangingMap = true;
						GameCanvas.clearKeyHold();
						GameCanvas.clearKeyPressed();
						InfoDlg.showWait();
					}
					else
					{
						int num = (int)((this.minX + this.maxX) / 2);
						int num2 = (int)this.maxY;
						global::Char.myCharz().currentMovePoint = new MovePoint(num, num2);
						global::Char.myCharz().cdir = ((global::Char.myCharz().cx - global::Char.myCharz().currentMovePoint.xEnd <= 0) ? 1 : (-1));
						global::Char.myCharz().endMovePointCommand = new Command(null, this, 2, null);
					}
				}
			}
		}
		else
		{
			int num3 = (int)((this.minX + this.maxX) / 2);
			int num4 = (int)this.maxY;
			bool flag3 = this.maxY > this.minY + 24;
			if (flag3)
			{
				num4 = (int)((this.minY + this.maxY) / 2);
			}
			GameScr.gI().auto = 0;
			global::Char.myCharz().currentMovePoint = new MovePoint(num3, num4);
			global::Char.myCharz().cdir = ((global::Char.myCharz().cx - global::Char.myCharz().currentMovePoint.xEnd <= 0) ? 1 : (-1));
			Service.gI().charMove();
		}
	}

	// Token: 0x040014BD RID: 5309
	public short minX;

	// Token: 0x040014BE RID: 5310
	public short minY;

	// Token: 0x040014BF RID: 5311
	public short maxX;

	// Token: 0x040014C0 RID: 5312
	public short maxY;

	// Token: 0x040014C1 RID: 5313
	public bool isEnter;

	// Token: 0x040014C2 RID: 5314
	public bool isOffline;

	// Token: 0x040014C3 RID: 5315
	public PopUp popup;
}
