//assumes variable name provided is safe
function setObjectVariable(%obj, %varname, %value)
{
	%firstChar = getSubStr(%varname, 0, 1);
	%rest = getSubStr(%varname, 1, 100);

	if (strPos("ABCDEFGHIJKLMNOPQRSTUVWXYZ", %firstChar) < 0)
	{
		switch$ (%firstChar)
		{
			case "a": %obj.a[%rest] = %value;
			case "b": %obj.b[%rest] = %value;
			case "c": %obj.c[%rest] = %value;
			case "d": %obj.d[%rest] = %value;
			case "e": %obj.e[%rest] = %value;
			case "f": %obj.f[%rest] = %value;
			case "g": %obj.g[%rest] = %value;
			case "h": %obj.h[%rest] = %value;
			case "i": %obj.i[%rest] = %value;
			case "j": %obj.j[%rest] = %value;
			case "k": %obj.k[%rest] = %value;
			case "l": %obj.l[%rest] = %value;
			case "m": %obj.m[%rest] = %value;
			case "n": %obj.n[%rest] = %value;
			case "o": %obj.o[%rest] = %value;
			case "p": %obj.p[%rest] = %value;
			case "q": %obj.q[%rest] = %value;
			case "r": %obj.r[%rest] = %value;
			case "s": %obj.s[%rest] = %value;
			case "t": %obj.t[%rest] = %value;
			case "u": %obj.u[%rest] = %value;
			case "v": %obj.v[%rest] = %value;
			case "w": %obj.w[%rest] = %value;
			case "x": %obj.x[%rest] = %value;
			case "y": %obj.y[%rest] = %value;
			case "z": %obj.z[%rest] = %value;
			case "_": %obj._[%rest] = %value;
		}
	}
	else
	{
		switch$ (%firstChar)
		{
			case "A": %obj.A[%rest] = %value;
			case "B": %obj.B[%rest] = %value;
			case "C": %obj.C[%rest] = %value;
			case "D": %obj.D[%rest] = %value;
			case "E": %obj.E[%rest] = %value;
			case "F": %obj.F[%rest] = %value;
			case "G": %obj.G[%rest] = %value;
			case "H": %obj.H[%rest] = %value;
			case "I": %obj.I[%rest] = %value;
			case "J": %obj.J[%rest] = %value;
			case "K": %obj.K[%rest] = %value;
			case "L": %obj.L[%rest] = %value;
			case "M": %obj.M[%rest] = %value;
			case "N": %obj.N[%rest] = %value;
			case "O": %obj.O[%rest] = %value;
			case "P": %obj.P[%rest] = %value;
			case "Q": %obj.Q[%rest] = %value;
			case "R": %obj.R[%rest] = %value;
			case "S": %obj.S[%rest] = %value;
			case "T": %obj.T[%rest] = %value;
			case "U": %obj.U[%rest] = %value;
			case "V": %obj.V[%rest] = %value;
			case "W": %obj.W[%rest] = %value;
			case "X": %obj.X[%rest] = %value;
			case "Y": %obj.Y[%rest] = %value;
			case "Z": %obj.Z[%rest] = %value;
			case "_": %obj._[%rest] = %value;
		}
	}
}