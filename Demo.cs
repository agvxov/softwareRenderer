using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

using Mesh = System.Collections.Generic.List<triangle>;

public
static
class Meshes {
	
	public
	static
	Mesh Cube = new Mesh{
        new triangle(0.0f, 0.0f, 0.0f,    0.0f, 1.0f, 0.0f,    1.0f, 1.0f, 0.0f),
        new triangle(0.0f, 0.0f, 0.0f,    1.0f, 1.0f, 0.0f,    1.0f, 0.0f, 0.0f),

        new triangle(1.0f, 0.0f, 0.0f,    1.0f, 1.0f, 0.0f,    1.0f, 1.0f, 1.0f),
        new triangle(1.0f, 0.0f, 0.0f,    1.0f, 1.0f, 1.0f,    1.0f, 0.0f, 1.0f),

        new triangle(1.0f, 0.0f, 1.0f,    1.0f, 1.0f, 1.0f,    0.0f, 1.0f, 1.0f),
        new triangle(1.0f, 0.0f, 1.0f,    0.0f, 1.0f, 1.0f,    0.0f, 0.0f, 1.0f),

        new triangle(0.0f, 0.0f, 1.0f,    0.0f, 1.0f, 1.0f,    0.0f, 1.0f, 0.0f),
        new triangle(0.0f, 0.0f, 1.0f,    0.0f, 1.0f, 0.0f,    0.0f, 0.0f, 0.0f),

        new triangle(0.0f, 1.0f, 0.0f,    0.0f, 1.0f, 1.0f,    1.0f, 1.0f, 1.0f),
        new triangle(0.0f, 1.0f, 0.0f,    1.0f, 1.0f, 1.0f,    1.0f, 1.0f, 0.0f),

        new triangle(1.0f, 0.0f, 1.0f,    0.0f, 0.0f, 1.0f,    0.0f, 0.0f, 0.0f),
        new triangle(1.0f, 0.0f, 1.0f,    0.0f, 0.0f, 0.0f,    1.0f, 0.0f, 0.0f)
    };

	public
	static
	Mesh Möbius(){
		Mesh r = new Mesh();

		const long graduality = 100;
		const long width      = 1;

		spacial_t previous_previous = null;
		spacial_t previous = null;

		/*
		for (int i = 0; i < graduality; i++) {
			spacial_t current = new spacial_t();

			float v = (float)(Math.PI*2 / (double)graduality) * i;
			float u = (((float)2 / (float)graduality) * i) - 1;

			current.x = (1 + (v/(float)2) * (float)Math.Cos(u/(float)2) * (float)Math.Cos(u));
			current.y = (1 + (v/(float)2) * (float)Math.Cos(u/(float)2) * (float)Math.Sin(u));
			current.z = (v/(float)2) * (float)Math.Sin(u/(float)2);

			r.Add(new triangle(previous_previous, previous, current));

			previous_previous = previous;
			previous = current;
		}
		r.Add(new triangle(0, 0, 0, 0, 1, 0, 1, 0, 0));
		r.Add(new triangle(1, 0, 0, 0, 1, 0, 1, 1, 0));
		*/
		float one_v = (float)((Math.PI*2) / (double)graduality);
		float one_u = ((float)2 / (float)graduality);
		for (int i = 0; i < graduality; i++) {
			float v = one_v * i;
			float u = (one_u * i) - 1;

			spacial_t current = new spacial_t();

			current.x = (1 + ((v / 2.0f) * (float)Math.Cos(u / 2.0f) * (float)Math.Cos(u)));
			current.y = (1 + ((v / 2.0f) * (float)Math.Cos(u / 2.0f) * (float)Math.Sin(u)));
			current.z = (v / 2.0f) * (float)Math.Sin(u / 2.0f);

			r.Add(new triangle(
				current,
				previous,
				previous_previous
			));

			previous_previous = previous;
			previous = current;
		}

		return r;
	}

	public
	static
    Dictionary<string, Mesh> dictionary = new Dictionary<string, Mesh>{
		{"Cube", Cube},
//		{"Möbius", Möbius()},
	};
};

public
class Program : Form {
	static int h = 800;
	static int w = 800;
	static float aspect_ratio(){ return (float)h / (float)w; }

	const float FOV = 90.0f;	
                            	
	Pen pen = new Pen(Color.	Green, 1);
                            	
	Matrix projection_matrix = Matrix.new_projection_matrix((float)aspect_ratio(), FOV);

	Timer ticker                = new Timer();
	long frame                  = 0;
	const long frame_graduality = 100;

	bool do_rotate      = true;
	bool is_transparent = true;

	Mesh displayed_object;

