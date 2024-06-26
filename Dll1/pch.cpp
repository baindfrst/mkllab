﻿#include "pch.h"
#include "mkl.h"
#include <iostream>

struct data
{
	double* x;
	double* y;
	void (*funcY)(int x, double* y1, double* y2);
	bool typemesh;

	data(int len, double* xv, double* yv, void (*f)(int x, double* y1, double* y2), bool mesh)
	{
		funcY = f;
		x = new double[len];
		y = new double[len];
		typemesh = mesh;
		for (int i = 0; i < len; i++)
		{
			x[i] = xv[i];
			y[i] = yv[i];
		}
	}
};

extern "C" _declspec(dllexport)
int CubicSplineSupport(
	int nX, // число узлов сплайна
	const double* X, // массив узлов сплайна
	int nY, // размерность векторной функции
	const double* Y,// массив заданных значений векторной функции
	int Sn,
	bool type,
	double* splineValues)  // массив вычисленных значений сплайна и производных
{
	MKL_INT s_order = DF_PP_CUBIC; // степень кубического сплайна
	MKL_INT s_type = DF_PP_NATURAL; // тип сплайна
	// тип граничных условий
	double bc[2] = { 0,0 };
	MKL_INT bc_type = DF_BC_2ND_LEFT_DER | DF_BC_2ND_RIGHT_DER;
	double* scoeff = new double[nY * (nX - 1) * s_order];
	std::cout << "-----------start spline------------------" << std::endl;

	try
	{
		DFTaskPtr task;
		MKL_INT type_mesh;
		type = false;
		if (type)
		{
			type_mesh = DF_UNIFORM_PARTITION;
		}
		else
		{
			type_mesh = DF_NON_UNIFORM_PARTITION;
		}
		int status = -1;
		// Cоздание задачи (task)
		status = dfdNewTask1D(&task,
			nX, X,
			type_mesh, // равномерная сетка узлов
			nY, Y,
			DF_NO_HINT); // формат хранения значений векторной
		std::cout << "-----------task created------------------" << std::endl;
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
		std::cout << "-----------edit end------------------" << std::endl;

		// Создание сплайна
		status = dfdConstruct1D(task,
			DF_PP_SPLINE, // поддерживается только одно значение
			DF_METHOD_STD); // поддерживается только одно значение
		double grid[2]{ X[0], X[nX - 1]};
		int nDorder = 3; // число производных, которые вычисляются, плюс 1
		MKL_INT dorder[] = { 1, 1, 1 }; // вычисляются значения сплайна,
		// его первая и вторая производные
			status = dfdInterpolate1D(task, DF_INTERP, DF_METHOD_PP, Sn, grid, DF_UNIFORM_PARTITION, nDorder, dorder, NULL, splineValues, DF_NO_HINT, NULL);
		std::cout << "-----------interpolate end------------------" << std::endl;
		if (status != DF_STATUS_OK) { throw 5; }
		if (status != DF_STATUS_OK) throw 3;
		status = dfDeleteTask(&task);
		if (status != DF_STATUS_OK) throw 4;
	}
	catch (int ret)
	{
		return ret;
	}
	std::cout << "-----------spline end------------------" << std::endl;
	return 0;
}

