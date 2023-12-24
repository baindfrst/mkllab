#include "pchh.h"
#include "mkl.h"
#include <iostream>

extern "C" _declspec(dllexport)
bool TrustRegion(
	MKL_INT n, // число независимых переменных
	MKL_INT m, // число компонент векторной функции
	double* x, // начальное приближение и решение
	double* y, // значения в сетке
	const double* eps, // массив с 6 элементами, определяющих критерии
	// остановки итерационного процесса
	double jac_eps, // точность вычисления элементов матрицы Якоби
	MKL_INT niter1, // максимальное число итераций
	MKL_INT niter2, // максимальное число итераций при выборе пробного шага
	double rs, // начальный размер доверительной области
	MKL_INT & ndoneIter, // число выполненных итераций
	double& resInitial, // начальное значение невязки
	double& resFinal, // финальное значение невязки
	MKL_INT & stopCriteria,// выполненный критерий остановки
	MKL_INT * checkInfo, // информация об ошибках при проверке данных
	int& error,
	USRFCNXD funcinp) // информация об ошибках 
{
	std::cout << "START DAN" << std::endl;
	_TRNSP_HANDLE_t handle = NULL; // переменная для дескриптора задачи
	double* fvec = NULL; // массив значений векторной функции
	double* fjac = NULL; // массив с элементами матрицы Якоби
	error = 0;
	try
	{
		std::cout << "try start" << std::endl;
	    fvec = new double[m]; // массив значений векторной функции
		fjac = new double[n * m]; // массив с элементами матрицы Якоби
		// Инициализация задачи
		std::cout << "int" << std::endl;
		MKL_INT ret = dtrnlsp_init(&handle, &n, &m, x, eps, &niter1, &niter2, &rs);
		if (ret != TR_SUCCESS) throw (1);
		std::cout << "correct data" << std::endl;
		// Проверка корректности входных данных
		ret = dtrnlsp_check(&handle, &n, &m, fjac, fvec, eps, checkInfo);
		if (ret != TR_SUCCESS) throw (2);
		MKL_INT RCI_Request = 0; // надо инициализировать 0 !!!
		// Итерационный процесс
		std::cout << "start while" << std::endl;
		while (true)
		{
			std::cout << x[0] << std::endl;
			ret = dtrnlsp_solve(&handle, fvec, fjac, &RCI_Request);
			if (ret != TR_SUCCESS) throw (3);
			if (RCI_Request == 0) continue;
			else if (RCI_Request == 1) fvec = y;
			else if (RCI_Request == 2)
			{
				std::cout << "jacob" << std::endl;
				ret = djacobix(funcinp, &n, &m, fjac, x, &jac_eps, y);
				if (ret != TR_SUCCESS) throw (4);
			}
			else if (RCI_Request >= -6 && RCI_Request <= -1) break;
			else throw (5);
		}
		// Завершение итерационного процесса
		std::cout << "procces end" << std::endl;
		ret = dtrnlsp_get(&handle, &ndoneIter, &stopCriteria,
			&resInitial, &resFinal);
		if (ret != TR_SUCCESS) throw (6);
		// Освобождение ресурсов
		std::cout << "free resurs" << std::endl;
		ret = dtrnlsp_delete(&handle);
		std::cout << "free resurs end" << std::endl;
		if (ret != TR_SUCCESS) throw (7);
		std::cout << "end try" << std::endl;
	}
	catch (int _error) { error = _error; }
	std::cout << "pre del" << std::endl;
	// Освобождение памяти

	std::cout << "end dll" << std::endl;
}
//extern "C" _declspec(dllexport)
//bool TrustRegion(
//	MKL_INT n, // число независимых переменных
//	MKL_INT m, // число компонент векторной функции
//	double* x, // начальное приближение и решение
//	USRFCND FValues, // указатель на функцию, вычисляющую векторную
//	// функцию в заданной точке
//	const double* eps, // массив с 6 элементами, определяющих критерии
//	// остановки итерационного процесса
//	double jac_eps, // точность вычисления элементов матрицы Якоби
//	MKL_INT niter1, // максимальное число итераций
//	MKL_INT niter2, // максимальное число итераций при выборе пробного шага
//	double rs, // начальный размер доверительной области
//	MKL_INT& ndoneIter, // число выполненных итераций
//	double& resInitial, // начальное значение невязки
//	double& resFinal, // финальное значение невязки
//	MKL_INT& stopCriteria,// выполненный критерий остановки
//	MKL_INT* checkInfo, // информация об ошибках при проверке данных
//	int& error) // информация об ошибках
//
//{
//	_TRNSP_HANDLE_t handle = NULL; // переменная для дескриптора задачи
//	double* fvec = NULL; // массив значений векторной функции
//	double* fjac = NULL; // массив с элементами матрицы Якоби
//	error = 0;
//	try
//	{
//		fvec = new double[m]; // массив значений векторной функции
//		fjac = new double[n * m]; // массив с элементами матрицы Якоби
//		// Инициализация задачи
//		MKL_INT ret = dtrnlsp_init(&handle, &n, &m, x, eps, &niter1, &niter2, &rs);
//		if (ret != TR_SUCCESS) throw (-1);
//		// Проверка корректности входных данных
//		ret = dtrnlsp_check(&handle, &n, &m, fjac, fvec, eps, checkInfo);
//		if (ret != TR_SUCCESS) throw (1);
//		MKL_INT RCI_Request = 0; // надо инициализировать 0 !!!
//		// Итерационный процесс
//		while (true)
//		{
//			ret = dtrnlsp_solve(&handle, fvec, fjac, &RCI_Request);
//			if (ret != TR_SUCCESS) throw (2);
//			if (RCI_Request == 0) continue;
//			else if (RCI_Request == 1) FValues(&m, &n, x, fvec);
//			else if (RCI_Request == 2)
//			{
//				ret = djacobi(FValues, &n, &m, fjac, x, &jac_eps);
//				if (ret != TR_SUCCESS) throw (3);
//			}
//			else if (RCI_Request >= -6 && RCI_Request <= -1) break;
//			else throw (4);
//		}
//			// Завершение итерационного процесса
//			ret = dtrnlsp_get(&handle, &ndoneIter, &stopCriteria,
//				&resInitial, &resFinal);
//		if (ret != TR_SUCCESS) throw (5);
//		// Освобождение ресурсов
//		ret = dtrnlsp_delete(&handle);
//		if (ret != TR_SUCCESS) throw (6);
//	}
//	catch (int _error) { error = _error; }
//	// Освобождение памяти
//	if (fvec != NULL) delete[] fvec;
//	if (fjac != NULL) delete[] fjac;
//}