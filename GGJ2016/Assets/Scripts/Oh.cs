using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public static class OhVec {
	// Vector Utility Methods. (static only)

	public static Vector3 SetX(Vector3 vector, float x) {
		return new Vector3(x, vector.y, vector.z);
	}

	public static Vector3 SetY(Vector3 vector, float y) {
		return new Vector3(vector.x, y, vector.z);
	}

	public static Vector3 SetZ(Vector3 vector, float z) {
		return new Vector3(vector.x, vector.y, z);
	}

	public static Vector3 SetXY(Vector3 vector, float x, float y) {
		return new Vector3(x, y, vector.z);
	}

	public static Vector3 SetXZ(Vector3 vector, float x, float z) {
		return new Vector3(x, vector.y, z);
	}

	public static Vector3 SetYZ(Vector3 vector, float y, float z) {
		return new Vector3(vector.x, y, z);
	}

	public static Vector3 FlipX(Vector3 vector) {
		return new Vector3(-vector.x, vector.y, vector.z);
	}

	public static Vector3 FlipY(Vector3 vector) {
		return new Vector3(vector.x, -vector.y, vector.z);
	}

	public static Vector3 FlipZ(Vector3 vector) {
		return new Vector3(vector.x, vector.y, -vector.z);
	}

	public static Vector3 FlipXY(Vector3 vector) {
		return new Vector3(-vector.x, -vector.y, vector.z);
	}

	public static Vector3 FlipXZ(Vector3 vector) {
		return new Vector3(-vector.x, vector.y, -vector.z);
	}

	public static Vector3 FlipYZ(Vector3 vector) {
		return new Vector3(vector.x, -vector.y, -vector.z);
	}

	public static Vector3 Flip(Vector3 vector) {
		return new Vector3(-vector.x, -vector.y, -vector.z);
	}

	public static Vector3 Scale(Vector3 vector, float scale) {
		return new Vector3(vector.x*scale, vector.y*scale, vector.z*scale);
	}

	public static Vector3 ScaleX(Vector3 vector, float scale) {
		return new Vector3(vector.x*scale, vector.y, vector.z);
	}

	public static Vector3 ScaleY(Vector3 vector, float scale) {
		return new Vector3(vector.x, vector.y*scale, vector.z);
	}

	public static Vector3 ScaleZ(Vector3 vector, float scale) {
		return new Vector3(vector.x, vector.y, vector.z*scale);
	}

	public static Vector3 ScaleXY(Vector3 vector, float scale) {
		return new Vector3(vector.x*scale, vector.y*scale, vector.z);
	}

	public static Vector3 ScaleXZ(Vector3 vector, float scale) {
		return new Vector3(vector.x*scale, vector.y, vector.z*scale);
	}

	public static Vector3 ScaleYZ(Vector3 vector, float scale) {
		return new Vector3(vector.x, vector.y*scale, vector.z*scale);
	}

	public static Vector2 toVector2(Vector3 vector) {
		return new Vector2(vector.x, vector.y);
	}

	public static Vector3 toVector3(Vector2 vector, float z) {
		return new Vector3(vector.x, vector.y, z);
	}

	public static Vector2 SetX(Vector2 vector, float x) {
		return new Vector2(x, vector.y);
	}

	public static Vector2 SetY(Vector2 vector, float y) {
		return new Vector2(vector.x, y);
	}

	public static Vector2 FlipX(Vector2 vector) {
		return new Vector2(-vector.x, vector.y);
	}

	public static Vector2 FlipY(Vector2 vector) {
		return new Vector2(vector.x, -vector.y);
	}

	public static Vector2 Flip(Vector2 vector) {
		return new Vector2(-vector.x, -vector.y);
	}

	public static Vector2 ScaleX(Vector2 vector, float scale) {
		return new Vector2(vector.x*scale, vector.y);
	}

	public static Vector2 ScaleY(Vector2 vector, float scale) {
		return new Vector2(vector.x, vector.y*scale);
	}

	public static Vector2 ScaleXY(Vector2 vector, float scale) {
		return new Vector2(vector.x*scale, vector.y*scale);
	}

	public static float Distance2D(Vector3 vector1, Vector3 vector2) {
		float xdiff = vector1.x - vector2.x;
		float ydiff = vector1.y - vector2.y;

		return Mathf.Sqrt((xdiff*xdiff)+(ydiff*ydiff));
	}

	public static float Distance2D(Vector2 vector1, Vector2 vector2) {
		float xdiff = vector1.x - vector2.x;
		float ydiff = vector1.y - vector2.y;

		return Mathf.Sqrt((xdiff*xdiff)+(ydiff*ydiff));
	}

	public static float SquareDistance2D(Vector3 vector1, Vector3 vector2) {
		float xdiff = vector1.x - vector2.x;
		float ydiff = vector1.y - vector2.y;

		return (xdiff*xdiff)+(ydiff*ydiff);
	}
}


