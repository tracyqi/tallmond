import datetime
import urllib.request
import pypyodbc 
import csv
from itertools import groupby


class Quote(object):
   
  DATE_FMT = '%Y-%m-%d'
  TIME_FMT = '%H:%M:%S'
   
  def __init__(self):
    self.symbol = ''
    self.date,self.time,self.open_,self.high,self.low,self.close,self.volume = ([] for _ in range(7))
 
  def append(self,dt,open_,high,low,close,volume):
    self.date.append(dt.date())
    self.time.append(dt.time())
    self.open_.append(float(open_))
    self.high.append(float(high))
    self.low.append(float(low))
    self.close.append(float(close))
    self.volume.append(int(volume))
       
  def to_csv(self):
    return ''.join(["{0},{1},{2},{3:.2f},{4:.2f},{5:.2f},{6:.2f},{7}\n".format(self.symbol,
              self.date[bar].strftime('%Y-%m-%d'),self.time[bar].strftime('%H:%M:%S'),
              self.open_[bar],self.high[bar],self.low[bar],self.close[bar],self.volume[bar]) 
              for bar in range(len(self.close))])
     
  def write_csv(self,filename):
    with open(filename,'w') as f:
      f.write(self.to_csv())
  
  def write_sql(self):
    connection  = pypyodbc.connect('Driver={SQL Server};'
                                'Server=wg4dnymr90.database.windows.net;'
                                'Database=finance;'
                                'uid=bingfinance;pwd=KrishDirects!')

    #conn = pymssql.connect(host='wg4dnymr90.database.windows.net', user='bingfinance', password='KrishDirects!', database='finance')
    cur = connection.cursor() 
    #cur = conn.cursor()

    for bar in range(len(self.close)):
        SQLCommand = ("INSERT INTO dbo.finance "
                 "([symbol], [day], [open], [high], [low], [close], [volume]) "
                 "VALUES (%s)")
        Values = ''.join(["'{0}','{1}',{2:.2f},{3:.2f},{4:.2f},{5:.2f},{6}\n".format(self.symbol,self.date[bar].strftime('%Y-%m-%d'),self.open_[bar],self.high[bar],self.low[bar],self.close[bar],self.volume[bar])]) 

        cur.execute(SQLCommand % Values) 

        #save=cur.execute('Insert into finance(symbol, day, open, high, low, close, volume) Values(%s, %d)'
        #    %(bar.symbol, bar.date[bar].strftime('%Y-%m-%d'), bar.open_[bar],bar.high[bar],bar.low[bar],bar.close[bar],bar.volume[bar] ))
    #cur.execute('SELECT * FROM my_table')
    #row = cur.fetchone()
    #while row:
    #    print ("%s, %s" % (row[0], row[1]))
    #    row = cur.fetchone()

    connection.commit()
    connection.close()    

  def read_csv(self,filename):
    self.symbol = ''
    self.date,self.time,self.open_,self.high,self.low,self.close,self.volume = ([] for _ in range(7))
    for line in open(filename,'r'):
      symbol,ds,ts,open_,high,low,close,volume = line.rstrip().split(',')
      self.symbol = symbol
      dt = datetime.datetime.strptime(ds+' '+ts,self.DATE_FMT+' '+self.TIME_FMT)
      self.append(dt,open_,high,low,close,volume)
    return True
 
  def __repr__(self):
    return self.to_csv()

