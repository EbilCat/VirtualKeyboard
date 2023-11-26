using CoreDev.Framework;
using CoreDev.Observable;
using UnityEngine;

public class VirtualKeyboardDO : TransformDO
{
    private OBool button1; public OBool Button1 => button1;
    private OBool button2; public OBool Button2 => button2;
    private OBool button3; public OBool Button3 => button3;
    private OBool button4; public OBool Button4 => button4;
    private OBool button5; public OBool Button5 => button5;
    private OBool button6; public OBool Button6 => button6;
    private OBool button7; public OBool Button7 => button7;
    private OBool button8; public OBool Button8 => button8;
    private OBool button9; public OBool Button9 => button9;
    private OBool button0; public OBool Button0 => button0;
    private OBool buttonBackSpace; public OBool ButtonBackSpace => buttonBackSpace;

    private OBool buttonA; public OBool ButtonA => buttonA;
    private OBool buttonB; public OBool ButtonB => buttonB;
    private OBool buttonC; public OBool ButtonC => buttonC;
    private OBool buttonD; public OBool ButtonD => buttonD;
    private OBool buttonE; public OBool ButtonE => buttonE;
    private OBool buttonF; public OBool ButtonF => buttonF;
    private OBool buttonG; public OBool ButtonG => buttonG;
    private OBool buttonH; public OBool ButtonH => buttonH;
    private OBool buttonI; public OBool ButtonI => buttonI;
    private OBool buttonJ; public OBool ButtonJ => buttonJ;
    private OBool buttonK; public OBool ButtonK => buttonK;
    private OBool buttonL; public OBool ButtonL => buttonL;
    private OBool buttonM; public OBool ButtonM => buttonM;
    private OBool buttonN; public OBool ButtonN => buttonN;
    private OBool buttonO; public OBool ButtonO => buttonO;
    private OBool buttonP; public OBool ButtonP => buttonP;
    private OBool buttonQ; public OBool ButtonQ => buttonQ;
    private OBool buttonR; public OBool ButtonR => buttonR;
    private OBool buttonS; public OBool ButtonS => buttonS;
    private OBool buttonT; public OBool ButtonT => buttonT;
    private OBool buttonU; public OBool ButtonU => buttonU;
    private OBool buttonV; public OBool ButtonV => buttonV;
    private OBool buttonW; public OBool ButtonW => buttonW;
    private OBool buttonX; public OBool ButtonX => buttonX;
    private OBool buttonY; public OBool ButtonY => buttonY;
    private OBool buttonZ; public OBool ButtonZ => buttonZ;

    private OBool buttonEnter; public OBool ButtonEnter => buttonEnter;
    private OBool buttonSpace; public OBool ButtonSpace => buttonSpace;
    
    protected override void Awake()
    {
        base.Awake();
        this.button1 = new OBool(false, this);
        this.button2 = new OBool(false, this);
        this.button3 = new OBool(false, this);
        this.button4 = new OBool(false, this);
        this.button5 = new OBool(false, this);
        this.button6 = new OBool(false, this);
        this.button7 = new OBool(false, this);
        this.button8 = new OBool(false, this);
        this.button9 = new OBool(false, this);
        this.button0 = new OBool(false, this);
        this.buttonBackSpace = new OBool(false, this);

        this.buttonA = new OBool(false, this);
        this.buttonB = new OBool(false, this);
        this.buttonC = new OBool(false, this);
        this.buttonD = new OBool(false, this);
        this.buttonE = new OBool(false, this);
        this.buttonF = new OBool(false, this);
        this.buttonG = new OBool(false, this);
        this.buttonH = new OBool(false, this);
        this.buttonI = new OBool(false, this);
        this.buttonJ = new OBool(false, this);
        this.buttonK = new OBool(false, this);
        this.buttonL = new OBool(false, this);
        this.buttonM = new OBool(false, this);
        this.buttonN = new OBool(false, this);
        this.buttonO = new OBool(false, this);
        this.buttonP = new OBool(false, this);
        this.buttonQ = new OBool(false, this);
        this.buttonR = new OBool(false, this);
        this.buttonS = new OBool(false, this);
        this.buttonT = new OBool(false, this);
        this.buttonU = new OBool(false, this);
        this.buttonV = new OBool(false, this);
        this.buttonW = new OBool(false, this);
        this.buttonX = new OBool(false, this);
        this.buttonY = new OBool(false, this);
        this.buttonZ = new OBool(false, this);

        this.buttonEnter = new OBool(false, this);
        this.buttonSpace = new OBool(false, this);
    }

    public OBool GetObservable(KeyCode keyCode)
    {
        if(keyCode == KeyCode.Keypad1) { return button1; }
        if(keyCode == KeyCode.Keypad2) { return button2; }
        if(keyCode == KeyCode.Keypad3) { return button3; }
        if(keyCode == KeyCode.Keypad4) { return button4; }
        if(keyCode == KeyCode.Keypad5) { return button5; }
        if(keyCode == KeyCode.Keypad6) { return button6; }
        if(keyCode == KeyCode.Keypad7) { return button7; }
        if(keyCode == KeyCode.Keypad8) { return button8; }
        if(keyCode == KeyCode.Keypad9) { return button9; }
        if(keyCode == KeyCode.Keypad0) { return button0; }
        if(keyCode == KeyCode.Backspace) { return buttonBackSpace; }

        if (keyCode == KeyCode.A) { return buttonA; }
        if (keyCode == KeyCode.B) { return buttonB; }
        if (keyCode == KeyCode.C) { return buttonC; }
        if (keyCode == KeyCode.D) { return buttonD; }
        if (keyCode == KeyCode.E) { return buttonE; }
        if (keyCode == KeyCode.F) { return buttonF; }
        if (keyCode == KeyCode.G) { return buttonG; }
        if (keyCode == KeyCode.H) { return buttonH; }
        if (keyCode == KeyCode.I) { return buttonI; }
        if (keyCode == KeyCode.J) { return buttonJ; }
        if (keyCode == KeyCode.K) { return buttonK; }
        if (keyCode == KeyCode.L) { return buttonL; }
        if (keyCode == KeyCode.M) { return buttonM; }
        if (keyCode == KeyCode.N) { return buttonN; }
        if (keyCode == KeyCode.O) { return buttonO; }
        if (keyCode == KeyCode.P) { return buttonP; }
        if (keyCode == KeyCode.Q) { return buttonQ; }
        if (keyCode == KeyCode.R) { return buttonR; }
        if (keyCode == KeyCode.S) { return buttonS; }
        if (keyCode == KeyCode.T) { return buttonT; }
        if (keyCode == KeyCode.U) { return buttonU; }
        if (keyCode == KeyCode.V) { return buttonV; }
        if (keyCode == KeyCode.W) { return buttonW; }
        if (keyCode == KeyCode.X) { return buttonX; }
        if (keyCode == KeyCode.Y) { return buttonY; }
        if (keyCode == KeyCode.Z) { return buttonZ; }

        if(keyCode == KeyCode.Return) { return buttonEnter; }
        if(keyCode == KeyCode.Space) { return buttonSpace; }

        return null;
    }
}