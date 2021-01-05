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

//assumes variable name provided is safe
function getObjectVariable(%obj, %varname)
{
	%firstChar = getSubStr(%varname, 0, 1);
	%rest = getSubStr(%varname, 1, 100);

	if (strPos("ABCDEFGHIJKLMNOPQRSTUVWXYZ", %firstChar) < 0)
	{
		switch$ (%firstChar)
		{
			case "a": return %obj.a[%rest];
			case "b": return %obj.b[%rest];
			case "c": return %obj.c[%rest];
			case "d": return %obj.d[%rest];
			case "e": return %obj.e[%rest];
			case "f": return %obj.f[%rest];
			case "g": return %obj.g[%rest];
			case "h": return %obj.h[%rest];
			case "i": return %obj.i[%rest];
			case "j": return %obj.j[%rest];
			case "k": return %obj.k[%rest];
			case "l": return %obj.l[%rest];
			case "m": return %obj.m[%rest];
			case "n": return %obj.n[%rest];
			case "o": return %obj.o[%rest];
			case "p": return %obj.p[%rest];
			case "q": return %obj.q[%rest];
			case "r": return %obj.r[%rest];
			case "s": return %obj.s[%rest];
			case "t": return %obj.t[%rest];
			case "u": return %obj.u[%rest];
			case "v": return %obj.v[%rest];
			case "w": return %obj.w[%rest];
			case "x": return %obj.x[%rest];
			case "y": return %obj.y[%rest];
			case "z": return %obj.z[%rest];
			case "_": return %obj._[%rest];
		}
	}
	else
	{
		switch$ (%firstChar)
		{
			case "A": return %obj.A[%rest];
			case "B": return %obj.B[%rest];
			case "C": return %obj.C[%rest];
			case "D": return %obj.D[%rest];
			case "E": return %obj.E[%rest];
			case "F": return %obj.F[%rest];
			case "G": return %obj.G[%rest];
			case "H": return %obj.H[%rest];
			case "I": return %obj.I[%rest];
			case "J": return %obj.J[%rest];
			case "K": return %obj.K[%rest];
			case "L": return %obj.L[%rest];
			case "M": return %obj.M[%rest];
			case "N": return %obj.N[%rest];
			case "O": return %obj.O[%rest];
			case "P": return %obj.P[%rest];
			case "Q": return %obj.Q[%rest];
			case "R": return %obj.R[%rest];
			case "S": return %obj.S[%rest];
			case "T": return %obj.T[%rest];
			case "U": return %obj.U[%rest];
			case "V": return %obj.V[%rest];
			case "W": return %obj.W[%rest];
			case "X": return %obj.X[%rest];
			case "Y": return %obj.Y[%rest];
			case "Z": return %obj.Z[%rest];
			case "_": return %obj._[%rest];
		}
	}
	return "";
}