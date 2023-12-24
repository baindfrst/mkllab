#include "pch.h"
#include "mkl.h"
extern "C" _declspec(dllexport) 
int CubicSpline(
	int nX, // число узлов сплайна
	const double* X, // массив узлов сплайна
	int nY, // размерность векторной функции
	const double* Y, // массив заданных значений векторной функции
	double* splineValues)  // массив вычисленных значений сплайна и производных
{
	MKL_INT s_order = DF_PP_CUBIC; // степень кубического сплайна
	MKL_INT s_type = DF_PP_NATURAL; // тип сплайна

	// тип граничных условий
	MKL_INT bc_type = DF_BC_FREE_END;

	// массив для коэффициентов сплайна
	double* scoeff = new double[nY * (nX - 1) * s_order];
	try
	{
		DFTaskPtr task;
		int status = -1;
		// Cоздание задачи (task)
		status = dfdNewTask1D(&task,
			nX, X,
			DF_NON_UNIFORM_PARTITION, // равномерная сетка узлов
			nY, Y,
			DF_MATRIX_STORAGE_ROWS); // формат хранения значений векторной
		// функции по умолчанию (построчно)
		if (status != DF_STATUS_OK) throw 1;
		// Настройка параметров задачи
		status = dfdEditPPSpline1D(task,
			s_order, s_type, bc_type, NULL,
			DF_NO_IC, // тип условий во внутренних точках
			NULL, // массив значений для условий во внутренних точках
			scoeff,
			DF_NO_HINT); // формат упаковки коэффициентов сплайна
		// в одномерный массив (Row-major - построчно)
		if (status != DF_STATUS_OK) throw 2;

		// Создание сплайна
		status = dfdConstruct1D(task,
			DF_PP_SPLINE, // поддерживается только одно значение
			DF_METHOD_STD); // поддерживается только одно значение
		double grid[2]{ X[0], X[nX - 1] };
		int nDorder = 3; // число производных, которые вычисляются, плюс 1
		MKL_INT dorder[] = { 1, 1, 1 }; // вычисляются значения сплайна,
		// его первая и вторая производные
		status = dfdInterpolate1D(task, DF_INTERP, DF_METHOD_PP, nX, grid, DF_UNIFORM_PARTITION, nDorder, dorder, NULL, splineValues, DF_MATRIX_STORAGE_ROWS, NULL);
		if (status != DF_STATUS_OK) { throw 5; }
		if (status != DF_STATUS_OK) throw 3;
		status = dfDeleteTask(&task);
		if (status != DF_STATUS_OK) throw 4;
	}
	catch (int ret)
	{
		delete[] scoeff;
		return ret;
	}
	delete[] scoeff;
	return 0;
}