#include<math.h>
#include<stdio.h>
#include<mpi.h>
#include<unistd.h>
#include<stdlib.h>


void Show(int procNumber, int m_Data[], int result)
{
	printf("wynik NWD dla zbioru liczb\n");
	int i;
	
	for (i=0; i<procNumber; i++)
	{
		printf("%d\n", m_Data[i]);
	}
	printf("Wynosi : %d\n",result);
}

int calculateNWD(int a, int b)
{
	int tmp;

	while(b != 0)
	{
		tmp = b;
		b = a%b;
		a = tmp;
	}
	return a;
}

int main(int argc, char **argv)
{

	int i, thisProcNumber, procNumber, number, receive;
	
	MPI_Init(NULL, NULL); //inicjalizacja MPI
	MPI_Comm_size(MPI_COMM_WORLD, &procNumber); //ustalenie ilosci procesów
	MPI_Comm_rank(MPI_COMM_WORLD, &thisProcNumber); //przyporzadkowanie rank do procesow
	
	//Walidacja m_Data procesow
	if(procNumber % 2 != 0)
	{
		printf("Podana liczba procesow nie jest potega 2!\n");
		MPI_Finalize();
		return (EXIT_FAILURE);
	}


	//wczytanie z argumentow wywolania liczb do liczenia nwd
	int m_Data[procNumber];
	for(i=1; i<procNumber+1; i++)
        m_Data[i-1] = atoi(argv[i]);

	number = m_Data[thisProcNumber]; //przydzielenie liczb do procesow
	int logProcNumber = log2(procNumber);

	if (argc-1 != procNumber) 
	{
        if (thisProcNumber == 0)
            printf("Liczba argumentów rozna od liczby procesow!\nPowinno byc  %d liczb, otrzymano %d\n", procNumber, argc-1);
        return -1;
    }

	for (int i = 0; i < argc-1; i++) 
	{
        int m_Data = atoi(argv[i+1]);

        if (m_Data <= 0) 
		{
            if (thisProcNumber == 0)
                printf("Niepoprawny argument, wartosc powinna byc dodatnia.\n");
            return -1;
        }
    }

	for(i = 1; i <= logProcNumber; i++)
	{
		int step = pow(2,(i-1));

		if(thisProcNumber + step < procNumber) //wysylka liczb, sprawdzenie kolejnosci
			MPI_Send(&number, 1, MPI_INT, thisProcNumber + step, 0, MPI_COMM_WORLD);
		else //wysylka liczb
			MPI_Send(&number, 1, MPI_INT, thisProcNumber + step - procNumber, 0, MPI_COMM_WORLD);

		
		if(thisProcNumber - step >= 0) //odbior liczb, dla poprawnej kolejnosci
			MPI_Recv(&receive, 1, MPI_INT, thisProcNumber - step, 0, MPI_COMM_WORLD, MPI_STATUS_IGNORE);
		else //odbior liczb
			MPI_Recv(&receive, 1, MPI_INT, thisProcNumber - step + procNumber, 0, MPI_COMM_WORLD, MPI_STATUS_IGNORE);

		printf("Proces o numerze %d, wartosc kroku: %d,  nwd wynosi(%d,%d) = %d\n", thisProcNumber, step, number, receive, calculateNWD(number, receive));		
	}


	if(thisProcNumber == 0) //dla procesu macierzystego wypisz wynik
		Show(procNumber, m_Data,number);

	MPI_Finalize();
	return 0;
}

