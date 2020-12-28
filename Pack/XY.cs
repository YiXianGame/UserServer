using Make.MODEL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Make.BLL;
using Pack.MODEL;
using System.IO;
using Newtonsoft.Json;
using Pack.Element;
using System.Threading;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Make.MODEL.TCP_Async_Event;

namespace Pack
{
    //主流工作区
    public static class XY
    {
        public static void TCP_Event_Receive(Token token, Msg_Client msg_Client)
        {
            if (msg_Client.Type == Enums.Msg_Client_Type.Game) Receive_Game(token, msg_Client);
            else if (msg_Client.Type == Enums.Msg_Client_Type.Information) Receive_Information(token, msg_Client);
        }
        public static void Console_Write(object msg)
        {
            Pack_General.MainWindow.Dispatcher.Invoke((Action)delegate ()
            {
                if (Pack_General.MainWindow.Menu_Data.Data_Console.Text.Length >= 10000)
                {
                    Pack_General.MainWindow.Menu_Data.Data_Console.Text = "";
                }
                Pack_General.MainWindow.Menu_Data.Data_Console.Text += DateTime.Now + "=>" + msg.ToString() + "\n";
            });
        }
        public static void Receive_Information(Token token, Msg_Client msg_Client)
        {
            Debug.WriteLine(msg_Client.ToString());
            long long_num;
            int num;
            string[] data = msg_Client.Head.Split('#');
            
            if (data.Length > 0)
            {
                if (data.Length == 1 && data[0] == "用户登录" && msg_Client.Bound != null)
                {
                    User login_user = JsonConvert.DeserializeObject<User>(msg_Client.Bound);
                    string filepath = Material.App.directory + "\\用户\\" + login_user.UserName + ".json";

                    if (!File.Exists(filepath))
                    {
                        token.Send(Enums.Msg_Server_Type.Information, "用户登录#用户不存在");
                        return;
                    }
                    token.User = JsonConvert.DeserializeObject<User>(File.ReadAllText(filepath));
                    if (token.User.UserName.Equals(login_user.UserName) && token.User.Passwords.Equals(login_user.Passwords))
                    {
                        token.Send(Enums.Msg_Server_Type.Information, "用户登录#登录成功", File.ReadAllText(filepath));
                    }
                    else
                    {
                        token.Send( Enums.Msg_Server_Type.Information, "用户登录#密码错误");
                    }
                    return;
                }
                else if (data.Length == 1 && data[0] == "用户注册" && msg_Client.Bound != null)
                {
                    User new_user = JsonConvert.DeserializeObject<User>(msg_Client.Bound);
                    string filepath = Material.App.directory + "\\用户\\" + new_user.UserName + ".json";
                    if (!File.Exists(filepath))
                    {
                        string json = JsonConvert.SerializeObject(new_user);
                        File.WriteAllText(filepath, json);
                        token.Send(Enums.Msg_Server_Type.Information,"注册用户#注册成功",json);
                    }   
                    else
                    {
                        token.Send(Enums.Msg_Server_Type.Information,"注册用户#用户已存在");
                    }
                    return;
                }
                else if (data.Length == 2 && data[0] == "修改密码" && msg_Client.Bound != null)
                {
                    User change_user = JsonConvert.DeserializeObject<User>(msg_Client.Bound);
                    string filepath = Material.App.directory + "\\用户\\" + change_user.UserName + ".json";
                    if (!File.Exists(filepath))
                    {
                        change_user.SendMessages("修改密码#尚未注册");
                    }
                    else
                    {
                        if (data[1].Equals("12345"))
                        {
                            token.User = JsonConvert.DeserializeObject<User>(File.ReadAllText(filepath));
                            token.User.Passwords = change_user.Passwords;
                            token.User.Save();
                            token.Send(Enums.Msg_Server_Type.Information, "修改密码#修改成功");
                        }
                        else token.Send(Enums.Msg_Server_Type.Information, "修改密码#验证码错误");
                    }
                    return;
                }
                User user = UserHelper.Verify(token);
                user = new User();
                user.Token = token;
                if (user != null) token.Send(Enums.Msg_Server_Type.Information, "Token错误");
                if (data.Length == 2 && data[0] == "获取技能卡" && DateTime.Parse(data[1]) != GeneralControl.Skill_Card_Date)
                {
                    user.SendMessages("开始更新卡牌版本" + GeneralControl.Skill_Card_Date);
                    user.SendMessages("获取技能卡", JsonConvert.SerializeObject(GeneralControl.Skill_Cards));
                }
                else if (data.Length == 2 && data[0] == "获取奇遇" && DateTime.Parse(data[1]) != GeneralControl.Adventure_Date)
                {
                    user.SendMessages("开始更新奇遇版本" + GeneralControl.Adventure_Date);
                    user.SendMessages("获取奇遇" , JsonConvert.SerializeObject(GeneralControl.Adventures));
                }
                else if (data.Length == 2 && data[0] == "技能卡上传")
                {
                    if (GeneralControl.Menu_Data_Monitor_Class.Instance.Pubmit_SkillCardsModel.Count <= 100)
                    {
                        SkillCardsModel skillCardsModel = JsonConvert.DeserializeObject<SkillCardsModel>(data[1]);
                        skillCardsModel.Cloud = "云端";
                        GeneralControl.Menu_Data_Monitor_Class.Instance.Pubmit_SkillCardsModel.Add(skillCardsModel);
                        Pack_General.MainWindow.Dispatcher.Invoke((Action)delegate ()
                        {
                            Pack_General.MainWindow.Menu_Data.Pubmit_Skill.ItemsSource = null;
                            Pack_General.MainWindow.Menu_Data.Pubmit_Skill.ItemsSource = GeneralControl.Menu_Data_Monitor_Class.Instance.Pubmit_SkillCardsModel;
                        });
                    }
                    else user.SendMessages("申请已满");
                }
                else if (data.Length == 2 && data[0] == "奇遇上传")
                {
                    if (GeneralControl.Menu_Data_Monitor_Class.Instance.Pubmit_Adventures.Count <= 100)
                    {
                        Adventure adventure = JsonConvert.DeserializeObject<Adventure>(data[1]);
                        adventure.Cloud = "云端";
                        GeneralControl.Menu_Data_Monitor_Class.Instance.Pubmit_Adventures.Add(adventure);

                        Pack_General.MainWindow.Dispatcher.Invoke((Action)delegate ()
                        {
                            Pack_General.MainWindow.Menu_Data.Pubmit_Adventures.ItemsSource = null;
                            Pack_General.MainWindow.Menu_Data.Pubmit_Adventures.ItemsSource = GeneralControl.Menu_Data_Monitor_Class.Instance.Pubmit_Adventures;
                        });
                    }
                    else user.SendMessages("申请已满");
                }
                else if (data.Length == 2 && data[0] == "用户查询" && long.TryParse(data[1], out long_num))
                {
                    string filepath = Material.App.directory + "\\用户\\" + long_num.ToString() + ".json";
                    if (!File.Exists(filepath))
                    {
                        Make.MODEL.User author = new Make.MODEL.User();
                        author.Create_num = 0;
                        author.UserName = long_num.ToString();
                        author.Information = "个性签名";
                        author.NickName = "玩家名";
                        author.Upgrade_num = 0;
                        author.Save();
                        user.SendMessages("用户查询" + File.ReadAllText(filepath));
                    }
                    else
                    {
                        user.SendMessages("用户查询" + File.ReadAllText(filepath));
                    }
                }
                //[加入战备 斩杀 1] 长度:3 参一:Add_Card_To_Repository 参二:技能卡名 参三:数量 作用:加入战备
                else if (data.Length == 3 && data[0] == GeneralControl.Menu_Command_Class.Instance.Add_Card_To_Battle && int.TryParse(data[2], out num))
                {
                    int sum = 0;
                    foreach (Simple_SkillCard item in user.Battle_SkillCards.Values)
                    {
                        sum += item.Amount;
                    }
                    if (sum + num <= GeneralControl.Menu_GameControl_Class.Instance.Battle_Max)
                    {
                        if (GeneralControl.Skill_Card_Name_Skllcard.TryGetValue(data[1], out SkillCard skillCard))
                        {
                            if (user.Repository_Skill_Add(skillCard.ID, -num))
                            {
                                user.SendMessages($"{num}张{data[1]}成功从战备加入战备");
                                user.Battle_Skill_Add(skillCard.ID, num);
                            }
                            else user.SendMessages("数量不足");
                        }
                        else user.SendMessages("不存在");
                    }
                    else user.SendMessages("您的战备技能卡数量已达上限。");
                }
                //[加入库存 斩杀 1] 长度:3 参一:Add_Card_To_Repository 参二:技能卡名 参三:数量 作用:加入库存
                else if (data.Length == 3 && data[0] == GeneralControl.Menu_Command_Class.Instance.Add_Card_To_Repository && int.TryParse(data[2], out num))
                {
                    int sum = 0;
                    foreach (Simple_SkillCard item in user.Repository_SkillCards.Values)
                    {
                        sum += item.Amount;
                    }
                    if (sum + num <= GeneralControl.Menu_GameControl_Class.Instance.Repository_Max)
                    {
                        if (GeneralControl.Skill_Card_Name_Skllcard.TryGetValue(data[1], out SkillCard skillCard))
                        {
                            if (user.Battle_Skill_Add(skillCard.ID, -num))
                            {
                                user.Repository_Skill_Add(skillCard.ID, num);
                                user.SendMessages($"{num}张{data[1]}成功从战备转入库存");
                            }
                            else user.SendMessages("数量不足或不存在");
                        }
                        else user.SendMessages("不存在");
                    }
                    else user.SendMessages("您的库存技能卡数量已达上限。");
                }
                //[创建房间 单挑] 长度:2 参一:Create_Room  参二:房间类型 作用:创建房间
                else if (data.Length == 2 && data[0] == GeneralControl.Menu_Command_Class.Instance.Create_Room)
                {
                    if (user.Active == Enums.User_Active.Leisure)
                    {
                        if (data[1] == "单挑")
                        {
                            Solo_Room room_Solo = new Solo_Room(2, 2)
                            {
                                Stage = Enums.Room.Wait
                            };
                            room_Solo.Enter(user);
                            user.SendMessages("创建房间成功.房间号为[" + (GeneralControl.Rooms.IndexOf(room_Solo)).ToString() + "]");
                        }
                        else if (data[1] == "团战")
                        {
                            Team_Room room_Team = new Team_Room(6, 6)
                            {
                                Stage = Enums.Room.Wait
                            };
                            room_Team.Enter(user);
                            user.SendMessages("创建房间成功.房间号为[" + (GeneralControl.Rooms.IndexOf(room_Team)).ToString() + "]");
                        }
                    }
                    else user.SendMessages("您当前状态无法创建房间");
                }
                //[加入房间 2] 长度:2 参一:Join_Room  参二:房间号 作用:加入一个房间
                else if (data.Length == 2 && data[0] == GeneralControl.Menu_Command_Class.Instance.Join_Room && int.TryParse(data[1], out num))
                {
                    if (num >= 0 && num <= GeneralControl.Rooms.Count - 1)
                    {
                        GeneralControl.Rooms[num].Enter(user);
                        if (GeneralControl.Rooms[num].Players.Count >= GeneralControl.Rooms[num].Max_Personals) GeneralControl.Rooms[num].Start();
                    }
                    else user.SendMessages("您输入的房间号有误！");
                }
                //[加入地图] 长度:1 参一:Join_Map 作用:加入地图
                else if (data.Length == 1 && data[0] == GeneralControl.Menu_Command_Class.Instance.Join_Map)
                {
                    if (user.Active == Enums.User_Active.Leisure)
                    {
                        GeneralControl.Map.Enter(user);
                    }
                    else user.SendMessages("您当前处于非空闲状态，无法进入地图");
                }
                if (data.Length == 1 && data[0] == GeneralControl.Menu_Command_Class.Instance.GameMenu)
                {

                    user.SendMessages("游戏菜单",JsonConvert.SerializeObject(GeneralControl.Menu_Command_Class.Instance));
                }
                //[查看战备] 长度:1 参一:View_Battle 作用:查看战备
                else if (data.Length == 1 && data[0] == GeneralControl.Menu_Command_Class.Instance.View_Battle)
                {
                    user.SendMessages("查看战备", JsonConvert.SerializeObject(user.Battle_SkillCards));
                }
                //[查看库存] 长度:1 参一:View_Repository 作用:查看库存
                else if (data.Length == 1 && data[0] == GeneralControl.Menu_Command_Class.Instance.View_Repository)
                {
                    user.SendMessages("查看库存", JsonConvert.SerializeObject(user.Repository_SkillCards));
                }
                //[查询 斩杀] 长度:2 参一:Skill_Query 参二:技能名 作用:查询技能
                else if (data.Length == 2 && data[0] == GeneralControl.Menu_Command_Class.Instance.Skill_Query)
                {
                    if (GeneralControl.Skill_Card_Name_Skllcard.TryGetValue(data[1], out SkillCard item))
                    {
                        user.SendMessages("查询#存在", JsonConvert.SerializeObject(item));
                    }
                    else user.SendMessages("查询#不存在");
                }
                //[充值仙域币 839336369 1000] 长度:3 参一:Add_Balances 参二:Q号 参三:数额 作用:加入地图
                else if (data.Length == 3 && data[0] == GeneralControl.Menu_Command_Class.Instance.Add_Balances)
                {
                    if (GeneralControl.Menu_GameControl_Class.Instance.Owner_QQ.Contains(user.UserName.ToString()))
                    {
                        if (user != null)
                        {
                            if (int.TryParse(data[2], out int temp))
                            {
                                user.Add_Balances(temp);
                                user.SendMessages("充值仙域币#成功" + temp);
                            }
                            else user.SendMessages("充值仙域币#失败#请输入合法的数额");
                        }
                        else user.SendMessages("充值仙域币#失败#玩家不存在");
                    }
                    /*
                    else if(GeneralControl.QQ_Sockets.TryGetValue(socket,out long QQ))
                    {
                        if(QQ == frompersonal)
                        {
                            user add_user = user.Load(long.Parse(data[1]));
                            Make.MODEL.Author author = Make.MODEL.Author.Load(QQ);
                            if (add_user != null)
                            {
                                if (int.TryParse(data[2], out int temp) && temp <= author.Money)
                                {
                                    add_user.Add_Balances(temp, "通过充值");
                                    author.Money -= temp;
                                    author.Save();
                                }
                                else user.SendMessages("请输入合法的数额");
                            }
                            else user.SendMessages("玩家不存在");
                        }
                    }
                    else user.SendMessages("您的权限不足,仙域卡牌运营者以及仙域开发者具有此权限");
                    */
                }
                //[抽卡] 长度:1 参一:Draw_Card 作用:抽取一张卡牌
                else if (data.Length == 1 && data[0] == GeneralControl.Menu_Command_Class.Instance.Draw_Card)
                {
                    if (user.Balances >= GeneralControl.Menu_GameControl_Class.Instance.Draw_Card_Coast)
                    {
                        SkillCard skillCard = SkillCard_Helper.Get_Random(1).FirstOrDefault();
                        if (skillCard != null)
                        {
                            user.Repository_Skill_Add(skillCard);
                            user.SendMessages("抽卡#成功",JsonConvert.SerializeObject(skillCard));
                        }
                        else user.SendMessages("抽卡#失败#无技能卡存在");
                    }
                    else user.SendMessages("抽卡#失败#仙域币不足");
                }
                //[购卡 斩杀 3] 长度:3 参一:Buy_Card 参二:技能名 参三:数量 作用:购买卡牌
                else if (data.Length == 3 && data[0] == GeneralControl.Menu_Command_Class.Instance.Buy_Card && int.TryParse(data[2], out num))
                {
                    if (GeneralControl.Menu_GameControl_Class.Instance.Can_Buy)
                    {
                        if (GeneralControl.Skill_Card_Name_Skllcard.TryGetValue(data[1], out SkillCard item) && item.State == 2)
                        {
                            SkillCard skillCard = item.Clone();
                            if (user.Add_Balances(-GeneralControl.Menu_GameControl_Class.Instance.Buy_Card_Coast * (int)Math.Pow(4, skillCard.Level - 1) * num))
                            {
                                skillCard.Amount = num;
                                user.Repository_Skill_Add(skillCard);
                                user.SendMessages("购卡#成功", JsonConvert.SerializeObject(skillCard));
                            }
                            else user.SendMessages("购卡#失败#仙域币不足");
                        }
                        else user.SendMessages("购卡#失败#技能卡不存在");
                    }
                    else user.SendMessages("购卡#失败#购买功能未开启");
                }
                //[出售 斩杀 3] 长度:3 参一:Sell_Card 参二:技能名 参三:数量 作用:出售卡牌
                else if (data.Length == 3 && data[0] == GeneralControl.Menu_Command_Class.Instance.Sell_Card && int.TryParse(data[2], out num))
                {
                    if (GeneralControl.Menu_GameControl_Class.Instance.Can_Sell)
                    {
                        if (user.Repository_Skill_Add(data[1], -num))
                        {
                            int money = GeneralControl.Menu_GameControl_Class.Instance.Sell_Card_Coast * (int)Math.Pow(4, user.Repository_SkillCards[data[1]].Level - 1) * num;
                            if (user.Add_Balances(money))
                            {
                                user.SendMessages("出售#成功" + money);
                            }
                        }
                        else user.SendMessages("出售#失败#技能卡数量不足或不存在");
                    }
                    else user.SendMessages("出售#失败#售卖功能未开启");
                }
                //[更改昵称 剑仙] 长度:2 参一:Sell_Card 参二:技能名 参三:数量 作用:出售卡牌
                else if (data.Length == 2 && data[0] == GeneralControl.Menu_Command_Class.Instance.Change_Name)
                {
                    user.NickName = data[1];
                    user.Save();
                    user.SendMessages($"更改昵称#成功#{user.NickName}");
                }
                //[升级技能卡 斩杀 3] 长度:3 参一:Upgrade_Card 参二:技能名 参三:数量 作用:升级卡
                else if (data.Length == 3 && data[0] == GeneralControl.Menu_Command_Class.Instance.Upgrade_Card && int.TryParse(data[2], out num))
                {
                    user.Upgrate_Skill(data[1], num);
                }
                //[查询金额] 长度:1 参一:View_Balances 作用:查看金额
                else if (data.Length == 1 && data[0] == GeneralControl.Menu_Command_Class.Instance.View_Balances)
                {
                    user.SendMessages("查询金额#成功#您当前的仙域币:" + user.Balances);
                }
                //[开始匹配 单挑] 长度:2 参一:Start_Match 参二:房间类型 作用:开始匹配
                else if (data.Length == 2 && data[0] == GeneralControl.Menu_Command_Class.Instance.Start_Match)
                {
                    if (data[1] == "单挑")
                    {
                        if (user.Active == Enums.User_Active.Leisure)
                        {
                            GeneralControl.Queue_Solo.Add(user);
                            user.Active = Enums.User_Active.Queue;
                            user.SendMessages("已加入匹配队列");
                            if (GeneralControl.Queue_Solo.Count >= 2)
                            {
                                Solo_Room room_Solo = new Solo_Room(2, 2);
                                room_Solo.Stage = Enums.Room.Wait;
                                GeneralControl.Rooms.Add(room_Solo);
                                for (int i = 0; i < room_Solo.Min_Personals; i++)
                                {
                                    room_Solo.Enter(GeneralControl.Queue_Solo.First());
                                    GeneralControl.Queue_Solo.RemoveAt(0);
                                }
                                room_Solo.Start();
                            }
                        }
                        else user.SendMessages("当前状态无法匹配");
                    }
                    else if (data[1] == "团战")
                    {
                        if (user.Active == Enums.User_Active.Leisure)
                        {
                            GeneralControl.Queue_Team.Add(user);
                            user.Active = Enums.User_Active.Queue;
                            user.SendMessages("已加入匹配队列");
                            if (GeneralControl.Queue_Team.Count >= 6)
                            {
                                Team_Room room_Team = new Team_Room(6, 6);
                                room_Team.Stage = Enums.Room.Wait;
                                GeneralControl.Rooms.Add(room_Team);
                                for (int i = 0; i < room_Team.Min_Personals; i++)
                                {
                                    room_Team.Enter(GeneralControl.Queue_Team.First());
                                    GeneralControl.Queue_Team.RemoveAt(0);
                                }
                                room_Team.Start();
                            }
                        }
                        else user.SendMessages("当前状态无法匹配");
                    }
                }
                //[退出匹配] 长度:1 参一:Stop_Match 作用:退出匹配
                else if (data.Length == 1 && data[0] == GeneralControl.Menu_Command_Class.Instance.Stop_Match)
                {
                    if (user.Active == Enums.User_Active.Queue)
                    {
                        if (GeneralControl.Queue_Solo.Contains(user))
                        {
                            GeneralControl.Queue_Solo.Remove(user);
                        }
                        else if (GeneralControl.Queue_Team.Contains(user))
                        {
                            GeneralControl.Queue_Team.Remove(user);
                        }
                        else if (GeneralControl.Queue_Battle_Royale.Contains(user))
                        {
                            GeneralControl.Queue_Battle_Royale.Remove(user);
                        }
                        user.Active = Enums.User_Active.Leisure;
                        user.SendMessages("退出匹配成功");
                    }
                    else user.SendMessages("您当前不在匹配队列");
                }
                //[房间信息 2] 长度:2 参一:Room_Condition 参二：房间号 作用:查询房间信息
                else if (data.Length == 2 && data[0] == GeneralControl.Menu_Command_Class.Instance.Room_Condition && int.TryParse(data[1], out num))
                {
                    if(GeneralControl.Rooms.Count>= num && num >= 0)
                    {
                        user.SendMessages("房间信息#成功", JsonConvert.SerializeObject(GeneralControl.Rooms[num]));
                    }
                    else
                    {
                        user.SendMessages("附加信息#失败#查询房间不存在");
                    }
                    
                }
                //[房间信息 2] 长度:2 参一:Room_Condition 参二：房间号 作用:查询房间信息
                else if (data.Length == 1 && data[0] == "卡片")
                {
                    user.SendMessages("[CQ:json,data=" + File.ReadAllText(@"D:\YuanMa\xianyu.xianzhan.core\Pack\TextFile1.txt") + "]");
                }
            }
        }
        public static void Receive_Game(Token token, Msg_Client msg_Client)
        {
            string[] data = msg_Client.Head.Split('#');
            int num;
            if(GeneralControl.Players.TryGetValue(msg_Client.Token.Username_token, out Player player))
            {
                if (data.Length > 0)
                {
                    //[查看手牌] 长度:1 参一:View_Hand 作用:查看手牌
                    if (data.Length == 1 && data[0] == GeneralControl.Menu_Command_Class.Instance.View_Hand)
                    {
                        player.Send("查看手牌#成功", player.Hand_SkillCards);
                    }
                }
                else if (player.Active == Enums.Player_Active.Round)
                {
                    Receive_Room(msg_Client);
                }
                else if(player.Active == Enums.Player_Active.Map)
                {
                    Receive_Map(msg_Client);
                }
            }
            
        }

