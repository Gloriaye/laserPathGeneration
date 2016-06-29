using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClipperLib;

namespace wsconvexdecomposition
{
    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;

    public class Population
    {


        // 保持 路径的 集合 即为种群
        Tour[] tours;
        public Paths oripaths ;
        public IntPoint startPoint ;
        
        // Construct a population
        public Population(int populationSize, Boolean initialise, Paths pgs, IntPoint m_startPoint)
        {
            this.oripaths = pgs;   //保存传入的路劲集合，用来计算适应度，及距离
            this.startPoint = m_startPoint;

            tours = new Tour[populationSize];        //种群路劲集合
            // If we need to initialise a population of tours do so
            if (initialise)
            {
                // Loop and create individuals
                for (int i = 0; i < populationSize; i++)
                {
                    Tour newTour = new Tour(pgs.Count);
                    newTour.generateIndividual();
                    saveTour(i, newTour);
                }
            }
        }

        public Population(int populationSize, Boolean initialise, Paths pgs, IntPoint m_startPoint,Tour greedtour)
        {
            this.oripaths = pgs;   //保存传入的路劲集合，用来计算适应度，及距离
            this.startPoint = m_startPoint;

            tours = new Tour[populationSize];        //种群路劲集合

            int greedseedsize = 15;
            // If we need to initialise a population of tours do so
            if (initialise)
            {
                for (int j = 0; j < greedseedsize; j++)
                {
                    saveTour(j, greedtour);
                }

                // Loop and create individuals
                for (int i = greedseedsize; i < populationSize; i++)
                {
                    Tour newTour = new Tour(pgs.Count);
                    newTour.generateIndividual();
                    saveTour(i, newTour);
                }
            }
        }

        // Saves a tour
        public void saveTour(int index, Tour tour)
        {
            tours[index] = tour;
        }

        // Gets a tour from population
        public Tour getTour(int index)
        {
            return tours[index];
        }

        // 获取当前种群 中 最优的个体
        public Tour getFittest()
        {
            Tour fittest = tours[0];
            fittest.getDistance(oripaths, startPoint);    //更新distance
            // Loop through individuals to find fittest
            for (int i = 1; i < populationSize(); i++)
            {
                getTour(i).getDistance(oripaths, startPoint);   //更新distance
                if (fittest.getFitness() <= getTour(i).getFitness())
                {
                    fittest = getTour(i);
                }
            }
            return fittest;
        }

        // Gets population size
        public int populationSize()
        {
            return tours.Length;
        }
    }
}
