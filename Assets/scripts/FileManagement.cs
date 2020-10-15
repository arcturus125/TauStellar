using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

public class FileManagement : MonoBehaviour
{
    public static void readFile(string file_path)
    {
        StreamReader inp_stm = new StreamReader(file_path);

        while (!inp_stm.EndOfStream)
        {
            string inp_ln = inp_stm.ReadLine();
            // Do Something with the input. 
        }

        inp_stm.Close();
    }
    public static string readLine(string file_path, int lineNumber)
    {
        StreamReader inp_stm = new StreamReader(file_path);

        while (!inp_stm.EndOfStream)
        {
            for(int i =0; i < lineNumber - 1; i++)
            {
                inp_stm.ReadLine();
            }
            string inp_ln = inp_stm.ReadLine();
            return inp_ln;
        }

        inp_stm.Close();
        return null;
    }
    public static int getFileLength(string file_path)
    {
        int lines = 0;
        StreamReader inp_stm = new StreamReader(file_path);
        while (!inp_stm.EndOfStream)
        {
            inp_stm.ReadLine();
            lines++;
            if (lines > 200)// just incase there is a bug. this prevents infinite loops
            {
                break;
            }
        }

        inp_stm.Close();
        return lines;
    }
}
