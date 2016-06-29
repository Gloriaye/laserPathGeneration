using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wsconvexdecomposition
{

//   public class City {
//    int x;
//    int y;
    
//    //构造一个随机的city
//    public City(){
//        this.x = (int)(new Random().NextDouble()*200);
//        this.y = (int)(new Random().NextDouble()*200);
//    }
    
//    public City(int x, int y){
//        this.x = x;
//        this.y = y;
//    }
    
//    public int getX(){
//        return this.x;
//    }
    
//    public int getY(){
//        return this.y;
//    }
    
//    // 计算到给定 city的距离
//    public double distanceTo(City city){
//        int xDistance = Math.Abs(getX() - city.getX());
//        int yDistance = Math.Abs(getY() - city.getY());
//        double distance = Math.Sqrt( (xDistance*xDistance) + (yDistance*yDistance) );
        
//        return distance;
//    }
    
//    public String toString(){
//        return getX()+", "+getY();
//    }
//}

    public class City
    {
        public int numberorder;
        public bool conversta = false;

        public City( int m_number){
            this.numberorder = m_number;
        }
    }

}
