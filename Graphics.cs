using System;
using System.Drawing;
using System.Text;

public
class spacial_t {
	public float x;
	public float y;
	public float z;

	public
	spacial_t() { ; }

	public
	spacial_t(float x, float y, float z) { this.x = x; this.y = y; this.z = z; }

	public
	spacial_t(int x, int y, int z) : this((float)x, (float)y, (float)z) { ; }

	public
	override
	string ToString() {
		return $"Point{{{x}, {y}, {z}}}, ";
	}

	public
	static
	spacial_t operator*(spacial_t s, Matrix m) {
		float divisor = s.x * m.m[0, 3] + s.y * m.m[1, 3] + s.z * m.m[2, 3] + m.m[3, 3];
		divisor = (divisor == 0) ? 1 : divisor;

		return new spacial_t(
			(s.x * m.m[0, 0] + s.y * m.m[1, 0] + s.z * m.m[2, 0] + m.m[3, 0]) / divisor,
            (s.x * m.m[0, 1] + s.y * m.m[1, 1] + s.z * m.m[2, 1] + m.m[3, 1]) / divisor,
            (s.x * m.m[0, 2] + s.y * m.m[1, 2] + s.z * m.m[2, 2] + m.m[3, 2]) / divisor
		);
	}

	public
	static
	spacial_t normalize(spacial_t p1, spacial_t p2) {
			return new spacial_t(
				p1.x - p2.x,
				p1.y - p2.y,
				p1.z - p2.z
			);
	}
}

public
class triangle {
	public spacial_t[] p = new spacial_t[3];

    public
	spacial_t this[int index] {
        get { return p[index]; }
        set { p[index] = value; }
    }

    public
	triangle(spacial_t p1, spacial_t p2, spacial_t p3) {
        p = new spacial_t[] { p1, p2, p3 };
    }

    public
	triangle(float x1, float y1, float z1,
			float x2, float y2, float z2,
			float x3, float y3, float z3) {
        p = new spacial_t[3];
        p[0] = new spacial_t { x = x1, y = y1, z = z1 };
        p[1] = new spacial_t { x = x2, y = y2, z = z2 };
        p[2] = new spacial_t { x = x3, y = y3, z = z3 };
    }

    public triangle clone() {
        return new triangle(p[0].x, p[0].y, p[0].z,
                             p[1].x, p[1].y, p[1].z,
                             p[2].x, p[2].y, p[2].z);
    }

   public
   override
   string ToString() {
        StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("Triangle{");
		for (int i = 0; i < 3; i++) {
                stringBuilder.Append(p[i].ToString());
                stringBuilder.Append(" ");
        }
        stringBuilder.Append("}");
        return stringBuilder.ToString();
   }

	public
	static
	triangle operator+(triangle t, int i) {
		return new triangle(
			new spacial_t (t[0].x, t[0].y, t[0].z + i),
			new spacial_t (t[1].x, t[1].y, t[1].z + i),
			new spacial_t (t[2].x, t[2].y, t[2].z + i)
		);
	}

    public
	void draw(Graphics g, Pen pen) {
		for (int i = 0; i < 3; i++) {
				Console.Write(p[i]);
				g.DrawLine(pen, p[i].x, p[i].y, p[(i+1) % 3].x, p[(i+1) % 3].y);
		}
		Console.WriteLine("");
    }
};

public
class Matrix {
	public float[,] m = new float[4, 4];

    public Matrix(float[] row1, float[] row2, float[] row3, float[] row4) {
        if (row1.Length != 4 || row2.Length != 4 || row3.Length != 4 || row4.Length != 4) {
            throw new ArgumentException("Each row array must have exactly 4 elements.");
        }

        m = new float[4, 4]
        {
            { row1[0], row1[1], row1[2], row1[3] },
            { row2[0], row2[1], row2[2], row2[3] },
            { row3[0], row3[1], row3[2], row3[3] },
            { row4[0], row4[1], row4[2], row4[3] }
        };
    }

    public Matrix(float m11, float m12, float m13, float m14,
                  float m21, float m22, float m23, float m24,
                  float m31, float m32, float m33, float m34,
                  float m41, float m42, float m43, float m44) {
        m = new float[4, 4]
        {
            { m11, m12, m13, m14 },
            { m21, m22, m23, m24 },
            { m31, m32, m33, m34 },
            { m41, m42, m43, m44 }
        };
    }

   public
   override
   string ToString() {
        StringBuilder stringBuilder = new StringBuilder();

        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < 4; j++) {
                stringBuilder.Append(m[i, j]);
                stringBuilder.Append(" ");
            }
            stringBuilder.AppendLine();
        }
        return stringBuilder.ToString();
    }

	public
	static
	Matrix new_projection_matrix(float aspect_ratio, float fov) {
		const float n = 0.1f;
		const float f = 1000.0f;
		float i = (float)(1.0f / Math.Tan(fov * 0.5f / 180.0f * 3.14159f));
		Matrix r = new Matrix(
			aspect_ratio * i, 0, 0, 0,
			0, i, 0, 0,
			0, 0, (float)f / (float)(f - n), 1,
			0, 0, (float)(-f * n) / (float)(f - n), 0
		);
		Console.WriteLine(r);
		return r;
	}
};
