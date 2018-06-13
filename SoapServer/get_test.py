#!/usr/bin/env python
# encoding: utf8
# 


from zeep import Client


client = Client('http://localhost:8000/?wsdl')

#for s in client.service.say_hello(u'Jérôme', 5):
#  print(s)
for s in client.service.test_pika():
  if s is not None:
    print(s)