using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Yipeee;

namespace Core
{
    public static class Interface
    {
        public static bool IsOnConsole(GameObject console)
        {
            Vector2 position = console.transform.position;
            return ShouldBeHighlighted(position, position + console.GetComponent<Debug_Console>().size) && console.activeSelf;
        }

        private static bool ShouldBeHighlighted(Vector2 firstPoint, Vector2 secondPoint)
        {
            Vector2 mousPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            if (mousPos.x >= firstPoint.x && mousPos.x <= secondPoint.x)
            {
                if (mousPos.y >= firstPoint.y && mousPos.y <= secondPoint.y)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