class YahooQuote(Quote):
  ''' Daily quotes from Yahoo. Date format='yyyy-mm-dd' '''
  def __init__(self,symbol,start_date,end_date=datetime.date.today().isoformat()):
    super(YahooQuote,self).__init__()
    self.symbol = symbol.upper()
    start_year,start_month,start_day = start_date.split('-')
    start_month = str(int(start_month)-1)
    end_year,end_month,end_day = end_date.split('-')
    end_month = str(int(end_month)-1)
    url_string = "http://ichart.finance.yahoo.com/table.csv?s={0}".format(symbol)
    url_string += "&a={0}&b={1}&c={2}".format(start_month,start_day,start_year)
    url_string += "&d={0}&e={1}&f={2}".format(end_month,end_day,end_year)
    try:
        with urllib.request.urlopen(url_string) as url:
            csv = url.readlines()
        #csv = urllib.urlopen(url_string).readlines()
            csv.reverse()
            for bar in range(0,len(csv)-1):
              ds,open_,high,low,close,volume,adjc = csv[bar].decode('UTF-8').rstrip().split(',')
              open_,high,low,close,adjc = [float(x) for x in [open_,high,low,close,adjc]]
              if close != adjc:
                factor = adjc/close
                open_,high,low,close = [x*factor for x in [open_,high,low,close]]
              dt = datetime.datetime.strptime(ds,'%Y-%m-%d')
              self.append(dt,open_,high,low,close,volume)
    except Exception as inst:
       print (type(inst) )    # the exception instance
       print (inst.args)      # arguments stored in .args
       print (inst)           # __str__ allows args to be printed directly
       #x, y = inst.args
       #print ('x =', x)
       #print ('y =', y )  

def write_ticker(self):
    connection  = pypyodbc.connect('Driver={SQL Server};'
                                'Server=wg4dnymr90.database.windows.net;'
                                'Database=finance;'
                                'uid=bingfinance;pwd=KrishDirects!')

    cur = connection.cursor() 
    #cur = conn.cursor()

    for bar in range(len(self.close)):
        SQLCommand = ("INSERT INTO dbo.nasdaq "
                 "([symbol], [day], [open], [high], [low], [close], [volume]) "
                 "VALUES (%s)")
        Values = ''.join(["'{0}','{1}',{2:.2f},{3:.2f},{4:.2f},{5:.2f},{6}\n".format(self.symbol,self.date[bar].strftime('%Y-%m-%d'),self.open_[bar],self.high[bar],self.low[bar],self.close[bar],self.volume[bar])]) 

        cur.execute(SQLCommand % Values) 

    connection.commit()
    connection.close()    

def readticker():
    connection  = pypyodbc.connect('Driver={SQL Server};'
                                'Server=wg4dnymr90.database.windows.net;'
                                'Database=finance;'
                                'uid=bingfinance;pwd=KrishDirects!')
    cur = connection.cursor() 

    cur.execute('''SELECT [symbol] FROM dbo.nasdaq ;''')
    for row in cur.fetchall():
        print (row[0])
        q = YahooQuote(row[0],'2015-01-01') 
        q.write_sql()
    
    cur.close()
    connection.close()


def loadticker():
    filename = "F://otherlisted.txt" #"F://nasdaqlisted.txt";
    connection  = pypyodbc.connect('Driver={SQL Server};'
                                'Server=wg4dnymr90.database.windows.net;'
                                'Database=finance;'
                                'uid=bingfinance;pwd=KrishDirects!')
    cur = connection.cursor() 
    with open(filename, 'rt') as csvfile:
        reader = csv.reader(csvfile, skipinitialspace=True, delimiter=r"|", )
        next(reader, None)  # skip the headers
        for line in reader:
                #ticket = line.split('|')
            SQLCommand = ("INSERT INTO dbo.nasdaq "
                         "([symbol], [SecurityName], [MarketCategory], [TestIssue], [FinancialStatus], [RoundLotSize]) "
                         "VALUES (?,?,?,?,?,?)")
            #Values = ''.join(["'{0}','{1}','{2}','{3}','{4}','{5}'\n".format(line[0],line[1],line[2],line[3],line[4],line[5])]) 

            #Values = [line[0],line[1],line[2],line[3],line[4],line[5]]
            cur.execute(SQLCommand ,(line[0],line[1],line[2],line[3],line[4],line[5]) ) 

    connection.commit()
    connection.close()

if __name__ == '__main__':
    readticker()
  #q = YahooQuote('aapl','2015-01-01')              # download year to date Apple data
  #print (q) 
  #loadticker()                                         # print it out
  #q = YahooQuote('orcl','2015-02-01','2015-02-28') # download Oracle data for February 2011
  #q.write_csv('orcl.csv')                          # save it to disk
  #q.write_sql()
  #q = Quote()                                      # create a generic quote object
  #q.read_csv('orcl.csv')                           # populate it with our previously saved data
  #print (q) 