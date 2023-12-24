#include "pchh.h"
#include "mkl.h"
#include <iostream>

extern "C" _declspec(dllexport)
bool TrustRegion(
	MKL_INT n, // ����� ����������� ����������
	MKL_INT m, // ����� ��������� ��������� �������
	double* x, // ��������� ����������� � �������
	double* y, // �������� � �����
	const double* eps, // ������ � 6 ����������, ������������ ��������
	// ��������� ������������� ��������
	double jac_eps, // �������� ���������� ��������� ������� �����
	MKL_INT niter1, // ������������ ����� ��������
	MKL_INT niter2, // ������������ ����� �������� ��� ������ �������� ����
	double rs, // ��������� ������ ������������� �������
	MKL_INT & ndoneIter, // ����� ����������� ��������
	double& resInitial, // ��������� �������� �������
	double& resFinal, // ��������� �������� �������
	MKL_INT & stopCriteria,// ����������� �������� ���������
	MKL_INT * checkInfo, // ���������� �� ������� ��� �������� ������
	int& error,
	USRFCNXD funcinp) // ���������� �� ������� 
{
	std::cout << "START DAN" << std::endl;
	_TRNSP_HANDLE_t handle = NULL; // ���������� ��� ����������� ������
	double* fvec = NULL; // ������ �������� ��������� �������
	double* fjac = NULL; // ������ � ���������� ������� �����
	error = 0;
	try
	{
		std::cout << "try start" << std::endl;
	    fvec = new double[m]; // ������ �������� ��������� �������
		fjac = new double[n * m]; // ������ � ���������� ������� �����
		// ������������� ������
		std::cout << "int" << std::endl;
		MKL_INT ret = dtrnlsp_init(&handle, &n, &m, x, eps, &niter1, &niter2, &rs);
		if (ret != TR_SUCCESS) throw (1);
		std::cout << "correct data" << std::endl;
		// �������� ������������ ������� ������
		ret = dtrnlsp_check(&handle, &n, &m, fjac, fvec, eps, checkInfo);
		if (ret != TR_SUCCESS) throw (2);
		MKL_INT RCI_Request = 0; // ���� ���������������� 0 !!!
		// ������������ �������
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
		// ���������� ������������� ��������
		std::cout << "procces end" << std::endl;
		ret = dtrnlsp_get(&handle, &ndoneIter, &stopCriteria,
			&resInitial, &resFinal);
		if (ret != TR_SUCCESS) throw (6);
		// ������������ ��������
		std::cout << "free resurs" << std::endl;
		ret = dtrnlsp_delete(&handle);
		std::cout << "free resurs end" << std::endl;
		if (ret != TR_SUCCESS) throw (7);
		std::cout << "end try" << std::endl;
	}
	catch (int _error) { error = _error; }
	std::cout << "pre del" << std::endl;
	// ������������ ������

	std::cout << "end dll" << std::endl;
}
//extern "C" _declspec(dllexport)
//bool TrustRegion(
//	MKL_INT n, // ����� ����������� ����������
//	MKL_INT m, // ����� ��������� ��������� �������
//	double* x, // ��������� ����������� � �������
//	USRFCND FValues, // ��������� �� �������, ����������� ���������
//	// ������� � �������� �����
//	const double* eps, // ������ � 6 ����������, ������������ ��������
//	// ��������� ������������� ��������
//	double jac_eps, // �������� ���������� ��������� ������� �����
//	MKL_INT niter1, // ������������ ����� ��������
//	MKL_INT niter2, // ������������ ����� �������� ��� ������ �������� ����
//	double rs, // ��������� ������ ������������� �������
//	MKL_INT& ndoneIter, // ����� ����������� ��������
//	double& resInitial, // ��������� �������� �������
//	double& resFinal, // ��������� �������� �������
//	MKL_INT& stopCriteria,// ����������� �������� ���������
//	MKL_INT* checkInfo, // ���������� �� ������� ��� �������� ������
//	int& error) // ���������� �� �������
//
//{
//	_TRNSP_HANDLE_t handle = NULL; // ���������� ��� ����������� ������
//	double* fvec = NULL; // ������ �������� ��������� �������
//	double* fjac = NULL; // ������ � ���������� ������� �����
//	error = 0;
//	try
//	{
//		fvec = new double[m]; // ������ �������� ��������� �������
//		fjac = new double[n * m]; // ������ � ���������� ������� �����
//		// ������������� ������
//		MKL_INT ret = dtrnlsp_init(&handle, &n, &m, x, eps, &niter1, &niter2, &rs);
//		if (ret != TR_SUCCESS) throw (-1);
//		// �������� ������������ ������� ������
//		ret = dtrnlsp_check(&handle, &n, &m, fjac, fvec, eps, checkInfo);
//		if (ret != TR_SUCCESS) throw (1);
//		MKL_INT RCI_Request = 0; // ���� ���������������� 0 !!!
//		// ������������ �������
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
//			// ���������� ������������� ��������
//			ret = dtrnlsp_get(&handle, &ndoneIter, &stopCriteria,
//				&resInitial, &resFinal);
//		if (ret != TR_SUCCESS) throw (5);
//		// ������������ ��������
//		ret = dtrnlsp_delete(&handle);
//		if (ret != TR_SUCCESS) throw (6);
//	}
//	catch (int _error) { error = _error; }
//	// ������������ ������
//	if (fvec != NULL) delete[] fvec;
//	if (fjac != NULL) delete[] fjac;
//}