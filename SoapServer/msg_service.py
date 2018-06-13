#!/usr/bin/env python
# encoding: utf8
#


from spyne import rpc, ServiceBase, Iterable, Integer, Unicode

import pika


#pylint: disable=E0213
class MsgService(ServiceBase):
    
    @rpc(Unicode, Integer, _returns=Iterable(Unicode))
    def say_hello(ctx, name, times):
        """Docstrings for service methods appear as documentation in the wsdl.
        <b>What fun!</b>
        @param name the name to say hello to
        @param times the number of times to say hello
        @return the completed array
        """

        for i in range(times):
            yield u'Hello, %s' % name

    @rpc(_returns=Iterable(Unicode))
    def test_pika(ctx):
        connection = pika.BlockingConnection(pika.ConnectionParameters(host='localhost'))
        channel = connection.channel()
        channel.queue_declare(queue='hello')
        while (True):
            method_frame, header_frame, body = channel.basic_get(queue = 'hello')        
            if method_frame is None or method_frame.NAME == 'Basic.GetEmpty':
                yield u''
                break
            else:
                channel.basic_ack(delivery_tag=method_frame.delivery_tag)
                yield u'%s' % body
        connection.close()
