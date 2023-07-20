using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PropagatorWPF.Services
{
    public static class ResourceCopier
    {
        public static void CopyAll(DirectoryInfo fromD, DirectoryInfo toD)
        {
            if (fromD == null || toD == null)
                throw new ArgumentNullException();

            foreach (FileInfo fI in fromD.GetFiles())
            {
                fI.CopyTo(Path.Combine(toD.FullName, fI.Name), true);
            }
            foreach (DirectoryInfo sourceDirs in fromD.GetDirectories())
            {
                DirectoryInfo targDirs = toD.CreateSubdirectory(sourceDirs.Name);
                CopyAll(sourceDirs, targDirs);
            }
        }

        public static int CountAllResItems(DirectoryInfo fromD, List<string> resourseList)
        {
            if (fromD == null || resourseList == null)
                throw new ArgumentNullException();
            if (resourseList.Count == 0)
                throw new ArithmeticException(nameof(resourseList)+"не должен быть пустым");

            foreach (FileInfo fI in fromD.GetFiles())
            {
                resourseList.Add(fI.Name);
            }

            foreach (DirectoryInfo sourceDirs in fromD.GetDirectories())
            {
                resourseList.Add(sourceDirs.Name);
            }

            return resourseList.Count();
        }
    }
}
