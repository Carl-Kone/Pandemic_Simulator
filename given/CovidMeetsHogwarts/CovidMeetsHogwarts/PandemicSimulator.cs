using System;
using System.Collections.Generic;

namespace CovidMeetsHogwarts
{
    public class PandemicSimulator
    {
        public static List<Human> infectious;
        
        /// <summary>
        /// initialize pandemic by infecting a random human by the corona virus in given location.
        /// </summary>
        /// <param name="location">location where the pandemic is simulated</param>
        public static void InitializePandemic(Location location)
        {
            infectious = new List<Human>();
            Random rand = new Random();
            Virus covid = new Virus("Covid-19", 0.6, 3, 14);

            List<Human> listHuman = location.GetHumans();
            int randIndex = rand.Next(listHuman.Count);
            Human randHuman = listHuman[randIndex];
            randHuman.SetVirus(covid);
            randHuman.SetSir(Human.SIR.INFECTIOUS);
            
            infectious.Add(randHuman);
        }

        /// <summary>
        /// move/travel given human to a neighboring spot according to their
        /// travelling rate.
        /// </summary>
        /// <param name="human">human to move (or not)</param>
        static void MoveAround(Human human)
        {
            Random rand = new Random();
            int r = rand.Next(100);
            double x = human.GetTravellingRate() * 100;
            if (r < x)
            {
                Node oldspot = human.GetCurrentSpot();
                List<Node> listNeighboors = oldspot.GetNeighbors();
                int randnum = rand.Next(listNeighboors.Count);
                Node newSpot = listNeighboors[randnum];
                human.SetCurrentSpot(newSpot);

                oldspot.GetHumans().Remove(human);
                newSpot.GetHumans().Add(human);
            }
        }

        /// <summary>
        /// try to infect susceptible humans at the transmitter's spot.
        /// the following factors are taken into account:
        ///     - the virus' infection range
        ///     - the virus's transmission rate
        ///     - the average hygiene between the transmitter and the susceptible human
        ///     - the distance between the transmitter and the susceptible human (also average of social distance)
        /// </summary>
        /// <param name="transmitter">the human carrying the virus</param>
        /// <param name="justGotInfected">the list of humans to update when someone gets infected</param>
        static void InfectOthers(Human transmitter, List<Human> justGotInfected)
        {
           
            Random rand = new Random();
            int numSusceptibles = transmitter.GetCurrentSpot().GetHumans().Count - 1; // to exclude the transmitter

            int i = transmitter.GetVirus().GetInfectionRange() > numSusceptibles
                ? numSusceptibles
                : transmitter.GetVirus().GetInfectionRange();

            foreach (var human in transmitter.GetCurrentSpot().GetHumans())
            {
                if (i > 0)
                {
                    double x = 0.6 * 100;
                    int r = rand.Next(100);
                    if (r < x)
                    {
                        if (human == transmitter || human.GetSir() != Human.SIR.SUSCEPTIBLE)
                        {
                            if (human != transmitter)
                                i--;
                        }
                        else
                        {
                            double averageHygiene = (human.GetHygiene() + transmitter.GetHygiene()) / 2;
                            double averageSocialDistance=
                                (human.GetSocialDistance() + transmitter.GetSocialDistance()) / 2;

                            if ( r >= (averageHygiene * 100) && r >= (averageSocialDistance * 100))
                            {
                                human.SetSir(Human.SIR.INFECTIOUS);
                                human.SetVirus(new Virus("Covid-19",0.6,3,14));
                                
                                justGotInfected.Add(human);
                            }
                            else
                            {
                                i--;
                            }
                        }
                    }
                    else
                    {
                        i--;
                    }
                }
                else
                {
                    break;
                }
            }

        }
        
        /// <summary>
        /// update pandemic by a unit of time at given location.
        ///     - infectious humans will infect around them
        ///     - some of the infectious humans will heal/die if enough days have passed
        ///     - some humans will travel to a neighboring spot
        /// the infectious list should be updated as well
        /// </summary>
        /// <param name="location">location where the pandemic is simulated</param>
        /// <returns>return number of infectious humans at this round</returns>
        public static int UpdatePandemic(Location location)
        {
            List<Human> justInfected = new List<Human>();
            foreach (var human in location.GetHumans())
            {
                if (human.GetVirus() != null)
                {
                     human.GetVirus().SetLifetime(human.GetVirus().GetLifetime() - 1);
                }
                else
                {
                    continue;
                }
                
                if (human.GetVirus().GetLifetime() > 0)
                    InfectOthers(human,justInfected);
                else
                {
                    human.SetSir(Human.SIR.REMOVED);
                    human.SetVirus(null);

                }
            }

            List<Human> copie = new List<Human>(infectious);
            foreach (var human in copie)
            {
                if (human.GetSir() == Human.SIR.REMOVED)
                { 
                    infectious.Remove(human);
                }
            }

            foreach (var human in justInfected)
            {
                infectious.Add(human);
            }

            foreach (var human in location.GetHumans())
            {
                MoveAround(human);
            }

            return infectious.Count;

        }
    }
}