//double normNev(int n, double* Xs, double* y)
//{
//	double ret = 0;
//	for (int i = 0; i < n; i++)
//	{
//		ret += (Xs[i*3] - y[i]) * (Xs[i*3] - y[i]);
//	}
//	return ret;
//}
//
//void funcY(int x, double* y1, double* y2);
//
//void func(MKL_INT* m, MKL_INT* n, double* x, double* f, void* usrdata)
//{
//	data* d = (data*)usrdata;
//	double* s = new double[*m * 3];
//	CubicSpline(*n, x, 1, d->y, *m, d->typemesh, s);
//	for (int i = 0; i < *m; i++)
//	{
//		double y1, y2;
//		d->funcY(x[0] + ((x[*n-1] - x[0]) / (*m-1) *i), &y1, &y2);
//		f[i] = (s[i*3] - y1) * (s[i * 3] - y1);
//	}
//
//}
//
//
//extern "C" _declspec(dllexport)
//bool TrustRegion(
//	MKL_INT n, // число независимых переменных
//	MKL_INT m, // число компонент векторной функции
//	double* x, // начальное приближение и решение
//	double* y,
//	void (*funcY)(int x, double* y1, double* y2),
//	const double* eps, // массив с 6 элементами, определяющих критерии
//	// остановки итерационного процесса
//	double jac_eps, // точность вычисления элементов матрицы Якоби
//	MKL_INT niter1, // максимальное число итераций
//	MKL_INT niter2, // максимальное число итераций при выборе пробного шага
//	double rs, // начальный размер доверительной области
//	MKL_INT & ndoneIter, // число выполненных итераций
//	double& resInitial, // начальное значение невязки
//	double& resFinal, // финальное значение невязки
//	MKL_INT & stopCriteria,// выполненный критерий остановки
//	MKL_INT * checkInfo, // информация об ошибках при проверке данных
//	bool typemesh,
//	int error) // информация об ошибках
//
//{
//	_TRNSP_HANDLE_t handle = NULL; // переменная для дескриптора задачи
//	double* fvec = NULL; // массив значений векторной функции
//	double* fjac = NULL; // массив с элементами матрицы Якоби
//	error = 0;
//	try
//	{
//		data d(m, x, y, funcY, typemesh);
//		fvec = new double[m]; // массив значений векторной функции
//		fjac = new double[n * m]; // массив с элементами матрицы Якоби
//		// Инициализация задачи
//		MKL_INT ret = dtrnlsp_init(&handle, &n, &m, y, eps, &niter1, &niter2, &rs);
//		if (ret != TR_SUCCESS) throw 1;
//		// Проверка корректности входных данных
//		ret = dtrnlsp_check(&handle, &n, &m, fjac, fvec, eps, checkInfo);
//		if (ret != TR_SUCCESS) throw 2;
//		MKL_INT RCI_Request = 0; // надо инициализировать 0 !!!
//		// Итерационный процесс
//		while (true)
//		{
//			ret = dtrnlsp_solve(&handle, fvec, fjac, &RCI_Request);
//			if (ret != TR_SUCCESS) throw 3;
//			if (RCI_Request == 0) continue;
//			else if (RCI_Request == 1) func(&m, &n, y, fvec, &d);
//			else if (RCI_Request == 2)
//			{
//				ret = djacobix(func, &n, &m, fjac, y, &jac_eps, &d);
//				if (ret != TR_SUCCESS) throw 4;
//			}
//			else if (RCI_Request >= -6 && RCI_Request <= -1) break;
//			else throw 5;
//		}
//		// Завершение итерационного процесса
//		ret = dtrnlsp_get(&handle, &ndoneIter, &stopCriteria,
//			&resInitial, &resFinal);
//		if (ret != TR_SUCCESS) throw 6;
//		// Освобождение ресурсов
//		ret = dtrnlsp_delete(&handle);
//		if (ret != TR_SUCCESS) throw 7;
//	}
//	catch (int _error) { error = _error; }
//}
//

enum class ErrorEnum { NO, INIT, CHECK, SOLVE, JACOBI, GET, DELET, RCI };

typedef void (*FValues) (MKL_INT* m, MKL_INT* n, double* x, double* f, void* user_data);

class SupportData {
public:
	const double* X;
	const double* Y;
	int argMin, argMax;
	SupportData(const int size, const double* x, const double* y)
		: X(x), Y(y) {
		argMin = 0;
		argMax = 0;
		for (int i = 1; i < size; ++i) {
			if (x[i] < x[argMin])
				argMin = i;
			if (x[i] > x[argMax])
				argMax = i;
		}
	}
};

using namespace std;

void TestFunction(MKL_INT* m, MKL_INT* n, double* y, double* f, void* user_data) {
	MKL_INT s_order = DF_PP_CUBIC; // степень кубического сплайна
	MKL_INT s_type = DF_PP_NATURAL; // тип сплайна
	MKL_INT bc_type = DF_BC_2ND_LEFT_DER | DF_BC_2ND_RIGHT_DER; // тип граничных условий

	double* scoeff = new double[(*n - 1) * s_order];
	SupportData* data = (SupportData*)user_data;

	double x[] = { data->X[data->argMin], data->X[data->argMax] };
	try
	{
		DFTaskPtr task;
		int status = -1;

		// Cоздание задачи (task) 
		status = dfdNewTask1D(&task,
			*n, x, DF_UNIFORM_PARTITION,
			1, y, DF_NO_HINT);
		if (status != DF_STATUS_OK) throw 1;

		// Настройка параметров задачи 
		double bc[2]{ 0.0, 0.0 }; // массив граничных значений
		status = dfdEditPPSpline1D(task,
			s_order, s_type, bc_type, bc,
			DF_NO_IC, NULL,
			scoeff, DF_NO_HINT);
		if (status != DF_STATUS_OK) throw 2;

		// Создание сплайна 
		status = dfdConstruct1D(task, DF_PP_SPLINE, DF_METHOD_STD);
		if (status != DF_STATUS_OK) throw 3;

		// Вычисление значений сплайна и его производныx
		int nDorder = 1;
		MKL_INT dorder[] = { 1 };
		status = dfdInterpolate1D(task,
			DF_INTERP, DF_METHOD_PP,
			*m, data->X,
			DF_NON_UNIFORM_PARTITION,
			nDorder, dorder, NULL,
			f, DF_NO_HINT, NULL);
		if (status != DF_STATUS_OK) throw 4;

		// Освобождение ресурсов 
		status = dfDeleteTask(&task);
		if (status != DF_STATUS_OK) throw 6;
	}
	catch (int ret)
	{
		delete[] scoeff;
		return;
	}

	for (int i = 0; i < *m; ++i) f[i] -= data->Y[i];
	delete[] scoeff;
}