	public
	Program() {
		this.Text = "Software Renderer";
		this.Size = new Size(1200, 1200);

		this.displayed_object = Meshes.Cube;

		this.ticker.Interval = 50;
		this.ticker.Tick    += rotate;
		this.ticker.Start();
		
		Button button;

		button = new Button();
			button.Text     = "Rotation";
			button.Location = new System.Drawing.Point(10, 10);
			button.Size     = new System.Drawing.Size(90, 30);
			button.Click   += (object s, EventArgs e) => {
				this.do_rotate = !this.do_rotate;
			};
		this.Controls.Add(button);
		
		button = new Button();
			button.Text     = "Transparency";
			button.Location = new System.Drawing.Point(10, 50);
			button.Size     = new System.Drawing.Size(90, 30);
			button.Click   += (object s, EventArgs e) => {
				this.is_transparent = !this.is_transparent;
				this.fill();
			};
		this.Controls.Add(button);

        ComboBox dropdown = new ComboBox();
            dropdown.Text     = "Object to display";
			dropdown.Location = new System.Drawing.Point(120, 10);
			foreach (var i in Meshes.dictionary) {
				dropdown.Items.Add(i.Key);
			}
            dropdown.SelectedIndexChanged += (object s, EventArgs e) => {
				this.displayed_object = Meshes.dictionary[((ComboBox)s).SelectedItem.ToString()];
				this.fill();
			};
        this.Controls.Add(dropdown);

		this.fill();
	}

	private
	void fill(object s, EventArgs e) {
		this.fill();
	}

	private
	void rotate(object s, EventArgs e) {
		if (do_rotate) {
			this.fill();
			++frame;
		}
	}

	private
	void fill() {
		this.Invalidate();
	}

	protected
	override
	void OnPaint(PaintEventArgs e) {
		Console.ForegroundColor = ConsoleColor.Blue;
		Console.WriteLine("@@@@@@@@@@@@@@@@@@@@@@@@@@ FRAME BEGIN @@@@@@@@@@@@@@@@@@@@@@@@@@");
		Console.ResetColor();

		base.OnPaint(e);
		Graphics g = e.Graphics;

		float xc = (float)(Math.PI * 2 / (double)frame_graduality) * (float)frame;
		float zc = 0.5f;

		Console.WriteLine($"XC: {xc}; ZC {zc} (frame: {frame})");

		/*
		Matrix rotz = new Matrix(
			(float)Math.Cos(zc), (float)Math.Sin(zc), 0, 0,
			-(float)Math.Sin(zc), (float)Math.Cos(zc), 0, 0,
			0, 0, 1, 0,
			0, 0, 0, 1
		);
		*/

		/*
		Matrix rotx = new Matrix(
			1, 0, 0, 0,
			0, (float)Math.Cos(xc), (float)Math.Sin(xc), 0,
			0, -(float)Math.Sin(xc), (float)Math.Cos(xc), 0,
			0, 0, 0, 1
		);
		*/

		Matrix roty = new Matrix(
			(float)Math.Cos(xc), 0, (float)Math.Sin(xc), 0,
			0, 1, 0, 0, 
			-(float)Math.Sin(xc), 0, (float)Math.Cos(xc), 0,
			0, 0, 0, 1
		);

		foreach (var t in displayed_object) {
			var tc = t.clone();
			for (int i = 0; i < 3; i++) {
				/* rotate */
				//tc[i] *= rotz;
				//tc[i] *= rotx;
				tc[i] *= roty;
				/* offset */
				tc[i].y += 0.5f;
				tc[i].z += 3;
				
				/* project */
				tc[i] *= projection_matrix;
				/* scale */
				tc[i].x += 1.0f;
				tc[i].y += 1.0f;
				tc[i].x *= 0.5f * w;
				tc[i].y *= 0.5f * h;
			}

			spacial_t l1 = spacial_t.normalize(tc[1], tc[0]);
			spacial_t l2 = spacial_t.normalize(tc[2], tc[0]);

			spacial_t normal = new spacial_t(
				(l1.y * l2.z) - (l1.z * l2.y),
				(l1.y * l2.x) - (l1.x * l2.z),
				(l1.x * l2.y) - (l1.y * l2.x)
			);
			if (is_transparent || (normal.z < 0)) {
				tc.draw(g, pen);
			}
		}
		Console.ForegroundColor = ConsoleColor.Blue;
		Console.WriteLine("@@@@@@@@@@@@@@@@@@@@@@@@@@  FRAME END  @@@@@@@@@@@@@@@@@@@@@@@@@@");
		Console.ResetColor();
	}

	public
	static
	void Main() {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
		Application.Run(new Program());
	}
}
