create table Accounts (
Username nvarchar(20) primary key,
Rank nvarchar(14) not null CHECK (Rank IN ('Guest', 'Member', 'Trusted', 'Administrator'))
);

create table Credentials (
Username nvarchar(20) foreign key references Accounts(Username) ON UPDATE CASCADE primary key,
Password nvarchar(20));

create table Messages(
Sender nvarchar(20) NOT NULL,
Receiver nvarchar(20) NOT NULL,
Submission Datetime NOT NULL,
Message nvarchar(250) NOT NULL,
Stamp int IDENTITY(1,1) primary key);
go


INSERT INTO Accounts(Username,Rank)
Values ('admin', 'Administrator')

INSERT INTO Credentials(Username,Password)
Values ('admin','admin')


