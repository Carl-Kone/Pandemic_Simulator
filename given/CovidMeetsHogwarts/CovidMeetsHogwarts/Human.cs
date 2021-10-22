using System;

namespace CovidMeetsHogwarts
{
    public class Human
    {
        public enum SIR
        {
            SUSCEPTIBLE,
            INFECTIOUS,
            REMOVED
        }

        // Attributes
        public static int populationCount = 0;
        
        public const double GLOBAL_HYGIENE = 0.5d;
        public const double GLOBAL_SOCIAL_DISTANCE = 0.5d;
        public const double GLOBAL_TRAVELLING_RATE = 0.5d;
        
        public int id;
        
        private SIR sir;
        private Virus virus;
        private Node currentSpot;
        
        private double hygiene; // from 0d to 1d
        // 0: dirty, stinks - 1: Mr. Propre, washes hands regularly
        private double socialDistance; // from 0d to 1d
        // 0: has no concept of personal space - 1: respects social distance, stands on stickers
        private double travellingRate; // from 0d to 1d
        // 0: stays at home - 1: Mr/Mrs/Mx International

        // Methods
        // - constructor
        public Human(double hygiene, double socialDistance,
            double travellingRate)
        {
            this.hygiene = hygiene;
            this.socialDistance = socialDistance;
            this.travellingRate = travellingRate;

            sir = SIR.SUSCEPTIBLE;
            virus = null;
            id = populationCount;
            populationCount++;
            currentSpot = null;
        }
        
        // - getters and setters
        public SIR GetSir()
        {
            return sir;    
        }

        public void SetSir(SIR value)
        {
            sir = value;
        }

        public Virus GetVirus()
        {
            return virus;
        }

        public void SetVirus(Virus value)
        {
            virus = value;
        }

        public Node GetCurrentSpot()
        {
            return currentSpot;
        }

        public void SetCurrentSpot(Node value)
        {
            currentSpot = value;
        }

        public double GetHygiene()
        {
            return hygiene;
        }

        public double GetSocialDistance()
        {
            return socialDistance;
        }

        public double GetTravellingRate()
        {
            return travellingRate;
        }

        // - == and != operators overload
        public static bool operator== (Human human1, Human human2)
        {
            if (human1 is null)
            {
                if (human2 is null)
                    return true;
                else
                {
                    return false;
                }
            }
            else
            {
                if (human2 is null)
                    return false;
                else
                {
                    return human1.id == human2.id;
                }
            }
        }

        public static bool operator!= (Human human1, Human human2)
        {
            if (human1 is null)
            {
                if (human2 is null)
                    return false;
                else
                {
                    return true;
                }
            }
            else
            {
                if (human2 is null)
                    return true;
                else
                {
                    return human1.id != human2.id;
                }
            }
        }
    }
}