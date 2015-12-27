using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLStore
{
    class ServerInfo
    {
        public static readonly ServerInfo NA = new ServerInfo("NA", "NA1", "North America", "prod.na2.lol.riotgames.com", "https://lq.na2.lol.riotgames.com/");
        public static readonly ServerInfo EUW = new ServerInfo("EUW", "EUW1", "Europe West", "prod.euw1.lol.riotgames.com", "https://lq.euw1.lol.riotgames.com/");
        public static readonly ServerInfo EUNE = new ServerInfo("EUNE", "EUN1", "Europe Nordic & East", "prod.eun1.lol.riotgames.com", "https://lq.eun1.lol.riotgames.com/");
        public static readonly ServerInfo KR = new ServerInfo("KR", "KR", "Korea", "prod.kr.lol.riotgames.com", "https://lq.kr.lol.riotgames.com/");
        public static readonly ServerInfo BR = new ServerInfo("BR", "BR1", "Brazil", "prod.br.lol.riotgames.com", "https://lq.br.lol.riotgames.com/");
        public static readonly ServerInfo TR = new ServerInfo("TR", "TR1", "Turkey", "prod.tr.lol.riotgames.com", "https://lq.tr.lol.riotgames.com/");
        public static readonly ServerInfo RU = new ServerInfo("RU", "RU", "Russia", "prod.ru.lol.riotgames.com", "https://lq.ru.lol.riotgames.com/");
        public static readonly ServerInfo LAN = new ServerInfo("LAN", "LA1", "Latin America North", "prod.la1.lol.riotgames.com", "https://lq.la1.lol.riotgames.com/");
        public static readonly ServerInfo LAS = new ServerInfo("LAS", "LA2", "Latin America South", "prod.la2.lol.riotgames.com", "https://lq.la2.lol.riotgames.com/");
        public static readonly ServerInfo OCE = new ServerInfo("OCE", "OC1", "Oceania", "prod.oc1.lol.riotgames.com", "https://lq.oc1.lol.riotgames.com/");
        public static readonly ServerInfo PBE = new ServerInfo("PBE", "PBE1", "Public Beta Environment", "prod.pbe1.lol.riotgames.com", "https://lq.pbe1.lol.riotgames.com/");
        public static readonly IReadOnlyCollection<ServerInfo> ALL = new List<ServerInfo> {NA,EUW,EUNE,KR,BR,TR,RU,LAN,LAS,OCE,PBE}.AsReadOnly();

        public string Region;
        public string Platform;
        public string Name;
        public string Server;
        public string LoginQueue;


        public ServerInfo(string region, string platform, string name, string server, string loginQueue)
        {
            Region = region;
            Platform = platform;
            Name = name;
            Server = server;
            LoginQueue = loginQueue;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
