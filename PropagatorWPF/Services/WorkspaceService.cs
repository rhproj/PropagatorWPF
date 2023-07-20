using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace PropagatorWPF.Services
{
    /// <summary>
    /// Сервис для нахождения рабочих мест пользователей
    /// </summary>
    public class WorkspaceService
    {
        public readonly string pathArm = AppDomain.CurrentDomain.BaseDirectory + @"Arm.csv";
        public List<string> ArmsOn { get; set; } = new List<string>();
        public List<string> ArmsOff { get; set; } = new List<string>();
        public List<string> ArmsList { get; set; } = new List<string>();
        public List<string> ResourseList { get; set; } = new List<string>();

        public async Task FillUsersList()
        {
            Ping ping = new Ping();

            foreach (var arm in ArmsList)
            {
                PingReply pingReply = await ping.SendPingAsync(arm);

                if (pingReply.Status == IPStatus.Success)
                {
                    ArmsOn.Add($@"\\{arm}\c$\Users");
                }
                else
                {
                    ArmsOff.Add(arm);
                }
            }
        }

        public void FillArmsList()
        {
            using (StreamReader sR = new StreamReader(pathArm, Encoding.Default))
            {
                while (!sR.EndOfStream)
                {
                    string[] armLine = sR.ReadLine().Split(';');
                    ArmsList.Add(armLine[0]);
                }
            }
        }
    }
}
