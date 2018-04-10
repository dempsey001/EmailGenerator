# Email Generator

This is a simple C# .NET Core application that can be used to generate a set of randomly created
email address for testing purposes. 

## Usage

The simplest usage would look like:

```
dotnet emlgen.dll -c <COUNT_TO_GENERATE> -o <OPTIONAL_OUTPUT_FILE> 
```

If no output file is provided, the results are written directly to the standard output.

**Example**

```
dotnet emlgen.dll -c 8
```

This will generate a list of 8 emails address and output them to the console.

```
bkcmcqk3yr5a@outlook.com
brxbqalwg@ej20udevosld2u1.gov
eb5h5sy@yahoo.com
gbjo@mail.com
mhezkrvc3sq@a41n5jpnm44jicn.edu
qem01lttwocr2hbq@ej20udevosld2u1.gov
uanx@onhsys3hguntvxz.net
uhuzmjtqws0bkdguaz@oryoi420bdwflhw.net
```

### Address Type Randomization

Unless otherwise excluded, the application will select a random percentage of
the address between 30 and 70 percent to be generated with the remainder coming
from the known mail domain pool.

### Customizing Addresses

#### Type 

Depending on your use case, it may be beneficial to control which type of addresses
are placed into the output set. To do this you can include ```-p``` and ```-g``` flags
to exclude public and generated addresses respectively.

**Example**
```
dotnet emlgen.dll -c 5 -g
```
In this example all emails from generated domains are ommitted leaving us with:

```
bbcx@mail.com
uryjtgt4fiq@icloud.com
wmebe030l2p@gmail.com
x504fka4ah@icloud.com
xtntzfh4@yahoo.com
```

#### Length

In order to facilitate a range of test types, each part of the email address (except for the TLD part) can have its max length set as a command line arguement. Use the ```-e``` switch to limit the user part of the address and the ```-d``` switch to limit the domain length.

**Example**

```
dotnet emlgen.dll -c 10 -e 5 -d 10
```

**Results**

```
bxjde@z2pexqigbhiuxlg.org
dlqm@yahoo.com
g0@hotmail.com
hkmrgnt@ceqfuisf3xficvv.edu
htmn@outlook.com
ocvvgboit@yhkjhnq0ghntjqi.com
pq2@jqoicho5l2rwhpo.edu
ukmd@hotmail.com
vs@mhejt2fh3d5w2fc.org
z@outlook.com
```

## Email Providers

For the email addresses that are generated using known public email providers the following
list is randomly selected from during creation.

* gmail.com
* yahoo.com
* hotmail.com
* outlook.com
* icloud.com
* mail.com
* aol.com

## Top Level Domains

All domains that are generated done so using known top level domains since most
traffic is seen from domains of these types. 

* com
* org
* net
* io
* gov
* edu
* mil

*Note: If needed, the code can be modified relatively easily to support less
known TLD including those for specific organizations.*


