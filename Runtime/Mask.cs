namespace CrazyPanda.UnityCore.Utils
{
    public static class Mask
    {
        public static bool CheckContainsInMask( int mask, int value )
        {
            return ( mask | value ) == mask;
        }

        public static bool CheckContainsInMask( float mask, int value )
        {
            return ( ( int ) mask | value ) == ( int ) mask;
        }


        public static int GetOrMask( int mask, int value )
        {
            return mask | value;
        }

        public static bool CheckMasksIntersection( int mask1, int mask2 )
        {
            return ( mask1 & mask2 ) > 0;
        }

        public static bool CheckMasksIntersection( uint mask1, uint mask2 )
        {
            return ( mask1 & mask2 ) > 0;
        }
    }
}
