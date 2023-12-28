#include "pch.h"
#include "mkl.h"
#include <iostream>

struct data
{
	double* x;
	double* y;
	void (*funcY)(int x, double* y1, double* y2);
	data(int len, double* xv, double* yv, void (*f)(int x, double* y1, double* y2))
	{
		funcY = f;
		x = new double[len];
		y = new double[len];
		for (int i = 0; i < len; i++)
		{
			x[i] = xv[i];
			y[i] = yv[i];
		}
	}
};

extern "C" _declspec(dllexport)
int CubicSpline(
	int nX, // число узлов сплайна
	const double* X, // массив узлов сплайна
	int nY, // размерность векторной функции
	const double* Y,// массив заданных значений векторной функции
	int Sn,
	double* splineValues)  // массив вычисленных значений сплайна и производных
{
	MKL_INT s_order = DF_PP_CUBIC; // степень кубического сплайна
	MKL_INT s_type = DF_PP_NATURAL; // тип сплайна
	// тип граничных условий
	double bc[2] = { 0,0 };
	MKL_INT bc_type = DF_BC_2ND_LEFT_DER | DF_BC_2ND_RIGHT_DER;
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
			DF_NO_HINT); // формат хранения значений векторной
		// функции по умолчанию (построчно)
		if (status != DF_STATUS_OK) throw 1;
		// Настройка параметров задачи
		status = dfdEditPPSpline1D(task,
			s_order, s_type, bc_type, bc,
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
		double grid[2]{ X[0], X[nX - 1]};
		int nDorder = 3; // число производных, которые вычисляются, плюс 1
		MKL_INT dorder[] = { 1, 1, 1 }; // вычисляются значения сплайна,
		// его первая и вторая производные
		status = dfdInterpolate1D(task, DF_INTERP, DF_METHOD_PP, Sn, grid, DF_UNIFORM_PARTITION, nDorder, dorder, NULL, splineValues, DF_NO_HINT, NULL);
		if (status != DF_STATUS_OK) { throw 5; }
		if (status != DF_STATUS_OK) throw 3;
		status = dfDeleteTask(&task);
		if (status != DF_STATUS_OK) throw 4;
	}
	catch (int ret)
	{
		return ret;
	}
	return 0;
}

double normNev(int n, double* Xs, double* y)
{
	double ret = 0;
	for (int i = 0; i < n; i++)
	{
		ret += (Xs[i*3] - y[i]) * (Xs[i*3] - y[i]);
	}
	return ret;
}

void funcY(int x, double* y1, double* y2);

void func(MKL_INT* m, MKL_INT* n, double* x, double* f, void* usrdata)
{
	data* d = (data*)usrdata;
	double* s = new double[*m * 3];
	CubicSpline(*n, x, 1, d->y, *m, s);
	for (int i = 0; i < *m; i++)
	{
		double y1, y2;
		d->funcY(x[0] + ((x[*n-1] - x[0]) / (*m-1) *i), &y1, &y2);
		f[i] = (s[i*3] - y1) * (s[i * 3] - y1);
	}

}


extern "C" _declspec(dllexport)
bool TrustRegion(
	MKL_INT n, // число независимых переменных
	MKL_INT m, // число компонент векторной функции
	double* x, // начальное приближение и решение
	double* y,
	void (*funcY)(int x, double* y1, double* y2),
	const double* eps, // массив с 6 элементами, определяющих критерии
	// остановки итерационного процесса
	double jac_eps, // точность вычисления элементов матрицы Якоби
	MKL_INT niter1, // максимальное число итераций
	MKL_INT niter2, // максимальное число итераций при выборе пробного шага
	double rs, // начальный размер доверительной области
	MKL_INT& ndoneIter, // число выполненных итераций
	double& resInitial, // начальное значение невязки
	double& resFinal, // финальное значение невязки
	MKL_INT& stopCriteria,// выполненный критерий остановки
	MKL_INT* checkInfo, // информация об ошибках при проверке данных
	int error) // информация об ошибках

{
	_TRNSP_HANDLE_t handle = NULL; // переменная для дескриптора задачи
	double* fvec = NULL; // массив значений векторной функции
	double* fjac = NULL; // массив с элементами матрицы Якоби
	error = 0;
	try
	{
		data d(n, x, y, funcY);
		fvec = new double[m]; // массив значений векторной функции
		fjac = new double[n * m]; // массив с элементами матрицы Якоби
		// Инициализация задачи
		MKL_INT ret = dtrnlsp_init(&handle, &n, &m, x, eps, &niter1, &niter2, &rs);
		if (ret != TR_SUCCESS) throw 1;
		// Проверка корректности входных данных
		ret = dtrnlsp_check(&handle, &n, &m, fjac, fvec, eps, checkInfo);
		if (ret != TR_SUCCESS) throw 2;
		MKL_INT RCI_Request = 0; // надо инициализировать 0 !!!
		// Итерационный процесс
		while (true)
		{
			ret = dtrnlsp_solve(&handle, fvec, fjac, &RCI_Request);
			if (ret != TR_SUCCESS) throw 3;
			if (RCI_Request == 0) continue;
			else if (RCI_Request == 1) func(&m, &n, x, fvec, &d);
			else if (RCI_Request == 2)
			{
				ret = djacobix(func, &n, &m, fjac, x, &jac_eps, &d);
				if (ret != TR_SUCCESS) throw 4;
			}
			else if (RCI_Request >= -6 && RCI_Request <= -1) break;
			else throw 5;
		}
		// Завершение итерационного процесса
		ret = dtrnlsp_get(&handle, &ndoneIter, &stopCriteria,
			&resInitial, &resFinal);
		if (ret != TR_SUCCESS) throw 6;
		// Освобождение ресурсов
		ret = dtrnlsp_delete(&handle);
		if (ret != TR_SUCCESS) throw 7;
	}
	catch (int _error) { error = _error; }
}