        /// <summary>
        /// 回合制在房间内的指令
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="msg"></param>
        /// <param name="frompersonal"></param>
        /// <param name="fromgroup"></param>
        public static void Receive_Room(Msg_Client msg_Client)
        {
            int num;
            string[] data = msg_Client.Head.Split('#');
            if (GeneralControl.Players.TryGetValue(msg_Client.Token.Username_token, out Player player))
            {
                if(player.Room == null)
                {
                    GeneralControl.Players.Remove(player.UserName);
                }
                //[斩杀 2] 长度:1或2 参一:技能名  参二:位置号 作用:发动技能
                if (data.Length > 0 && (player.Hand_SkillCards.ContainsKey(data[0])))
                {
                    player.Room.Action_Stage(player, data);
                }
                //[无招] 长度:1 参一:技能名  作用:跳过技能
                else if (data.Length == 1 && data[0] == "无招")
                {
                    if (!player.Action)
                    {
                        player.Action = true;
                        player.Send("无招#成功");
                        if (!((from Player item in player.Room.Players where item.Action == false select item).Any())) player.Room.Result_Stage();
                    }
                    else player.Send("无招#失败#您已出招");
                }
                //[离开房间] 长度:1 参一:Leave_Room 作用:退出房间
                else if (data.Length == 1 && data[0] == GeneralControl.Menu_Command_Class.Instance.Leave_Room)
                {
                    if (player.Room != null)
                    {
                        //判断是否房间是等待状态
                        if (player.Room.Stage == Enums.Room.Wait)
                        {
                            player.Room.Leave(player);
                        }
                        else player.Send("当前房间状态下无法退出");
                    }
                    else player.Send("您当前不在房间");
                }
                //[聊天 哈哈] 长度:2 参一:Chat 参二：内容 作用:聊天内容
                else if (data.Length == 2 && data[0] == GeneralControl.Menu_Command_Class.Instance.Chat)
                {
                    player.Room.SendMessages(player.NickName + ":" + data[1]);
                }
                //[机器人] 长度:1 参一:AI_Bot 作用:加入房间一个智能机器人
                else if (data.Length == 1 && data[0] == GeneralControl.Menu_Command_Class.Instance.AI_Bot)
                {
                    if (player.Room.Stage == Enums.Room.Wait)
                    {
                        player.Room.Enter(NPC_Helper.New_NPC());
                        player.Send("机器人#成功");
                        if (player.Room.Players.Count >= player.Room.Max_Personals) player.Room.Start();
                    }
                    else player.Send("机器人#失败#房间已处于非等待阶段，无法加入机器人");
                }
                //[投降] 长度:1 参一:Surrender 作用:投降
                else if (data.Length == 1 && data[0] == GeneralControl.Menu_Command_Class.Instance.Surrender)
                {
                    try
                    {
                        player.Room.SendMessages(player.NickName + "已投降");
                        player.Room.Players.Remove(player);
                        GeneralControl.Players.Remove(player.UserName);
                        Room room = player.Room;
                        player.Init();
                        if ((from Player item in room.Players where item.Action == false select item).Any() == false) room.Result_Stage();
                    }
                    catch (Exception e)
                    {
                        XY.Console_Write(e.Message);
                    }
                }
                //[强制结束 2] 长度:2 参一:Compulsory_Stop 参二:房间号 作用:强制结束房间
                else if (data.Length == 2 && data[0] == GeneralControl.Menu_Command_Class.Instance.Compulsory_Stop && int.TryParse(data[1], out num))
                {
                    if (GeneralControl.Rooms.Count - 1 >= num && num >= 0)
                    {
                        player.Room.Force_Close();
                        player.Send("房间已经进入自我销毁");
                    }
                    else player.Send("房间号不存在");
                }
            }
        }

