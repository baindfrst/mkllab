#include "pchh.h"
#include "pch.h"
#include "mkl.h"
#include <iostream>



struct data
{
	double* uniform = NULL;
	double* X = NULL;
	double* Y = NULL;
};

void Generate(MKL_INT* X, MKL_INT* nX, double* Y, double* Ynev, void* usr_data)
{
	data* d = (data*)usr_data;
	CubicSpline(*nX, d->X, 1, Y, *X, d->X[0], d->X[*nX - 1], Ynev);

	for (int i = 0; i < *X; i++)
	{
		Ynev[i] -= d->Y[i];
	}

}
extern "C" _declspec(dllexport)
bool TrustRegion(
	MKL_INT n, // число независимых переменных
	MKL_INT m, // число компонент векторной функции
	double* x, // начальное приближение и решение
	double* y,
	const double* eps, // массив с 6 элементами, определ€ющих критерии
	// остановки итерационного процесса
	double jac_eps, // точность вычислени€ элементов матрицы якоби
	MKL_INT niter1, // максимальное число итераций
	MKL_INT niter2, // максимальное число итераций при выборе пробного шага
	double rs, // начальный размер доверительной области
	MKL_INT& ndoneIter, // число выполненных итераций
	double& resInitial, // начальное значение нев€зки
	double& resFinal, // финальное значение нев€зки
	MKL_INT& stopCriteria,// выполненный критерий остановки
	MKL_INT* checkInfo, // информаци€ об ошибках при проверке данных
	int& error) // информаци€ об ошибках

{
	_TRNSP_HANDLE_t handle = NULL; // переменна€ дл€ дескриптора задачи
	double* fvec = NULL; // массив значений векторной функции
	double* fjac = NULL; // массив с элементами матрицы якоби
	error = 0;
	try
	{
		fvec = new double[m]; // массив значений векторной функции
		fjac = new double[n * m]; // массив с элементами матрицы якоби
		double* Yitr = new double[m];
		for (int i = 0; i < m - 1; i++)
		{
			Yitr[i] = 3;
		}
		// »нициализаци€ задачи
		double* Xravnomer = new double[m];
		for (int i = 0; i < m - 1; i++)
		{
			Xravnomer[i] = x[0] + ((x[n - 1] - x[0]) / m) * i;
		}
		data* dataopt = NULL;
		dataopt->uniform = Xravnomer;
		dataopt->X = x;
		dataopt->Y = y;
		MKL_INT ret = dtrnlsp_init(&handle, &m, &n, Yitr, eps, &niter1, &niter2, &rs);
		if (ret != TR_SUCCESS) throw (-1);
		// ѕроверка корректности входных данных
		ret = dtrnlsp_check(&handle, &m, &n, fjac, fvec, eps, checkInfo);
		if (ret != TR_SUCCESS) throw (1);
		MKL_INT RCI_Request = 0; // надо инициализировать 0 !!!
		// »терационный процесс
		while (true)
		{
			ret = dtrnlsp_solve(&handle, fvec, fjac, &RCI_Request);
			if (ret != TR_SUCCESS) throw (2);
			if (RCI_Request == 0) continue;
			else if (RCI_Request == 1) Generate(&m, &n, x, fvec, dataopt);
			else if (RCI_Request == 2)
			{
				ret = djacobix(Generate, &n, &m, fjac, x, &jac_eps, dataopt);
				if (ret != TR_SUCCESS) throw (3);
			}
			else if (RCI_Request >= -6 && RCI_Request <= -1) break;
			else throw (4);
		}
			// «авершение итерационного процесса
			ret = dtrnlsp_get(&handle, &ndoneIter, &stopCriteria,
				&resInitial, &resFinal);
		if (ret != TR_SUCCESS) throw (5);
		// ќсвобождение ресурсов
		ret = dtrnlsp_delete(&handle);
		if (ret != TR_SUCCESS) throw (6);
		//CubicSpline(m, Xravnomer,1, Yitr, m, x[0], x[n-1], )
	}
	catch (int _error) { error = _error; }
	// ќсвобождение пам€ти
	if (fvec != NULL) delete[] fvec;
	if (fjac != NULL) delete[] fjac;
}