public static class OhColor {
	// Color Utility Methods. (static only)

	public static Color SetAlpha(Color color, float alpha) {
		return new Color(color.r, color.g, color.b, alpha);
	}

	public static Color SetRed(Color color, float red) {
		return new Color(red, color.g, color.b, color.a);
	}

	public static Color SetGreen(Color color, float green) {
		return new Color(color.r, green, color.b, color.a);
	}

	public static Color SetBlue(Color color, float blue) {
		return new Color(color.r, color.g, blue, color.a);
	}

	public static Color MultiplyRGB(Color color, float factor) {
		return new Color(color.r * factor, color.g * factor, color.b * factor, color.a);
	}
}


public static class OhScreen {
	// NOTE: WORKS ONLY IF YOU DONT CHANGE CAMERA OR SCENE!
	// NOTE 2: ONLY WORKS WITH 2D CAMERA

	private static float yExtent = Camera.main.orthographicSize; // In World Coordinates
	private static float xExtent = Camera.main.orthographicSize*Screen.width/Screen.height; // In World Coordinates.
	private static float ySize = yExtent*2;
	private static float xSize = xExtent*2;
	private static float screenHalfWidth = Screen.width/2;
	private static float screenHalfHeight = Screen.height/2;
	private static Camera camera = Camera.main;

	public static float YExtent {get{return yExtent;}}
	public static float XExtent {get{return xExtent;}}
	public static float YSize {get{return ySize;}}
	public static float XSize {get{return xSize;}}

	public static float CameraRight() {
		return camera.transform.position.x + xExtent;
	}
	public static float CameraLeft() {
		return camera.transform.position.x - xExtent;
	}
	public static float CameraUp() {
		return camera.transform.position.y + yExtent;
	}
	public static float CameraDown() {
		return camera.transform.position.y - yExtent;
	}

	public static Vector2 CameraTopLeft() {
		return new Vector2(CameraLeft(), CameraUp());
	}

	public static Vector2 CameraTopRight() {
		return new Vector2(CameraRight(), CameraUp());
	}

	public static Vector2 CameraBottomLeft() {
		return new Vector2(CameraLeft(), CameraDown());
	}

	public static Vector2 CameraBottomRight() {
		return new Vector2(CameraRight(), CameraDown());
	}

	public static Vector3 ScreenToWorldPoint(Vector3 position, float setZ) {
		Vector3 point = camera.ScreenToWorldPoint(position);
		return new Vector3(point.x, point.y, setZ);
	}

	/*public static Vector2 WorldToScreenPoint(Vector3 position) {
		return OhVec.toVector2(camera.WorldToScreenPoint(position));
	}*/

	public static Vector2 WorldToScreenPoint(Vector3 position) {
		return new Vector2 ( (position.x - camera.transform.position.x)*Screen.height/ySize + screenHalfWidth,
			(-position.y + camera.transform.position.y)*Screen.height/ySize + screenHalfHeight
		);
	}

	public static float ToWorldLength(float screenLength) {
		return screenLength*ySize/Screen.height;
	}

	public static float ToScreenLength(float worldLength) {
		return worldLength*Screen.height/ySize;
	}

	public static Vector3 ToWorldLength2D(Vector3 vector) {
		return new Vector3(ToWorldLength(vector.x), ToWorldLength(vector.y), vector.z);
	}

	public static Vector3 ToScreenLength2D(Vector3 vector) {
		return new Vector3(ToScreenLength(vector.x), ToScreenLength(vector.y), vector.z);
	}
}

public static class OhTest {

	private static bool assertionsOn = false;
	public delegate bool DelBool();
	public delegate object DelObject();

	public static void turnOnAssertions() {
		assertionsOn = true;
	}

	public static void AssertTrue(DelBool statement) {
		if (!assertionsOn) return;
		if (statement() == false) {
			throw new UnityException("Assertion failure: expected true");
		}
	}

	public static void AssertTrue(bool statement) {
		if (!assertionsOn) return;
		if (!statement) {
			throw new UnityException("Assertion failure: expected true");
		}
	}

	public static void AssertFalse(DelBool statement) {
		if (!assertionsOn) return;
		if (statement() == true) {
			throw new UnityException("Assertion failure: expected false");
		}
	}

	public static void AssertEqual(DelObject object1, DelObject object2) {
		if (!assertionsOn) return;
		object obj1 = object1();
		object obj2 = object2();
		if (!obj1.Equals(obj2)) {
			throw new UnityException("Assertion failure: expected " + obj1.ToString() +
				", actual " + obj2.ToString());
		}
	}
}

public static class OhUtil {

	public static string LinkedListToString<T>(LinkedList<T> list) {
		StringBuilder sb = new StringBuilder();
		var current = list.First;
		string separator = "[";
		while (current != null) {
			sb.Append(separator);
			sb.Append(current.Value);

			separator = ", ";
			current = current.Next;
		}
		sb.Append("]");
		return sb.ToString();
	}

}