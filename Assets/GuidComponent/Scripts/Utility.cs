using UnityEngine;
using System.Linq;

namespace GuidBasedReference
{
    /// <summary>
    /// Reference: http://baba-s.hatenablog.com/entry/2014/12/08/225548
    /// </summary>
    public static class GameObjectUtils
    {
        public static GameObject[] FindAllInScene()
        {
            var objects = Resources.FindObjectsOfTypeAll( typeof( GameObject ) ) as GameObject[];
            return objects
                .Where( c => !c.hideFlags.ContainsAny( HideFlags.NotEditable, HideFlags.HideAndDontSave ) )
                .ToArray();
        }

        public static GameObject[] FindAllNotInScene()
        {
            var objects = Resources.FindObjectsOfTypeAll( typeof( GameObject ) ) as GameObject[];
            return objects
                .Where( c => c.hideFlags.ContainsAny( HideFlags.NotEditable, HideFlags.HideAndDontSave ) )
                .ToArray();
        }
    }

    /// <summary>
    /// Reference: http://baba-s.hatenablog.com/entry/2014/07/23/120735
    /// </summary>
    public static class GenericExtensions
    {
        public static bool ContainsAny<T>(this T self, params T[] values)
        {
            return values.Any(c => c.Equals(self));
        }
    }
}