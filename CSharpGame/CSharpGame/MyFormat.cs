﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace CSharpGame
{
    class  MyFormat
    {
        public static void genPic(ref int[] a)
        {
            Random r = new Random();
            for(int i = 0; i < 64; i++)                             
            {    
                a[i] = r.Next(0,16);
            }           
        }

        public static int countPairPic(int[] a)
        {
            int count = 0;
            for (int i = 0; i < a.Length; i++)
            {
               if (a[i] != -1)
               {
                   for (int j = i + 1; j < a.Length; j++)
                   {
                       if (a[j] != -1)
                       {
                           if (a[i] == a[j])
                           {
                               count ++;                           
                               a[j] = -1;
                               j = a.Length;
                           }
                       }
                   }
               }
            }
            return count;
        }
    }
}