bool TrustRegion(
	MKL_INT n, // число независимых переменных
	MKL_INT m, // число компонент векторной функции
	double* x, // начальное приближение и решение
	FValues FValues, // указатель на функцию, вычисляющую векторную
	// функцию в заданной точке
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
	SupportData* data) // информация об ошибках 
{
	_TRNSP_HANDLE_t handle = NULL; // переменная для дескриптора задачи
	double* fvec = NULL; // массив значений векторной функции
	double* fjac = NULL; // массив с элементами матрицы Якоби
	fvec = new double[m]; // массив значений векторной функции
	fjac = new double[n * m]; // массив с элементами матрицы Якоби

	// Инициализация задачи
	MKL_INT ret = dtrnlsp_init(&handle, &n, &m, x, eps, &niter1, &niter2, &rs);

	// Проверка корректности входных данных 
	ret = dtrnlsp_check(&handle, &n, &m, fjac, fvec, eps, checkInfo);
	MKL_INT RCI_Request = 0;

	// Итерационный процесс
	while (true)
	{
		ret = dtrnlsp_solve(&handle, fvec, fjac, &RCI_Request);
		if (ret != TR_SUCCESS) break;
		if (RCI_Request == 0) continue;
		else if (RCI_Request == 1) TestFunction(&m, &n, x, fvec, data);
		else if (RCI_Request == 2)
		{
			ret = djacobix(TestFunction, &n, &m, fjac, x, &jac_eps, data);
		}
		else break;
	}

	// Завершение итерационного процесса
	ret = dtrnlsp_get(&handle, &ndoneIter, &stopCriteria, &resInitial, &resFinal);

	// Освобождение ресурсов
	ret = dtrnlsp_delete(&handle);

	// Освобождение памяти
	if (fvec != NULL) delete[] fvec;
	if (fjac != NULL) delete[] fjac;
	return 0;
}


extern "C" _declspec(dllexport)
int CubicSpline(
	int nX, int m, int maxIter,
	double* X,
	double* Y,
	double* YSpline,
	double& minRes,
	int& countIter,
	int& status)
{
	setlocale(LC_ALL, ""); // использование кириллицы

	SupportData data(nX, X, Y);

	double* y_ret = new double[m]; // массив с начальным приближением и решением 
	for (int i = 0; i < m; ++i)
		y_ret[i] = 0;
	y_ret[0] = data.Y[data.argMin];
	y_ret[m - 1] = data.Y[data.argMax];

	MKL_INT niter1 = maxIter; // максимальное число итераций
	MKL_INT niter2 = maxIter / 2; // максимальное число итераций при выборе пробного шага
	double rs = 10; // начальное значение для доверительного интервала

	// массив критериев остановки
	const double eps[6] = { 1.0E-22 , 1.0E-20 , 1.0E-20 , 1.0E-20 , 1.0E-22 , 1.0E-22 };
	double jac_eps = 1.0E-8; // точность вычисления элементов матрицы Якоби
	double res_initial = 0; // начальное значение невязки 

	MKL_INT check_data_info[4]; // результат проверки корректности данных 
	TrustRegion(m, nX, y_ret, TestFunction, eps, jac_eps, niter1, niter2, rs,
		countIter, res_initial, minRes, status,
		check_data_info, &data);

	TestFunction(&nX, &m, y_ret, YSpline, &data);
	for (int i = 0; i < nX; ++i)
		YSpline[i] += Y[i];

	return 0;
}