        /// <summary>
        /// 即时战略在地图中的指令
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="msg"></param>
        /// <param name="frompersonal"></param>
        /// <param name="fromgroup"></param>
        public static void Receive_Map(Msg_Client msg_Client)
        {
            int num;
            string[] data = msg_Client.Head.Split('#');
            int x, y;
            if (GeneralControl.Players.TryGetValue(msg_Client.Token.Username_token, out Player player))
            {
                if(player == null)
                //[-2 3] 长度:2 参一:X标  参二:Y标 作用:移动
                if (data.Length >= 2 && int.TryParse(data[0], out int result) && int.TryParse(data[1], out int result_1))
                {
                    int pos_x, pos_y;
                    if (data.Length == 2 && int.TryParse(data[0], out x) && int.TryParse(data[1], out y))
                    {
                        pos_x = player.Pos.X + x;
                        pos_y = player.Pos.Y - y;
                    }
                    else if (data.Length == 3 && data[0] == "5" && int.TryParse(data[1], out x) && int.TryParse(data[2], out y))
                    {
                        pos_x = x;
                        pos_y = y;
                    }
                    else
                    {
                        player.Send("无效移动");
                        return;
                    }
                    if (!(pos_x < 0 || pos_y < 0 || pos_x > player.Map.Width || pos_y > player.Map.Height || player.Map.Pos_Map[pos_x, pos_y].Item is Player))
                    {
                        player.Move(player.Map.Pos_Map[pos_x, pos_y]);
                    }
                    else player.Send("无效移动");
                }
                //[66688882222] 长度:1 参一:移动方向和次数 作用:移动
                else if (data.Length == 1 && int.TryParse(data[0], out x))
                {
                    y = Regex.Matches(data[0], "[2]").Count - Regex.Matches(data[0], "[8]").Count;
                    x = Regex.Matches(data[0], "[4]").Count - Regex.Matches(data[0], "[6]").Count;
                    int pos_x = player.Pos.X - x, pos_y = player.Pos.Y - y;
                    if (!(pos_x < 0 || pos_y < 0 || pos_x > player.Map.Width || pos_y > player.Map.Height || player.Map.Pos_Map[pos_x, pos_y].Item is Player))
                    {
                        player.Move(player.Map.Pos_Map[pos_x, pos_y]);
                    }
                    else player.Send("无效移动");
                }
                //[斩杀 2] 长度:1或3或4 参一:技能名  参二:X位置号 参三:Y位置号  作用:发动技能
                else if (data.Length >= 1 && player.Hand_SkillCards.ContainsKey(data[0]))
                {
                    player.Map.Action_Stage(player, data);
                }
                //[聊天 哈哈] 长度:2 参一:Chat 参二：内容 作用:聊天内容
                else if (data.Length == 2 && data[0] == GeneralControl.Menu_Command_Class.Instance.Chat)
                {
                    foreach(Player item in player.Map.Players.Values)
                    {
                            item.Send($"聊天#{player.NickName}:{data[1]}");
                    }
                }
                //[清识] 长度:1或2或3或4 参一:View_Player 作用:查看状态
                else if (data.Length >= 1 && data[0] == GeneralControl.Menu_Command_Class.Instance.View_Player)
                {
                    player.Send("清识#成功", player);
                }
                //[传送 20 300] 长度:3 参一:TP  参二:X标  参三:Y标 作用:传送
                else if (data.Length == 3 && data[0] == GeneralControl.Menu_Command_Class.Instance.TP && int.TryParse(data[1], out x) && int.TryParse(data[2], out y) && !(x < 0 && y < 0 && x > player.Map.Width && y > player.Map.Height))
                {
                    if (!(x < 0 && y < 0 && x > player.Map.Width && y > player.Map.Height && player.Map.Pos_Map[x, y].Item != null))
                    {
                            
                        if (User.Load(player.UserName).Add_Balances(-GeneralControl.Menu_GameControl_Class.Instance.TP_Coast))
                        {
                            player.Pos.Init();
                            player.Pos = null;
                            player.Move(player.Map.Pos_Map[x, y]);
                            player.Send($"传送#成功", player.Pos);
                        }
                        else player.Send($"传送#游戏币不足");
                    }
                    else player.Send("传送#无效传送");
                }
                //[魂命 20 300] 长度:1|3|4 参一:Direct  参二:X标  参三:Y标 作用:锁定目标
                else if (data.Length >= 1 && data[0] == GeneralControl.Menu_Command_Class.Instance.Direct)
                {
                    if (data.Length == 1)
                    {
                        player.Send("魂命#锁定成功", player.Enemies);
                        return;
                    }
                    int pos_x, pos_y;
                    if (data.Length == 2 && int.TryParse(data[1], out x))
                    {
                        y = Regex.Matches(data[1], "[2]").Count - Regex.Matches(data[1], "[8]").Count;
                        x = Regex.Matches(data[1], "[4]").Count - Regex.Matches(data[1], "[6]").Count;
                        pos_x = player.Pos.X - x;
                        pos_y = player.Pos.Y - y;
                    }
                    else if (data.Length == 3 && int.TryParse(data[1], out x) && int.TryParse(data[2], out y))
                    {
                        pos_x = player.Pos.X + x;
                        pos_y = player.Pos.Y - y;
                    }
                    else if (data.Length == 4 && data[1] == "5" && int.TryParse(data[2], out x) && int.TryParse(data[3], out y))
                    {
                        pos_x = x;
                        pos_y = y;
                    }
                    else
                    {
                        player.Send("魂命#无效锁定");
                        return;
                    }
                    if (!(pos_x < 0 || pos_y < 0 || pos_x > player.Map.Width || pos_y > player.Map.Height))
                    {
                        if (player.Map.Pos_Map[pos_x, pos_y].Item is Player)
                        {
                            if (!player.Enemies.Contains(player.Map.Pos_Map[pos_x, pos_y].Item))
                            {
                                player.Add_Enemy(player.Map.Pos_Map[pos_x, pos_y].Item as Player);
                            }
                            else player.Send((player.Map.Pos_Map[pos_x, pos_y].Item as Player).NickName + "魂命#重复锁定");
                        }
                        else player.Send("魂命#无效锁定");
                    }
                    else player.Send("魂命#无效锁定");
                }
                //[灵命 20 300] 长度:3|4 参一:Friend  参二:X标  参三:Y标 作用:结交目标
                else if (data.Length >= 3 && data[0] == GeneralControl.Menu_Command_Class.Instance.Friend)
                {
                    if (data.Length == 1)
                    {
                        player.Send("灵命#锁定成功", player.Enemies);
                        return;
                    }
                    int pos_x, pos_y;
                    if (data.Length == 2 && int.TryParse(data[1], out x))
                    {
                        y = Regex.Matches(data[1], "[2]").Count - Regex.Matches(data[1], "[8]").Count;
                        x = Regex.Matches(data[1], "[4]").Count - Regex.Matches(data[1], "[6]").Count;
                        pos_x = player.Pos.X - x;
                        pos_y = player.Pos.Y - y;
                    }
                    else if (data.Length == 3 && int.TryParse(data[1], out x) && int.TryParse(data[2], out y))
                    {
                        pos_x = player.Pos.X + x;
                        pos_y = player.Pos.Y - y;
                    }
                    else if (data.Length == 4 && data[1] == "5" && int.TryParse(data[2], out x) && int.TryParse(data[3], out y))
                    {
                        pos_x = x;
                        pos_y = y;
                    }
                    else
                    {
                        player.Send("灵命#无效锁定");
                        return;
                    }
                    if (!(pos_x < 0 || pos_y < 0 || pos_x > player.Map.Width || pos_y > player.Map.Height))
                    {
                        if (player.Map.Pos_Map[pos_x, pos_y].Item is Player)
                        {
                            if (!player.Friends.Contains(player.Map.Pos_Map[pos_x, pos_y].Item))
                            {
                                player.Add_Friend(player.Map.Pos_Map[pos_x, pos_y].Item as Player);
                            }
                            else player.Send((player.Map.Pos_Map[pos_x, pos_y].Item as Player).NickName + "灵命#重复锁定");
                        }
                        else player.Send("灵命#无效锁定");
                    }
                    else player.Send("灵命#无效锁定");
                }
                //[消魂 2] 长度:2 参一:Cancel_Direct  参二:序号  作用:取消该目标的锁定
                else if (data.Length == 2 && data[0] == GeneralControl.Menu_Command_Class.Instance.Cancel_Direct)
                {
                    if(int.TryParse(data[1], out num) && num < player.Enemies.Count())
                    {
                        player.Cancel_Direct(player.Enemies.ElementAt(num));
                    }
                }
                //[消灵 3] 长度:2 参一:Cancel_Friend  参二:序号  作用:取消该目标的锁定
                else if (data.Length == 2 && data[0] == GeneralControl.Menu_Command_Class.Instance.Cancel_Friend)
                {
                    if(int.TryParse(data[1], out num) && num < player.Friends.Count())
                    {
                        player.Cancel_Friend(player.Friends.ElementAt(num));
                    }
                }
                //[离开地图] 长度:1 参一:Leave_Map 作用:离开地图
                else if (data.Length == 1 && data[0] == GeneralControl.Menu_Command_Class.Instance.Leave_Map)
                {
                    player.Map.Leave(player);
                    player.Send("离开地图#成功");
                }
                //[探知] 长度:1 参一:Review 作用:查看周围情况
                else if (data.Length == 1 && data[0] == GeneralControl.Menu_Command_Class.Instance.Review)
                {
                        //**游戏大世界地图代码，基本框架已成，但还未进行客户端兼容
                        // player.Review();
                }
            }
        }
        /*
        /// <summary>
        /// 管理员操作
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="msg"></param>
        /// <param name="frompersonal"></param>
        /// <param name="fromgroup"></param>
        public static void Receive_Administrator(Player player, string msg, long frompersonal, long fromgroup = -1)
        {   
            string[] data = msg.Split(' ');
            //[-2 3] 长度:2 参一:X标  参二:Y标 作用:移动
            if (data.Length == 2 && data[0] == "批准奇遇")
            {
                Adventure adventure = (from Adventure item in GeneralControl.Menu_Data_Monitor_Class.Instance.Pubmit_Adventures where item.ID == data[1] select item).FirstOrDefault();
                if (adventure != null)
                {
                    App.Current.Dispatcher.Invoke((Action)delegate ()
                    {
                        IEnumerable<Custom_Card_Adventure> ienum = from Custom_Card_Adventure ienum_item in Pack_General.MainWindow.AdventurePanle.AdventurePanel.Children where adventure.ID == ienum_item.Adventure.ID select ienum_item;
                        if (ienum.Any())
                        {
                            adventure.Delete();
                            ienum.First().Adventure = adventure;
                            ienum.First().DataContext = adventure;
                            adventure.Save();
                        }
                        else
                        {
                            Pack_General.MainWindow.AdventurePanle.Add_Adventure(adventure);
                            adventure.Save();
                        }
                        adventure.Add_To_General();
                        player.SendMessages(adventure.Name + "成功通过");
                    });
                }
                else player.SendMessages("查无此申请");
            }
            else if (data.Length == 2 && data[0] == "批准技能卡")
            {
                SkillCardsModel skillCardsModel = (from SkillCardsModel item in GeneralControl.Menu_Data_Monitor_Class.Instance.Pubmit_Adventures where item.ID== data[1] select item).FirstOrDefault();
                if (skillCardsModel != null)
                {
                    App.Current.Dispatcher.Invoke((Action)delegate ()
                    {
                        IEnumerable<Custom_Card_SkillCard> ienum = from Custom_Card_SkillCard ienum_item in Pack_General.MainWindow.CardPanle.CardsPanel.Children where skillCardsModel.ID == ienum_item.SkillCardsModel.ID select ienum_item;
                        if (ienum.Any())
                        {
                            Custom_Card_SkillCard skillCards = ienum.First();
                            skillCards.SkillCardsModel.Delete();
                            skillCards.SkillCardsModel = skillCardsModel;
                            skillCards.DataContext = skillCards.SkillCardsModel.SkillCards[skillCards.Rate.Value - 1];
                            skillCards.SkillCardsModel.Save();
                        }
                        else
                        {
                            Pack_General.MainWindow.CardPanle.Add_Card(skillCardsModel);
                            GeneralControl.Skill_Cards.Add(skillCardsModel);
                            skillCardsModel.Save();
                        }
                        skillCardsModel.Add_To_General();
                        player.SendMessages(skillCardsModel.ID + "成功批准");
                    });
                }
                else player.SendMessages("查无此申请");
            }
        }
        */
    }
}
