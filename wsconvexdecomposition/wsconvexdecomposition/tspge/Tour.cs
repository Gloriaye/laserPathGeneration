using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using ClipperLib;

namespace wsconvexdecomposition
{
    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;

    public class Tour{

    // 保持所有城市的路径
    public List<City> tour = new List<City>();
    // Cache
    private double fitness = 0;
    public int distance = 0;
    private int citynum = 0;
    public Paths orderPath = new Paths ();
    // Constructs a blank tour

    public Tour( int citynum){

        this.citynum = citynum;
        for (int i = 0; i < citynum; i++)
        {
            tour.Add(null);
        }
    }
    
    public Tour(List<City> tour){
        this.tour = tour;
    }

    //创建一个随机的 个体(路径)
    public void generateIndividual() {
        // Loop through all our destination cities and add them to our tour
        for (int cityIndex = 0; cityIndex < citynum; cityIndex++)
        {
            setCity(cityIndex, new City(cityIndex));
        }
        // 打乱顺序
        //Collections.shuffle(tour);
        tour = RandSort<City>(tour);
    }

    // Gets a city from the tour
    public City getCity(int tourPosition) {
        return (City)tour[tourPosition];
    }

    // 讲城市加入到路径中
    public void setCity(int tourPosition, City city) {
       // tour.set(tourPosition, city);
        tour[tourPosition] = city;
        // If the tours been altered we need to reset the fitness and distance
        fitness = 0;
        distance = 0;
    }
    
    // 距离小的 适应值较大
    public double getFitness() {
        if (fitness == 0) {
            fitness = 1/(double)distance;
        }
        return fitness;
    }
    
    // 获取当前路径的总距离
    //public int getDistance(){
    //    if (distance == 0) {
    //        int tourDistance = 0;
    //        for (int cityIndex=0; cityIndex < tourSize(); cityIndex++) {
    //            // Get city we're travelling from
    //            City fromCity = getCity(cityIndex);
    //            // City we're travelling to
    //            City destinationCity;
    //            // Check we're not on our tour's last city, if we are set our 
    //            // tour's final destination city to our starting city
    //            if(cityIndex+1 < tourSize()){
    //                destinationCity = getCity(cityIndex+1);
    //            }
    //            else{
    //                destinationCity = getCity(0);
    //            }
    //            // Get the distance between the two cities
    //            tourDistance += (int)fromCity.distanceTo(destinationCity);
    //        }
    //        distance = tourDistance;
    //    }
    //    return distance;
    //}


    public int getDistance( Paths pgs1,IntPoint startpoint)
    {
        Paths pgs = new Paths(pgs1);
        Paths m_orderpath = new Paths();
        if (distance == 0)
        {
            float  tourDistance = 0.0f;
            IntPoint p0 = new IntPoint(startpoint);

            for (int i = 0; i < pgs.Count; i++)
            {
                int index = tour[i].numberorder;    //获取该条路径的索引
                float dist1 = vSize2f(pgs[index][0], p0);   //第一个点
                float dist2 = vSize2f(pgs[index][pgs[index].Count() - 1], p0);   //path的最后一点

                if (dist1 < dist2)
                {
                    tourDistance += dist1; 
                    tour[i].conversta = false;                 //不需要翻转
                    p0 = pgs[index][pgs[index].Count() - 1];   //p0为最后一点
                    m_orderpath.Add(pgs[index]);
                }
                else
                {
                    tourDistance += dist2;
                    tour[i].conversta = true;            //需要翻转
                    p0 = pgs[index][0];                 //p0 为第一点
                    Path tempg = new Path(pgs[index]);
                    tempg.Reverse();
                    m_orderpath.Add(tempg);
                }
            }
            distance = (int)tourDistance;
            orderPath = new Paths (m_orderpath);
        }
        return distance;
    }

    private float vSize2f(IntPoint v1, IntPoint v2)  //测量两点间的距离
    {
      return (float)Math.Sqrt(Math.Pow(v1.X - v2.X, 2) + Math.Pow(v1.Y - v2.Y, 2));  //绝对距离
        //return Math.Max(Math.Abs(v1.X - v2.X),Math.Abs(v1.Y - v2.Y));           //时间距离
    }


    // Get number of cities on our tour
    public int tourSize() {
        return tour.Count();
    }
    
    // Check if the tour contains a city
    public bool containsCity(City city)
    {
        foreach (City temp in tour)
        {
            if (temp != null)
            {
                if (temp.numberorder == city.numberorder)
                    return true;
            }
        }
        return false;
    }
    
    public String toString() {
        String geneString = "|";
        for (int i = 0; i < tourSize(); i++) {
            geneString += getCity(i)+"|";
        }
        return geneString;
    }


    //ws 2016 0528 随机排序
    public static List<T> RandSort<T>(List<T> arry)
    {
        List<T> arryNew = new List<T>();
        Random rnd = new Random();
        int n = arry.Count;

        for (int j = 0; j < n; j++)
        {
            arryNew.Add(arry[j]);
        }

        int i = 0;
        while (n > 0)
        {
            int index = rnd.Next(n);
            arryNew[i] = arry[index];
            arry[index] = arry[n - 1];
            n--;
            i++;
        }

        return arryNew;

    }
}
}
