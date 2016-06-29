using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClipperLib;
using System.IO;

namespace wsconvexdecomposition
{
    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;
    class mGcodeExport
    {
        String mFilePath;                   //文件路径和名
        Paths mOriginalPathsSet=new Paths ();            //原始的路径数据，包括是多边形还是路径
        List<bool> mPathTypestates=new List<bool>();         //表示是闭合多边形还是路径type ,true表示闭合多边形，false表示线路径
        public String outGcodeStr;

        int mBeZoomTime;                   //数据被放大了多少倍 ，相当于Scale，由于intpoint和floatpoint计算间的转换
        int machineSpeed;                  //加工速度
        int nopTravelSpeed;                //空行程速度


        

        //用文件名字符串初始化class
        public mGcodeExport(String filename) 
        {
            this.mFilePath = filename;
        }

        //设置文件名路径、加工速度、空行程速度、被放大的倍数
        public void  SetFileName(String str){this.mFilePath=str ;}
        public void SetMachineSpeed(int machinesp){this.machineSpeed=machinesp;}
        public void SetNopTraveSpeed(int nopTravelSp){this.nopTravelSpeed=nopTravelSp;}
        public void ConfigMachingParam(int machinesp, int nopTravelSp, int times) { this.machineSpeed = machinesp; this.nopTravelSpeed = nopTravelSp; this.mBeZoomTime = times; }
        public void SetmBeZoomTime(int times) { this.mBeZoomTime = times; }
        //插入路径或多边形集合
        public void  AddPathsOrPologons(Paths inpaths, bool pathtype)
        {
            foreach (Path tempths in inpaths)
            {
                mOriginalPathsSet.Add(tempths);
                mPathTypestates.Add(pathtype);
            }
        }

        //override 插入单条路径或轮廓
        public void AddPathsOrPologons(Path inpath, bool pathtype)
        {
               mOriginalPathsSet.Add(inpath);
               mPathTypestates.Add(pathtype);          
        }

        //public void GenerateGcode()
        //{
        //    FileStream myFs = new FileStream(mFilePath, FileMode.Create);
        //    StreamWriter myStreamWriter = new StreamWriter(myFs);
        //    String header;
        //    String str;

        //    foreach (Path pt in mOriginalPathsSet)
        //    {
        //        int index = mOriginalPathsSet.IndexOf(pt);
        //        if (mPathTypestates[index])    //如果是多边形
        //        {
        //            myStreamWriter.Write("F{0}\r\n", nopTravelSpeed);
        //            header = "G01";              //运动到第一个点是空行程
        //            foreach (IntPoint temPoint in pt)    //写点
        //            {
        //                str = String.Format("{0}\0X{1}\0Y{2}\r\n", header, (float)temPoint.X / mBeZoomTime, (float)temPoint.Y / mBeZoomTime);
        //                myStreamWriter.Write(str);
        //                header = "G00";
        //            }
        //            //写入闭合的初始点
        //            str = String.Format("{0}\0X{1}\0Y{2}\r\n", header, (float)pt[0].X / mBeZoomTime, (float)pt[0].Y / mBeZoomTime);
        //            myStreamWriter.Write(str);
        //            myStreamWriter.Write("\r\n");
        //        }
        //        else 
        //        {
        //            myStreamWriter.Write("F{0}\r\n", nopTravelSpeed);
        //            header = "G01";              //运动到第一个点是空行程
        //            foreach (IntPoint temPoint in pt)    //写点     //转义符\0表空格  \t是TAb
        //            {
        //                str = String.Format("{0}\0X{1}\0Y{2}\r\n", header, (float)temPoint.X / mBeZoomTime, (float)temPoint.Y / mBeZoomTime);
        //                myStreamWriter.Write(str);
        //                header = "G00";
        //            }
        //            myStreamWriter.Write("\r\n");
        //        }
                            
        //    }
        //    myStreamWriter.Close();
        //    myFs.Close();
        //}

        public void GenerateGcode()
        {
            FileStream myFs = new FileStream(mFilePath, FileMode.Create);
            StreamWriter myStreamWriter = new StreamWriter(myFs);
            String header;
            String str;

            foreach (Path pt in mOriginalPathsSet)
            {
                int index = mOriginalPathsSet.IndexOf(pt);
                if (mPathTypestates[index])    //如果是多边形
                {
                    header = "G01";              //运动到第一个点是空行程
                    foreach (IntPoint temPoint in pt)    //写点
                    {
                        int indexl = pt.IndexOf(temPoint);
                        if (indexl == 0)
                        {
                            str = String.Format("{0}\0X{1}\0Y{2}\0F{3}\r\n", header, (float)temPoint.X / mBeZoomTime, (float)temPoint.Y / mBeZoomTime, nopTravelSpeed);
                            myStreamWriter.Write(str);
                            header = "G00";
                        }
                        else if (indexl == 1)
                        {
                            str = String.Format("{0}\0X{1}\0Y{2}\0F{3}\r\n", header, (float)temPoint.X / mBeZoomTime, (float)temPoint.Y / mBeZoomTime, machineSpeed);
                            myStreamWriter.Write(str);
                            //                  header = "G00";
                        }
                        else
                        {
                            str = String.Format("{0}\0X{1}\0Y{2}\r\n", header, (float)temPoint.X / mBeZoomTime, (float)temPoint.Y / mBeZoomTime);
                            myStreamWriter.Write(str);
                        }
                    }
                    //写入闭合的初始点
                    str = String.Format("{0}\0X{1}\0Y{2}\r\n", header, (float)pt[0].X / mBeZoomTime, (float)pt[0].Y / mBeZoomTime);
                    myStreamWriter.Write(str);
                    myStreamWriter.Write("\r\n");
                }
                else
                {
                    header = "G01";              //运动到第一个点是空行程
                    foreach (IntPoint temPoint in pt)    //写点     //转义符\0表空格  \t是TAb
                    {
                        int indexl = pt.IndexOf(temPoint);
                        if (indexl == 0)
                        {
                            str = String.Format("{0}\0X{1}\0Y{2}\0F{3}\r\n", header, (float)temPoint.X / mBeZoomTime, (float)temPoint.Y / mBeZoomTime, nopTravelSpeed);
                            myStreamWriter.Write(str);
                            header = "G00";
                        }
                        else if (indexl == 1)
                        {
                            str = String.Format("{0}\0X{1}\0Y{2}\0F{3}\r\n", header, (float)temPoint.X / mBeZoomTime, (float)temPoint.Y / mBeZoomTime, machineSpeed);
                            myStreamWriter.Write(str);
                            //                  header = "G00";
                        }
                        else
                        {
                            str = String.Format("{0}\0X{1}\0Y{2}\r\n", header, (float)temPoint.X / mBeZoomTime, (float)temPoint.Y / mBeZoomTime);
                            myStreamWriter.Write(str);
                        }
                    }
                    myStreamWriter.Write("\r\n");
                }

            }
            myStreamWriter.Close();
            myFs.Close();
            FileStream myFss = new FileStream(mFilePath, FileMode.Open);
            StreamReader myStreamReader = new StreamReader(myFss);
            String mstr = myStreamReader.ReadToEnd();
            mstr = mstr.Replace("\0", " ");
            outGcodeStr = mstr;
            myStreamReader.Close();
            myFss.Close();
        }

    }

    
}
